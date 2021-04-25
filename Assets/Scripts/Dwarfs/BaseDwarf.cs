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
  public bool hasTileAboveInFront;
  public bool hasTileBelow;
  public bool hasTileBelowInFront;
}

public enum Direction { LEFT = -1, RIGHT = 1 }

public class BaseDwarf : MonoBehaviour {

    const float climbLedgeRopeAnimationTime = 1f;

    [SerializeField]
    private float speed;
    private float timeElapsedBeforeDirectionFlip = 0f;
    //0 is sober, Goes down by 1 a second base
    private float maxDrunkAmount = 100.0f;
    private float currentDrunkAmount = 100.0f;
    private bool isClimbingLedge;
    private SpriteRenderer dwarfSprite;
    private DwarfJob currentJob = null;
    new private Transform light;

    [HideInInspector] public DwarfAnimator animator;
    [HideInInspector] public AudioPlayer audioPlayer;

    [HideInInspector] public Rigidbody2D Rigidbody { get; private set; }
    [HideInInspector] public JobIconChanger JobIcon { get; private set; }
    [HideInInspector] public Direction MoveDirection { get; private set; } = Direction.RIGHT;
    [HideInInspector] public bool IsFalling { get; private set; }

    public float distanceForHorizontalCollision;
    public float currentSpeed;
    public float timeToClimb;
    public float timeToFlipDirection;
    
  public Vector3Int CurrentCell { get; private set; }
  private DwarfSurroundings surroundings;

  delegate bool JobAction(DwarfSurroundings surroundings);
  private JobAction doJobAction = null;
  private Func<bool> canStopJob;

  private void Awake() {
    Physics2D.queriesStartInColliders = false;

    animator = GetComponent<DwarfAnimator>();
    light = GetComponentInChildren<Light2D>().transform;
    audioPlayer = GetComponent<AudioPlayer>();
  }

  private void Start() {
    currentSpeed = speed;
    dwarfSprite = GetComponent<SpriteRenderer>();
    Rigidbody = GetComponent<Rigidbody2D>();
    JobIcon = GetComponentInChildren<JobIconChanger>();
    surroundings = new DwarfSurroundings();
    animator.Initialize(this);
    dwarfSprite.sortingLayerID = Constants.nonworkingDwarvesLayer;
  }

    private void Update()
    {
        CurrentCell = GameController.Tilemap.layoutGrid.WorldToCell(transform.position);

        if (currentJob != null) {
          currentDrunkAmount -= Time.deltaTime * currentJob.sobrietyScale;
        } else {
          currentDrunkAmount -= Time.deltaTime;
        }
        Debug.Log(currentDrunkAmount);
        if(currentDrunkAmount < 0.0f) {
          RemoveDwarf();
        }
        UpdateSurroundings();
        UpdateIsFalling();

        if (!IsFalling)
        {
            bool doDefaultMovement = true;
            if (currentJob != null)
                doDefaultMovement = doJobAction(surroundings);

            if (doDefaultMovement)
            {
                if (surroundings.hasTileInFront)
                    ClimbUpOrChangeDirection();

                gameObject.transform.Translate(Vector3.right * (int)MoveDirection * currentSpeed * Time.deltaTime);
            }
        }

        if (GameController.workingDwarvesHoveredOver.Contains(this))
            GameController.workingDwarvesHoveredOver.Remove(this);
    }

    public void OnMouseDown() {
        DwarfJob jobToAssign = JobSelector.GetSelectedJob();

        if (currentJob != null) {
            // Can't override current job; ordered to stop previous job first
            StopJob();
        } else if (currentJob.GetJobType() == jobToAssign.GetJobType()) {
            // Can't reassign the same job again
            return;
        } else {
            // Assign new job
            currentJob = jobToAssign;
            currentJob.InitializeJobAction(this, CurrentCell);
            doJobAction = jobToAssign.JobAction;
            canStopJob = jobToAssign.CanStopJob;
            dwarfSprite.sortingLayerID = Constants.workingDwarvesLayer;
        }
    }

    public void OnMouseOver()
    {
        if (currentJob == null)
        {
            if (GameController.workingDwarvesHoveredOver.Contains(this))
                GameController.workingDwarvesHoveredOver.Remove(this);
        }
        else if (!GameController.workingDwarvesHoveredOver.Contains(this))
            GameController.workingDwarvesHoveredOver.Add(this);

    }

    public void StopJob() {
      if (!canStopJob())
        return;

      currentJob = null;
      animator.Walk();
      JobIcon.RemoveIcon();
      dwarfSprite.sortingLayerID = Constants.nonworkingDwarvesLayer;
      currentSpeed = speed;
    }

    public void ResetDrunk() {
      currentDrunkAmount = maxDrunkAmount;
    }

    public void RemoveDwarf() {
      Destroy(gameObject);
    }

    public void SnapToCurrentCell()
    {
        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(CurrentCell) + Vector3.right * 0.5f;
    }

    public void SnapToRelativeCell(Vector3Int cellMovement)
    {
        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(CurrentCell + cellMovement) + Vector3.right * 0.5f;
    }

    private void ClimbUpOrChangeDirection()
    {
        bool canClimb = surroundings.hasTileInFront && !surroundings.hasTileAboveInFront
            && !GameController.TilemapController.IsCellOccupiedWithDwarf(surroundings.cellInFront);

        if (canClimb)
        {
            StartCoroutine(ClimbLege());
            return;
        }

        timeElapsedBeforeDirectionFlip += Time.deltaTime;
        currentSpeed = 0;

        if (timeElapsedBeforeDirectionFlip >= timeToFlipDirection)
        {
            FlipDirection();
            ResetSpeed();
            timeElapsedBeforeDirectionFlip = 0f;
        }
    }

    IEnumerator ClimbLege()
    {
        // Ensure coroutine isn't running more than once
        if (isClimbingLedge)
            yield break;

        isClimbingLedge = true;

        yield return new WaitForSeconds(timeToClimb);

        // If can no longer climb by end of waiting time, abort
        bool canClimb = surroundings.hasTileInFront && !surroundings.hasTileAboveInFront
            && !GameController.TilemapController.IsCellOccupiedWithDwarf(surroundings.cellInFront);
        if (!canClimb)
            yield break;

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

    private void UpdateSurroundings()
    {
        int moveDirectionInt = (int)MoveDirection;

        surroundings.cellInFront = new Vector3Int(CurrentCell.x + moveDirectionInt, CurrentCell.y, 0);
        surroundings.hasTileInFront = GameController.TilemapController.HasTileOrDwarf(surroundings.cellInFront);

        surroundings.cellAboveInFront = new Vector3Int(CurrentCell.x + moveDirectionInt, CurrentCell.y + 1, 0);
        surroundings.hasTileAboveInFront = GameController.TilemapController.HasTileOrDwarf(surroundings.cellAboveInFront);

        surroundings.cellBelow = new Vector3Int(CurrentCell.x, CurrentCell.y - 1, 0);
        surroundings.hasTileBelow = GameController.TilemapController.HasTileOrDwarf(surroundings.cellBelow);

        surroundings.cellBelowInFront = new Vector3Int(CurrentCell.x + (int)MoveDirection, CurrentCell.y - 1, 0);
        surroundings.hasTileBelowInFront = GameController.TilemapController.HasTileOrDwarf(surroundings.cellBelowInFront);
    }

    private void UpdateIsFalling()
    {
        IsFalling = Rigidbody.velocity.y <= -0.01 && !surroundings.hasTileBelow;
    }
}
