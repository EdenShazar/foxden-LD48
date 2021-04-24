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

        RaycastHit2D leftSide = Physics2D.Raycast(transform.position, Vector2.left, 0.6f);
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position, Vector2.right, 0.6f);
        Debug.DrawRay(transform.position, Vector2.left, Color.yellow, 0.6f);
        Debug.DrawRay(transform.position, Vector2.right, Color.yellow, 0.6f);

        //is the dwarf in a hole
        if ((leftSide.collider && rightSide.collider) && !digging) {
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
        else if (leftSide.collider && !rightSide.collider) {
            Tilemap tileMap = leftSide.collider.GetComponent<Tilemap>();
            if (tileMap) {
                Grid grid = tileMap.layoutGrid;

                Vector3Int tileCoordinates = grid.WorldToCell(leftSide.point);

                TileBase tileCollidedWith = tileMap.GetTile(tileCoordinates);

                Vector3 abovePlayerPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                RaycastHit2D checkForClimableTile = Physics2D.Raycast(abovePlayerPosition, Vector2.left, 0.5f);

                ClimbUpOrChangeDirection(false);
            }
            
        }
        else if (rightSide.collider && !leftSide.collider) {
            Tilemap tileMap = rightSide.collider.GetComponent<Tilemap>();
            if (tileMap) {
                Grid grid = tileMap.layoutGrid;
                Vector3Int tileCoordinates = grid.WorldToCell(rightSide.point);

                //This seems really bad!!!! - TODO
                //On left side coordinates of the tile are returned correctly
                //but on the right it's one tile too short to reach so have artificially added an extra grid
                tileCoordinates.x = tileCoordinates.x + 1;

                TileBase tileCollidedWith = tileMap.GetTile(tileCoordinates);

                Vector3 abovePlayerPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                RaycastHit2D checkForClimableTile = Physics2D.Raycast(abovePlayerPosition, Vector2.right, 0.5f);

                ClimbUpOrChangeDirection(false);
            }
        }

        //Temp solution just for testing "assigning" dig
        if (Input.GetButtonDown("Fire1")) { 
            if (ableToDig) {
                ableToDig = false;
                currentSpeed = speed;
            }
            else {
                ableToDig = true;
            }
        }


        if (ableToDig) {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z);
            digging = true;
            currentSpeed = 0;
        }
        else {
            digging = false;
        }

        if (digging) {
            RaycastHit2D checkDown = Physics2D.Raycast(transform.position, Vector2.down, 1f);
            Debug.DrawRay(transform.position, Vector2.down, Color.yellow, 1f);

            if (checkDown.collider) {

                timeElapsedBeforeDig += Time.deltaTime;

                if(timeElapsedBeforeDig >= timeToDig) {
                    Tilemap tileMap = checkDown.collider.GetComponent<Tilemap>();
                    Grid grid = tileMap.layoutGrid;

                    Vector3Int tileCoordinates = grid.WorldToCell(checkDown.point);

                    TileBase tileToDig = tileMap.GetTile(tileCoordinates);

                    //Just setting to null at the moment, should be replaced with a proper function that resolves what happens when a tile is dug out - TODO 
                    GameController.TilemapController.RemoveTile(tileCoordinates.x, tileCoordinates.y);

                    timeElapsedBeforeDig = 0;
                }
            }
        }
    }

  private void ClimbUpOrChangeDirection(bool surroundedByTiles) {
        Vector3 abovePlayerPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        RaycastHit2D checkForClimableTile = new RaycastHit2D();

        if (moveDirection == Vector3.right) {
            Debug.Log("GOING RIGHT");
            checkForClimableTile = Physics2D.Raycast(abovePlayerPosition, Vector2.right, 0.6f);
        }
        else if (moveDirection == Vector3.left) {
            Debug.Log("GOING LEFT");
            checkForClimableTile = Physics2D.Raycast(abovePlayerPosition, Vector2.left, 0.6f);
        }

        //Is there an empty tile above one beside dwarf
        if (!checkForClimableTile.collider && !digging) {

            //if there is an empty space, stop and get ready to climb
            if (timeElapsedBeforeClimb < timeToClimb) {
                timeElapsedBeforeClimb += Time.deltaTime;
                //currentSpeed = 0;


                if (timeElapsedBeforeClimb >= timeToClimb) {
                    if (moveDirection == Vector3.right) {
                        transform.position = new Vector3(transform.position.x + 1, transform.position.y + 1, transform.position.z);
                    }
                    else if (moveDirection == Vector3.left) {
                        transform.position = new Vector3(transform.position.x - 1, transform.position.y + 1, transform.position.z);
                    }
                    
                    timeElapsedBeforeClimb = 0;
                    currentSpeed = speed;
                }
            }

        }
        else {
            if (!surroundedByTiles) {


                if (dwarfSprite.flipX) {
                    Debug.Log("FLIP");
                    dwarfSprite.flipX = false;
                    moveDirection = Vector3.right;
                }
                else {
                    Debug.Log("FLIP AGAIN");
                    dwarfSprite.flipX = true;
                    moveDirection = Vector3.left;
                }
            }
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
