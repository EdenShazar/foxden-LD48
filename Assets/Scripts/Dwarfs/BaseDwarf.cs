using UnityEngine;

public class BaseDwarf : MonoBehaviour {

    enum Direction { LEFT = -1, RIGHT = 1 }

    [SerializeField]
    private float speed;
    private Direction moveDirection = Direction.RIGHT;
    private float timeElapsedBeforeClimb;
    private float timeElapsedBeforeDig;
    private float timeElapsedBeforeSpriteFlip;
    public float currentSpeed;
    private bool digging;
    private bool ableToDig;
    private SpriteRenderer dwarfSprite;

    public float timeToClimb;
    public float timeToDig;
    public float timeToFlip;

    private Vector3Int currentCell;

    private void Awake() {
        Physics2D.queriesStartInColliders = false;
    }

    private void Start() {
        timeElapsedBeforeClimb = 0;
        timeElapsedBeforeDig = 0;
        timeElapsedBeforeSpriteFlip = 0;
        currentSpeed = speed;
        dwarfSprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        currentCell = GameController.Tilemap.layoutGrid.WorldToCell(transform.position);

        Vector3Int cellOnLeft = new Vector3Int(currentCell.x - 1, currentCell.y, 0);
        Vector3Int cellOnRight = new Vector3Int(currentCell.x + 1, currentCell.y, 0);
        bool hasTileOnLeft = GameController.Tilemap.HasTile(cellOnLeft);
        bool hasTileOnRight = GameController.Tilemap.HasTile(cellOnRight);

        //is the dwarf in a hole
        if ((hasTileOnLeft && hasTileOnRight) && !digging) {
            currentSpeed = 0;

            ClimbUpOrChangeDirection(true);

        }
        else if (hasTileOnLeft != hasTileOnRight) {
            ClimbUpOrChangeDirection(false);
        }

        //Temp solution just for testing "assigning" dig
        if (Input.GetMouseButtonDown(0)) { 
            if (ableToDig) {
                ableToDig = false;
                currentSpeed = speed;
            }
            else {
                ableToDig = true;
                transform.position = GameController.Tilemap.layoutGrid.CellToWorld(currentCell) + new Vector3(0.5f, 0.5f, 0);
            }
        }

        if (ableToDig) {
            digging = true;
            currentSpeed = 0;
        }
        else {
            digging = false;
        }

        if (digging) {
            Vector3Int cellBelow = new Vector3Int(currentCell.x, currentCell.y - 1, 0);
            bool hasTileBelow = GameController.Tilemap.HasTile(cellBelow);

            if (hasTileBelow) {

                timeElapsedBeforeDig += Time.deltaTime;

                if(timeElapsedBeforeDig >= timeToDig) {
                    GameController.TilemapController.RemoveTile(cellBelow);

                    timeElapsedBeforeDig = 0;
                }
            }
        }

        
        gameObject.transform.Translate(Vector3.right * (int)moveDirection * currentSpeed * Time.deltaTime);
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

        if (canClimb && !digging) {
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
        //else if (!surroundedByTiles)
        //    FlipDirection();
    }

    void FlipDirection()
    {
        moveDirection = (Direction)((int)moveDirection * -1);
        dwarfSprite.flipX = moveDirection == Direction.LEFT;
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
