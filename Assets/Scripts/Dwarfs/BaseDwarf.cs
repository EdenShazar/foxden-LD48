using UnityEngine;

public struct DwarfSurroundings {
  public Vector3Int cellOnLeft;
  public Vector3Int cellOnRight;
  public Vector3Int cellBelow;
  public bool hasTileOnLeft;
  public bool hasTileOnRight;
}


public class BaseDwarf : MonoBehaviour {

    enum Direction { LEFT = -1, RIGHT = 1 }

    [SerializeField]
    private float speed;
    private Direction moveDirection = Direction.RIGHT;
    private float timeElapsedBeforeClimb;
    private float timeElapsedBeforeSpriteFlip;
    private SpriteRenderer dwarfSprite;

    public float currentSpeed;
    public float timeToClimb;
    public float timeToFlip;

    private Vector3Int currentCell;
    private DwarfSurroundings surroundings;

    delegate bool JobAction(DwarfSurroundings surroundings);
    private JobAction doJobAction = null;

    private void Awake() {
        Physics2D.queriesStartInColliders = false;
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

        UpdateSurroundings(currentCell);

        if(doJobAction != null) {
          doJobAction(surroundings);
        } else {
          //is the dwarf in a hole
          if (surroundings.hasTileOnLeft && surroundings.hasTileOnRight) {
            currentSpeed = 0;

            ClimbUpOrChangeDirection(true);

          }
          else if (surroundings.hasTileOnLeft != surroundings.hasTileOnRight) {
            ClimbUpOrChangeDirection(false);
          }

          gameObject.transform.Translate(Vector3.right * (int)moveDirection * currentSpeed * Time.deltaTime);
        }

        //Temp solution just for testing "assigning" dig
        if (Input.GetMouseButtonDown(0)) {
          DwarfJob debugJob = ScriptableObject.CreateInstance<DigDownJob>();
          if (doJobAction == null) {
            doJobAction = debugJob.JobAction;
            debugJob.InitializeJobAction(this, currentCell);
          } else {
            doJobAction = null;
          }
        }
    }

  private void ClimbUpOrChangeDirection(bool surroundedByTiles) {
        bool canClimb = false;
        if (moveDirection == Direction.RIGHT) {
            Vector3Int cellAboveAndToRight = new Vector3Int(currentCell.x + 1, currentCell.y + 1, 0);
            canClimb = !GameController.Tilemap.HasTile(cellAboveAndToRight);
        }
        else if (moveDirection == Direction.LEFT) {
            Vector3Int cellAboveAndToLeft= new Vector3Int(currentCell.x - 1, currentCell.y + 1, 0);
            canClimb = !GameController.Tilemap.HasTile(cellAboveAndToLeft);
        }

        if (canClimb) {
            //if there is an empty space, stop and get ready to climb
            if (timeElapsedBeforeClimb < timeToClimb) {
                timeElapsedBeforeClimb += Time.deltaTime;
                //currentSpeed = 0;

                if (timeElapsedBeforeClimb >= timeToClimb) {
                    if (moveDirection == Direction.RIGHT)
                        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(currentCell) + new Vector3(1.5f, 1.5f, 0);
                    else if (moveDirection == Direction.LEFT)
                        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(currentCell) + new Vector3(-0.5f, 1.5f, 0);
                    
                    timeElapsedBeforeClimb = 0f;
                    currentSpeed = speed;
                }
            }
        }
        else {
            timeElapsedBeforeSpriteFlip += Time.deltaTime;

            if (timeElapsedBeforeSpriteFlip >= timeToFlip)
            {
                FlipDirection();

                timeElapsedBeforeSpriteFlip = 0;
            }
        }
    }

    void FlipDirection()
    {
        moveDirection = (Direction)((int)moveDirection * -1);
        dwarfSprite.flipX = moveDirection == Direction.LEFT;
    }

    private void UpdateSurroundings(Vector3Int currentCell) {
      surroundings.cellOnLeft = new Vector3Int(currentCell.x - 1, currentCell.y, 0);
      surroundings.cellOnRight = new Vector3Int(currentCell.x + 1, currentCell.y, 0);
      surroundings.cellBelow = new Vector3Int(currentCell.x, currentCell.y - 1, 0);
      surroundings.hasTileOnLeft = GameController.Tilemap.HasTile(surroundings.cellOnLeft);
      surroundings.hasTileOnRight = GameController.Tilemap.HasTile(surroundings.cellOnRight);
  }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    private void HandleCollision(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);

        foreach (ContactPoint2D contact in contacts)
            if(contact.normal.y == 0f && contact.normal.x == -(float)moveDirection)
            {
                FlipDirection();
                break;
            }
    }
}
