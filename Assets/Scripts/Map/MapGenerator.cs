using UnityEngine;

public class MapGenerator : MonoBehaviour {

  [SerializeField] private int leftBoundary = -5;
  [SerializeField] private int rightBoundary = 5;
  [SerializeField] private int topBoundary = 2;
  [SerializeField] private int bottomBoundary = -50;

  public int LeftBoundary { get => leftBoundary; }
  public int RightBoundary { get => rightBoundary; }
  public int TopBoundary { get => topBoundary; }
  public int BottomBoundary { get => bottomBoundary; }

  public void GenerateMap() {
    //Left and right boundary
    GenerateBox(leftBoundary, topBoundary, leftBoundary, bottomBoundary, TileType.STONE);
    GenerateBox(rightBoundary, topBoundary, rightBoundary, bottomBoundary, TileType.STONE);

    //Fill the center
    GenerateBox(leftBoundary + 1, topBoundary - 2, rightBoundary - 1, bottomBoundary, TileType.DIRT);
  }

  public void GenerateBox(int topLeftX, int topLeftY, int botRightX, int botRightY, TileType type) {
    for(int y = topLeftY; y >= botRightY; y--) {
      for(int x = topLeftX; x <= botRightX; x++) {
        GameController.TilemapController.InitializeTile(x, y, type);
      }
    }
  }
}
