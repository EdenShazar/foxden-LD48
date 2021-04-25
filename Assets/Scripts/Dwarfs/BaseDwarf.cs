using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public struct DwarfSurroundings {
  public Vector3Int cellInFront;
  public Vector3Int cellAboveInFront;
  public Vector3Int cellBelow;
  public Vector3Int cellBelowInFront;
  public bool hasTileInFront;
  public bool hasTileBelow;
  public bool hasTileBelowInFront;
}

public enum Direction { LEFT = -1, RIGHT = 1 }

public class BaseDwarf : MonoBehaviour {

    const float climbLedgeRopeAnimationTime = 1f;

    [SerializeField]
    private float speed;
    private float timeElapsedBeforeClimb;
    private float timeElapsedBeforeSpriteFlip;
    private bool isClimbingLedge;
    private SpriteRenderer dwarfSprite;
    private JobType currentJob = JobType.NONE;
    new private Transform light;

    [HideInInspector] public DwarfAnimator animator;

    [HideInInspector] public Rigidbody2D Rigidbody { get; private set; }
    [HideInInspector] public JobIconChanger JobIcon { get; private set; }
    [HideInInspector] public Direction MoveDirection { get; private set; } = Direction.RIGHT;
    [HideInInspector] public bool IsFalling { get; private set; }

    public float distanceForHorizontalCollision;
    public float currentSpeed;
    public float timeToClimb;
    public float timeToFlip;

  public Vector3Int CurrentCell { get; private set; }
  private DwarfSurroundings surroundings;

  delegate bool JobAction(DwarfSurroundings surroundings);
  private JobAction doJobAction = null;
  private Func<bool> canStopJob;

  private void Awake() {
    Physics2D.queriesStartInColliders = false;

    animator = GetComponent<DwarfAnimator>();
    light = GetComponentInChildren<Light2D>().transform;
  }

  private void Start() {
    timeElapsedBeforeClimb = 0;
    timeElapsedBeforeSpriteFlip = 0;
    currentSpeed = speed;
    dwarfSprite = GetComponent<SpriteRenderer>();
    Rigidbody = GetComponent<Rigidbody2D>();
    JobIcon = GetComponentInChildren<JobIconChanger>();
    surroundings = new DwarfSurroundings();
    animator.Initialize(this);
  }

  private void Update() {
    CurrentCell = GameController.Tilemap.layoutGrid.WorldToCell(transform.position);
    float dist = Mathf.Abs(GameController.Tilemap.GetCellCenterWorld(CurrentCell).x - transform.position.x);
    UpdateSurroundings(CurrentCell);
    UpdateIsFalling();

    bool doDefaultMovement = true;
    if (currentJob != JobType.NONE) {
      doDefaultMovement = doJobAction(surroundings);
    }
    if (doDefaultMovement) {
      if (!NullVector3Int.IsVector3IntNull(surroundings.cellInFront)
          && GameController.TilemapController.GetTypeOfTile(surroundings.cellInFront) != TileType.NONE) {
        ClimbUpOrChangeDirection();
      }
      gameObject.transform.Translate(Vector3.right * (int)MoveDirection * currentSpeed * Time.deltaTime);
    }
  }

    public void OnMouseDown() {
        DwarfJob jobToAssign = JobSelector.GetSelectedJob();

        if (currentJob != JobType.NONE) {
            // Can't override current job; ordered to stop previous job first
            StopJob();
        } else if (currentJob == jobToAssign.GetJobType()) {
            // Can't reassign the same job again
            return;
        } else {
            // Assign new job
            currentJob = jobToAssign.InitializeJobAction(this, CurrentCell);
            doJobAction = jobToAssign.JobAction;
            canStopJob = jobToAssign.CanStopJob;
        }
    }

  public void StopJob() {
    if (!canStopJob())
        return;

    if (currentJob == JobType.STOP) {
        if (GameController.TilemapController.GetTypeOfTile(CurrentCell) == TileType.DWARF)
            GameController.TilemapController.RemoveTile(CurrentCell);
    }

    currentJob = JobType.NONE;
    animator.Walk();
    JobIcon.RemoveIcon();
    currentSpeed = speed;
  }

