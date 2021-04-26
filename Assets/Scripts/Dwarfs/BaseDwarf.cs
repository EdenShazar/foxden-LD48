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
  public float cellDistanceToTileInFront;
  public float cellDistanceToTileBelow;
}

public enum Direction { LEFT = -1, RIGHT = 1 }

public class BaseDwarf : MonoBehaviour {

    const float climbLedgeAnimationTime = 1f;
    const float puffAnimationTime = 0.75f;

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
    new private Collider2D collider;
    private Direction lightDirection = Direction.RIGHT;

    private bool doDefaultMovement = true;

    [HideInInspector] public DwarfAnimator animator;
    [HideInInspector] public AudioPlayer audioPlayer;

    [HideInInspector] public Rigidbody2D Rigidbody { get; private set; }
    [HideInInspector] public JobIconChanger JobIcon { get; private set; }
    [HideInInspector] public Direction MoveDirection { get; private set; } = Direction.RIGHT;
    [HideInInspector] public bool IsFalling { get; private set; }
    [HideInInspector] public DwarfJob CurrentJob { get => currentJob; }

    [HideInInspector] public bool isAtWagon;

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
    collider = GetComponent<Collider2D>();
  }

    private void Start()
    {
        currentSpeed = speed;
        dwarfSprite = GetComponent<SpriteRenderer>();
        Rigidbody = GetComponent<Rigidbody2D>();
        JobIcon = GetComponentInChildren<JobIconChanger>();
        surroundings = new DwarfSurroundings();
        animator.Initialize(this);
        dwarfSprite.sortingLayerID = Constants.nonworkingDwarvesLayer;

        GameController.DwarfManager.RegisterDwarf(this);

        if (Rigidbody.velocity.x <= 0f)
            FlipDirection();
    }

    private void OnDestroy()
    {
        GameController.DwarfManager.UnregisterDwarf(this);
    }

    private void Update()
    {
        CurrentCell = GameController.Tilemap.layoutGrid.WorldToCell(transform.position);

        if (currentJob != null) {
          currentDrunkAmount -= Time.deltaTime * currentJob.SobrietyScale;
        } else {
          currentDrunkAmount -= Time.deltaTime;
        }
        if(currentDrunkAmount < 0.0f) {
          RemoveDwarf();
        }
        UpdateSurroundings();
        UpdateIsFalling();

        if (!IsFalling)
        {
            doDefaultMovement = true;
            if (currentJob != null)
                doDefaultMovement = doJobAction(surroundings);

            if (doDefaultMovement)
            {
                if (surroundings.hasTileInFront && surroundings.cellDistanceToTileInFront <= Constants.horizontalInteractionDistance
                    || isAtWagon)
                    ClimbUpOrChangeDirection();

                gameObject.transform.Translate(Vector3.right * (int)MoveDirection * currentSpeed * Time.deltaTime);
            }
        }

        if (GameController.workingDwarvesHoveredOver.Contains(this))
            GameController.workingDwarvesHoveredOver.Remove(this);

        if (currentJob == null)
        {
            // Update sorting
            dwarfSprite.sortingLayerID = Constants.nonworkingDwarvesLayer;
            transform.position = new Vector3(transform.position.x, transform.position.y, currentDrunkAmount * 0.01f);

            if (!surroundings.hasTileInFront)
                ResetSpeed();
        }
        else
        {
            // Update sorting
            dwarfSprite.sortingLayerID = Constants.workingDwarvesLayer;
            transform.position = new Vector3(transform.position.x, transform.position.y, -5f + currentDrunkAmount * 0.01f);
        }


        if (MoveDirection != lightDirection)
        {
            light.localPosition = new Vector3(-light.localPosition.x, light.localPosition.y, 0f);
            light.localRotation = Quaternion.Euler(0f, 0f, -light.rotation.eulerAngles.z);

            lightDirection = (Direction)((int)lightDirection * -1f);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer != Constants.wagonLayer)
            return;

        isAtWagon = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer != Constants.wagonLayer)
            return;

        isAtWagon = false;
    }

    public void OnMouseDown() {
        DwarfJob jobToAssign = JobSelector.GetSelectedJob();

        if (currentJob != null) {
            // Can't override current job; ordered to stop previous job first
            StopJob();
        } else if (currentJob != null && currentJob.GetJobType() == jobToAssign.GetJobType()) {
            // Can't reassign the same job again
            return;
        } else {
            // Assign new job
            currentJob = jobToAssign;
            if (jobToAssign) {
                currentJob.InitializeJobAction(this, CurrentCell);
                doJobAction = jobToAssign.JobAction;
                canStopJob = jobToAssign.CanStopJob;
                GameController.AddToScore(-1 * currentJob.JobCost);
                dwarfSprite.sortingLayerID = Constants.workingDwarvesLayer;
            }
        }
    }

    public void ForceJob(DwarfJob job)
    {
        if (currentJob != null)
            StopJob();

        currentJob = job;
        job.InitializeJobAction(this, CurrentCell);
        doJobAction = job.JobAction;
        canStopJob = job.CanStopJob;
        dwarfSprite.sortingLayerID = Constants.workingDwarvesLayer;
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

      currentJob.FinalizeJobAction();

      currentJob = null;
      animator.Walk();
      JobIcon.RemoveIcon();
      currentSpeed = speed;
      doDefaultMovement = true;
    }

    public void ResetDrunk() {
      currentDrunkAmount = maxDrunkAmount;
    }

    public void RemoveDwarf() {
      StartCoroutine(PuffAndRemoveDwarf());
    }

    IEnumerator PuffAndRemoveDwarf()
    {
        animator.Puff();
        JobIcon.RemoveIcon();
        currentSpeed = 0f;
        
        if (currentJob != null)
            currentJob.FinalizeJobAction();
        currentJob = null;

        yield return new WaitForSeconds(puffAnimationTime);

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
            StartCoroutine(ClimbLedge());
            return;
        }

        timeElapsedBeforeDirectionFlip += Time.deltaTime;
        currentSpeed = 0;

        if (timeElapsedBeforeDirectionFlip >= timeToFlipDirection)
        {
            FlipDirection();
            ResetSpeed();
            timeElapsedBeforeDirectionFlip = 0f;
            isAtWagon = false;
        }
    }

    IEnumerator ClimbLedge()
    {
        // Ensure coroutine isn't running more than once
        if (isClimbingLedge)
            yield break;

        isClimbingLedge = true;
        currentSpeed = 0f;

        yield return new WaitForSeconds(timeToClimb);

        // If returned to default movement mid-wait, abort
        if (!doDefaultMovement)
        {
            isClimbingLedge = false;
            yield break;
        }

        // If can no longer climb by end of waiting time, abort
        bool canClimb = surroundings.hasTileInFront && !surroundings.hasTileAboveInFront
            && !GameController.TilemapController.IsCellOccupiedWithDwarf(surroundings.cellInFront);
        if (!canClimb)
        {
            isClimbingLedge = false;
            yield break;
        }

        Rigidbody.gravityScale = 0f;

        SnapToCurrentCell();
        animator.ClimbLedge();

        yield return new WaitForSeconds(climbLedgeAnimationTime);

        isClimbingLedge = false;
        Rigidbody.gravityScale = 1f;
        SnapToRelativeCell(Vector3Int.up + Vector3Int.right * (int)MoveDirection);
        animator.Walk();
        ResetSpeed();
    }

  public void FlipDirection() {
    MoveDirection = (Direction)((int)MoveDirection * -1);
    dwarfSprite.flipX = MoveDirection == Direction.LEFT;
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

        if (!surroundings.hasTileInFront)
            surroundings.cellDistanceToTileInFront = 1f;
        else
        {
            float cellEdgeX;
            if (MoveDirection == Direction.LEFT)
            {
                cellEdgeX = collider.bounds.min.x;
                surroundings.cellDistanceToTileInFront = (cellEdgeX - GameController.Tilemap.GetCellCenterWorld(surroundings.cellInFront).x) / GameController.Tilemap.cellSize.x - 0.5f;
            }
            else
            {
                cellEdgeX = collider.bounds.max.x;
                surroundings.cellDistanceToTileInFront = (GameController.Tilemap.GetCellCenterWorld(surroundings.cellInFront).x - cellEdgeX) / GameController.Tilemap.cellSize.x - 0.5f;
            }
        }

        if (!surroundings.hasTileBelow)
            surroundings.cellDistanceToTileBelow = 1f;
        else
            surroundings.cellDistanceToTileBelow = (collider.bounds.min.y - GameController.Tilemap.GetCellCenterWorld(surroundings.cellBelow).y) / GameController.Tilemap.cellSize.y - 0.5f;
    }

    private void UpdateIsFalling()
    {
        if (Mathf.Abs(Rigidbody.velocity.y) <= Constants.fallingSpeedThreshold)
        {
            IsFalling = false;
            return;
        }

        if (!surroundings.hasTileBelow)
        {
            // No tile immediately below, so far enough from ground
            IsFalling = true;
            return;
        }    

        if (surroundings.cellDistanceToTileBelow > 0f)
        {
            // Tile exists immediately below, but not close enough yet
            IsFalling = true;
            return;
        }

        IsFalling = false;
    }
}
