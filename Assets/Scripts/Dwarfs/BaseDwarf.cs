using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public struct DwarfSurroundings {
  public Vector3Int cellInFront;
  public Vector3Int cellAboveInFront;
  public Vector3Int cellBelow;
  public bool hasTileInFront;
}


public class BaseDwarf : MonoBehaviour {

  enum Direction { LEFT = -1, RIGHT = 1 }

  [SerializeField]
  private float speed;
  private Direction moveDirection = Direction.RIGHT;
  private float timeElapsedBeforeClimb;
  private float timeElapsedBeforeSpriteFlip;
  private SpriteRenderer dwarfSprite;
  private JobType currentJob = JobType.NONE;
  private DwarfAnimator animator;
  new private Transform light;

  public float distanceForHorizontalCollision;
  public float currentSpeed;
  public float timeToClimb;
  public float timeToFlip;

  private Vector3Int currentCell;
  private DwarfSurroundings surroundings;

  delegate bool JobAction(DwarfSurroundings surroundings);
  private JobAction doJobAction = null;

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
    surroundings = new DwarfSurroundings();
  }

  private void Update() {
    currentCell = GameController.Tilemap.layoutGrid.WorldToCell(transform.position);
    float dist = Mathf.Abs(GameController.Tilemap.GetCellCenterWorld(currentCell).x - transform.position.x);
    UpdateSurroundings(currentCell);

    bool doDefaultMovement = true;
    if (currentJob != JobType.NONE) {
      doDefaultMovement = doJobAction(surroundings);
    }
    if (doDefaultMovement) {
      if (!NullVector3Int.IsVector3IntNull(surroundings.cellInFront)
          && GameController.TilemapController.GetTypeOfTile(surroundings.cellInFront) != TileType.NONE) {
        ClimbUpOrChangeDirection();
      }
      gameObject.transform.Translate(Vector3.right * (int)moveDirection * currentSpeed * Time.deltaTime);
    }
  }

  public void OnMouseDown() {
    DwarfJob jobToAssign = JobSelector.GetSelectedJob();
    if (currentJob != jobToAssign.GetJobType()) {
      currentJob = jobToAssign.InitializeJobAction(this, currentCell);
      doJobAction = jobToAssign.JobAction;
    } else {
      StopJob();
    }
  }

  public void StopJob() {
    currentJob = JobType.NONE;
  }

  private void ClimbUpOrChangeDirection() {
    bool canClimb = false;
    Vector3Int cellAboveFront = surroundings.cellAboveInFront;

    canClimb = !NullVector3Int.IsVector3IntNull(cellAboveFront) && !GameController.Tilemap.HasTile(cellAboveFront);

    if (canClimb) {
      //if there is an empty space, stop and get ready to climb
      if (timeElapsedBeforeClimb < timeToClimb) {
        timeElapsedBeforeClimb += Time.deltaTime;
        currentSpeed = 0;

        if (timeElapsedBeforeClimb >= timeToClimb) {
          transform.position = GameController.Tilemap.layoutGrid.CellToWorld(cellAboveFront);
          timeElapsedBeforeClimb = 0f;
          currentSpeed = speed;
        }
      }
    } else {
      timeElapsedBeforeSpriteFlip += Time.deltaTime;
        currentSpeed = 0;

      if (timeElapsedBeforeSpriteFlip >= timeToFlip) {
        FlipDirection();
        currentSpeed = speed;
        timeElapsedBeforeSpriteFlip = 0;
      }
    }
  }

  void FlipDirection() {
    moveDirection = (Direction)((int)moveDirection * -1);
    dwarfSprite.flipX = moveDirection == Direction.LEFT;
    light.localPosition = new Vector3(-light.localPosition.x, light.localPosition.y, 0f);
    light.localRotation = Quaternion.Euler(0f, 0f, -light.rotation.eulerAngles.z);
  }

  private void UpdateSurroundings(Vector3Int currentCell) {
    bool horizontalCollision;
    if(moveDirection == Direction.RIGHT) {
       horizontalCollision = GameController.Tilemap.GetCellCenterWorld(currentCell).x 
         - transform.position.x < -distanceForHorizontalCollision;
    }else {
       horizontalCollision = GameController.Tilemap.GetCellCenterWorld(currentCell).x 
         - transform.position.x > distanceForHorizontalCollision;
    }
    if(horizontalCollision) {
      if (moveDirection == Direction.LEFT) {
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
  }
}