    public void SnapToCurrentCell()
    {
        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(CurrentCell) + Vector3.right * 0.5f;
    }

    public void SnapToRelativeCell(Vector3Int cellMovement)
    {
        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(CurrentCell + cellMovement) + Vector3.right * 0.5f;
    }

    private void ClimbUpOrChangeDirection() {
    bool canClimb;
    Vector3Int cellAboveFront = surroundings.cellAboveInFront;
    Vector3Int cellInFront = surroundings.cellInFront;

    canClimb = !NullVector3Int.IsVector3IntNull(cellAboveFront) && !GameController.Tilemap.HasTile(cellAboveFront);

    if(GameController.TilemapController.GetTypeOfTile(cellInFront) == TileType.DWARF) {
        canClimb = false;
    }

    if (canClimb) {
      StartCoroutine(ClimbLege());
    } else {
      timeElapsedBeforeSpriteFlip += Time.deltaTime;
        currentSpeed = 0;

      if (timeElapsedBeforeSpriteFlip >= timeToFlip) {
        FlipDirection();
        ResetSpeed();
        timeElapsedBeforeSpriteFlip = 0;
      }
    }
  }

    IEnumerator ClimbLege()
    {
        // Ensure coroutine isn't running more than once
        if (isClimbingLedge)
            yield break;

        isClimbingLedge = true;

        yield return new WaitForSeconds(timeToClimb);

        Rigidbody.gravityScale = 0f;

        SnapToCurrentCell();
        animator.ClimbLedge();

        yield return new WaitForSeconds(climbLedgeRopeAnimationTime);

        isClimbingLedge = false;
        Rigidbody.gravityScale = 1f;
        SnapToRelativeCell(Vector3Int.up + Vector3Int.right * (int)MoveDirection);
        animator.Walk();
        ResetSpeed();
    }

  public void FlipDirection() {
    MoveDirection = (Direction)((int)MoveDirection * -1);
    dwarfSprite.flipX = MoveDirection == Direction.LEFT;
    light.localPosition = new Vector3(-light.localPosition.x, light.localPosition.y, 0f);
    light.localRotation = Quaternion.Euler(0f, 0f, -light.rotation.eulerAngles.z);
  }

    public void ResetSpeed()
    {
        currentSpeed = speed;
    }

  private void UpdateSurroundings(Vector3Int currentCell) {
    bool horizontalCollision;
    if(MoveDirection == Direction.RIGHT) {
       horizontalCollision = GameController.Tilemap.GetCellCenterWorld(currentCell).x 
         - transform.position.x < -distanceForHorizontalCollision;
    }else {
       horizontalCollision = GameController.Tilemap.GetCellCenterWorld(currentCell).x 
         - transform.position.x > distanceForHorizontalCollision;
    }
    if(horizontalCollision) {
      if (MoveDirection == Direction.LEFT) {
        surroundings.cellInFront = new Vector3Int(currentCell.x - 1, currentCell.y, 0);
        surroundings.cellAboveInFront = new Vector3Int(currentCell.x - 1, currentCell.y + 1, 0);
      } else {
        surroundings.cellInFront = new Vector3Int(currentCell.x + 1, currentCell.y, 0);
        surroundings.cellAboveInFront = new Vector3Int(currentCell.x + 1, currentCell.y + 1, 0);
      }
    } else {
      surroundings.cellInFront = NullVector3Int.GetNullVector3Int();
      surroundings.cellBelow = NullVector3Int.GetNullVector3Int();
    }
    surroundings.hasTileInFront = GameController.Tilemap.HasTile(surroundings.cellInFront);
    surroundings.cellBelow = new Vector3Int(currentCell.x, currentCell.y - 1, 0);
    surroundings.hasTileBelow = GameController.Tilemap.HasTile(surroundings.cellBelow);

    surroundings.cellBelowInFront = new Vector3Int(currentCell.x + (int)MoveDirection, currentCell.y - 1, 0);
    surroundings.hasTileBelowInFront = GameController.Tilemap.HasTile(surroundings.cellBelowInFront);
    }

    private void UpdateIsFalling()
    {
        IsFalling = Rigidbody.velocity.y <= -0.01f && surroundings.hasTileBelow;
    }
}
