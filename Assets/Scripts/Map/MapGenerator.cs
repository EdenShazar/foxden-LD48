using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {

  [SerializeField] private int leftBoundary;
  [SerializeField] private int rightBoundary;
  [SerializeField] private Tilemap dirtTilemap;
  [SerializeField] private Tilemap stomeTilemap;
  private Tilemap tilemap;

  public void setTilemap(Tilemap incTilemap) {
    tilemap = incTilemap;
  }

  public void GenerateMap() {
    int topBoundary = 2;
    int bottomBoundary = -50;
    //Left and right boundary
    GenerateBox(leftBoundary, topBoundary, leftBoundary, bottomBoundary, TileType.STONE);
    GenerateBox(rightBoundary, topBoundary, rightBoundary, bottomBoundary, TileType.STONE);

    //Fill the center
    GenerateBox(leftBoundary + 1, topBoundary - 2, rightBoundary - 1, bottomBoundary, TileType.DIRT);
  }

  public void GenerateBox(int topLeftX, int topLeftY, int botRightX, int botRightY, TileType type) {
    for(int y = topLeftY; y >= botRightY; y--) {
      for(int x = topLeftX; x <= botRightX; x++) {
        tilemap.SetTile(new Vector3Int(x, y, 0), TileDictionary.GenerateTile(type));
      }
    }
  }
}
