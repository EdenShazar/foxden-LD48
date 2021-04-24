using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseDwarf : MonoBehaviour {

    [SerializeField]
    private float speed;
    private Vector3 moveDirection = Vector3.right;
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
        gameObject.transform.Translate(moveDirection * currentSpeed * Time.deltaTime);

        currentCell = GameController.Tilemap.layoutGrid.WorldToCell(transform.position);

        Vector3Int cellOnLeft = new Vector3Int(currentCell.x - 1, currentCell.y, 0);
        Vector3Int cellOnRight = new Vector3Int(currentCell.x + 1, currentCell.y, 0);
        bool hasTileOnLeft = GameController.Tilemap.HasTile(cellOnLeft);
        bool hasTileOnRight = GameController.Tilemap.HasTile(cellOnRight);

        //is the dwarf in a hole
        if ((hasTileOnLeft && hasTileOnRight) && !digging) {
            currentSpeed = 0;

            //Almost like the ClimbUpOrChangeDirection function isn't fully being completed before the code underneath is
            ClimbUpOrChangeDirection(true);

            timeElapsedBeforeSpriteFlip += Time.deltaTime;

            if (timeElapsedBeforeSpriteFlip >= timeToFlip) {
                if (dwarfSprite.flipX) {
                    Debug.Log("FLIP");
                    dwarfSprite.flipX = false;
                    moveDirection = Vector3.left;
                }
                else {
                    Debug.Log("FLIP AGAIN");
                    dwarfSprite.flipX = true;
                    moveDirection = Vector3.right;
                }
                
                timeElapsedBeforeSpriteFlip = 0;
            }

        }
        else if (hasTileOnLeft != hasTileOnRight) {
            ClimbUpOrChangeDirection(false);
        }

        //Temp solution just for testing "assigning" dig
        if (Input.GetButtonDown("Fire1")) { 
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
    }

  private void ClimbUpOrChangeDirection(bool surroundedByTiles) {
        bool canClimb = false;

        if (moveDirection == Vector3.right) {
            Vector3Int cellAboveAndToRight = new Vector3Int(currentCell.x + 1, currentCell.y + 1, 0);
            canClimb = !GameController.Tilemap.HasTile(cellAboveAndToRight);
        }
        else if (moveDirection == Vector3.left) {
            Vector3Int cellAboveAndToLeft= new Vector3Int(currentCell.x - 1, currentCell.y + 1, 0);
            canClimb = !GameController.Tilemap.HasTile(cellAboveAndToLeft);
        }

        //Is there an empty tile above one beside dwarf
        if (canClimb && !digging) {

            //if there is an empty space, stop and get ready to climb
            if (timeElapsedBeforeClimb < timeToClimb) {
                timeElapsedBeforeClimb += Time.deltaTime;
                //currentSpeed = 0;

                if (timeElapsedBeforeClimb >= timeToClimb) {
                    if (moveDirection == Vector3.right)
                        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(currentCell) + new Vector3(1.5f, 1.5f, 0);
                    else if (moveDirection == Vector3.left)
                        transform.position = GameController.Tilemap.layoutGrid.CellToWorld(currentCell) + new Vector3(-0.5f, 1.5f, 0);
                    
                    timeElapsedBeforeClimb = 0;
                    currentSpeed = speed;
                }
            }
        }
        else if (!surroundedByTiles)
        {
            dwarfSprite.flipX = !dwarfSprite.flipX;
            moveDirection *= -1f;
        }
    }

  private void OnTriggerStay2D(Collider2D collision) {
    HandleCollision(collision);
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    HandleCollision(collision);
  }

  private void HandleCollision(Collider2D collision) {
    if(Mathf.Abs(collision.ClosestPoint(transform.position).y - transform.position.y) < 0.01) {
      //Horizontal collision
      moveDirection *= -1.0f;
    }
  }
}
