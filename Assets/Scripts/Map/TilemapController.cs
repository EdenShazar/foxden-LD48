using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    

    [SerializeField] private int leftBoundary = -5;
    [SerializeField] private int rightBoundary = 5;
    [SerializeField] private int topBoundary = 2;
    [SerializeField] private int bottomBoundary = -50;

    Tilemap tilemap;

    TileType[,] tileTypes;
    Dictionary<Vector3Int, int> cellsOccupiedWithDwarvesCount = new Dictionary<Vector3Int, int>();

    public event Action<int, int, TileType> TileDestroyed;

    public int LeftBoundaryCell { get => leftBoundary; }
    public int RightBoundaryCell { get => rightBoundary; }
    public int TopBoundaryCell { get => topBoundary; }
    public int BottomBoundaryCell { get => bottomBoundary; }
    public int WidthCell { get => rightBoundary - leftBoundary + 1; }
    public int HeightCell { get => topBoundary - bottomBoundary + 1; }

    public float LeftBoundaryWorld { get => tilemap.transform.position.x + tilemap.layoutGrid.cellSize.x * leftBoundary; }
    public float RightBoundaryWorld { get => tilemap.transform.position.x + tilemap.layoutGrid.cellSize.x * rightBoundary; }
    public float TopBoundaryWorld { get => tilemap.transform.position.y + tilemap.layoutGrid.cellSize.y * topBoundary; }
    public float BottomBoundaryWorld { get => tilemap.transform.position.y + tilemap.layoutGrid.cellSize.y * bottomBoundary; }
    public float WidthWorld { get => tilemap.layoutGrid.cellSize.x * WidthCell; }
    public float HeightWorld { get => tilemap.layoutGrid.cellSize.y * HeightCell; }

    public void Initialize()
    {
        tilemap = GameController.Tilemap;

        tileTypes = new TileType[WidthCell, HeightCell];
        for (int x = 0; x < WidthCell; x++)
            for (int y = 0; y < HeightCell; y++)
                tileTypes[x, y] = TileType.NONE;
    }

    public void InitializeTile(int x, int y, TileType type)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), TileDictionary.GenerateTile(type, TileNeighbors.Neighbors.NONE));
        tileTypes[x - leftBoundary, y - bottomBoundary] = type;
    }

    public void SetTile(int x, int y, TileType type)
    {
        InitializeTile(x, y, type);
        UpdateTile(x, y);
    }

    public void OccupyCellWithDwarf(Vector3Int cell)
    {
        cell.z = 0;

        if (!cellsOccupiedWithDwarvesCount.ContainsKey(cell))
        {
            cellsOccupiedWithDwarvesCount[cell] = 0;
            return;
        }

        cellsOccupiedWithDwarvesCount[cell]++;
    }

    public void UnoccupyCellWithDwarf(Vector3Int cell)
    {
        cell.z = 0;

        if (!cellsOccupiedWithDwarvesCount.ContainsKey(cell))
            return;

        cellsOccupiedWithDwarvesCount[cell]--;

        if (cellsOccupiedWithDwarvesCount[cell] <= 0)
            cellsOccupiedWithDwarvesCount.Remove(cell);
    }

    public bool IsCellOccupiedWithDwarf(Vector3Int cell)
    {
        return cellsOccupiedWithDwarvesCount.ContainsKey(cell);
    }

    public bool HasTile(Vector3Int cell)
    {
        cell.z = 0;
        return tilemap.HasTile(cell);
    }

    public bool HasTileOrDwarf(Vector3Int cell)
    {
        cell.z = 0;
        return tilemap.HasTile(cell) || IsCellOccupiedWithDwarf(cell);
    }

    public TileType GetTypeOfTile(int x, int y) {
      if(x < leftBoundary || x > rightBoundary || y < bottomBoundary || y > topBoundary) {
        Debug.LogError($"Looked up {x},{y} which is outside the map");
        return TileType.NONE;
      }
      return tileTypes[x - leftBoundary, y - bottomBoundary];
    }

    public TileType GetTypeOfTile(Vector3Int location) {
      return GetTypeOfTile(location.x, location.y);
    }

    public bool IsCellOutOfBounds(Vector3Int cell)
    {
        BoundsInt bounds = tilemap.cellBounds;

        if (cell.x < bounds.xMin || cell.x > bounds.xMax || cell.y < bounds.yMin || cell.y > bounds.yMax)
            return true;

        return false;
    }

    public void RemoveTile(int x, int y)
    {
        if (x < leftBoundary || x > rightBoundary || y < bottomBoundary || y > topBoundary)
            return;

        int score;
        switch (GetTypeOfTile(x, y)) {
        case TileType.BOOZE:
          score = 1;
          break;
        default:
          score = 0;
          break;
        }
        GameController.AddToScore(score);
        tilemap.SetTile(new Vector3Int(x, y, 0), null);
        UpdateAdjacentTiles(x, y);

        TileDestroyed?.Invoke(x, y, tileTypes[x - leftBoundary, y - bottomBoundary]);

        tileTypes[x - leftBoundary, y - bottomBoundary] = TileType.NONE;
    }

    public void RemoveTile(Vector3Int tile)
    {
        RemoveTile(tile.x, tile.y);
    }

    public void UpdateAllTiles()
    {
        for (int x = leftBoundary; x <= rightBoundary; x++)
            for (int y = bottomBoundary; y <= topBoundary; y++)
                UpdateTile(x, y);
    }

    public void UpdateAdjacentTiles(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                UpdateTile(x + i, y + j);
            }
    }

    public void UpdateTile(int x, int y)
    {
        TileNeighbors.Neighbors neighbors = TileNeighbors.Neighbors.NONE;

        bool hasLeftTile = tilemap.HasTile(new Vector3Int(x - 1, y, 0));
        bool hasRightTile = tilemap.HasTile(new Vector3Int(x + 1, y, 0));
        bool hasTopTile = tilemap.HasTile(new Vector3Int(x, y + 1, 0));
        bool hasBottomTile = tilemap.HasTile(new Vector3Int(x, y - 1, 0));

        // All the following conditions are horribly inefficient, but I'm keeping it simple and readable

        // Update neighbors on cardinal directions
        if (hasLeftTile)
            neighbors |= TileNeighbors.Neighbors.A;
        if (hasRightTile)
            neighbors |= TileNeighbors.Neighbors.D;
        if (hasTopTile)
            neighbors |= TileNeighbors.Neighbors.W;
        if (hasBottomTile)
            neighbors |= TileNeighbors.Neighbors.X;

        // Update neighbors on diagonals
        if (hasLeftTile && hasBottomTile)
        {
            bool hasBottomLeftTile = tilemap.HasTile(new Vector3Int(x - 1, y - 1, 0));
            if (hasBottomLeftTile)
                neighbors |= TileNeighbors.Neighbors.Z;
        }
        if (hasBottomTile && hasRightTile)
        {
            bool hasBottomRightTile = tilemap.HasTile(new Vector3Int(x + 1, y - 1, 0));
            if (hasBottomRightTile)
                neighbors |= TileNeighbors.Neighbors.C;
        }
        if (hasRightTile && hasTopTile)
        {
            bool hasTopRightTile = tilemap.HasTile(new Vector3Int(x + 1, y + 1, 0));
            if (hasTopRightTile)
                neighbors |= TileNeighbors.Neighbors.E;
        }
        if (hasTopTile && hasLeftTile)
        {
            bool hasTopLeftTile = tilemap.HasTile(new Vector3Int(x - 1, y + 1, 0));
            if (hasTopLeftTile)
                neighbors |= TileNeighbors.Neighbors.Q;
        }

        GameController.Tilemap.SetTile(new Vector3Int(x, y, 0), TileDictionary.GenerateTile(tileTypes[x - leftBoundary, y - bottomBoundary], neighbors));
    }

    public void GenerateMap()
    {
        tilemap.ClearAllTiles();

        // Left and right boundary
        GenerateBox(leftBoundary, topBoundary, leftBoundary, bottomBoundary, TileType.STONE);
        GenerateBox(rightBoundary, topBoundary - 2, rightBoundary, bottomBoundary, TileType.STONE);

        // Fill the center
        GenerateBox(leftBoundary + 1, topBoundary - 5, rightBoundary - 1, bottomBoundary, TileType.DIRT, TileType.BOOZE);

        // Under wagon
        GenerateBox(rightBoundary - 8, topBoundary - 5, rightBoundary, topBoundary - 5, TileType.STONE);
        GenerateBox(rightBoundary - 7, topBoundary - 6, rightBoundary, topBoundary - 6, TileType.STONE);
        GenerateBox(rightBoundary - 5, topBoundary - 7, rightBoundary, topBoundary - 7, TileType.STONE);
        GenerateBox(rightBoundary - 2, topBoundary - 8, rightBoundary, topBoundary - 8, TileType.STONE);
        GenerateBox(rightBoundary - 1, topBoundary - 9, rightBoundary, topBoundary - 9, TileType.STONE);

        // Left pile
        GenerateBox(leftBoundary + 1, topBoundary - 1, leftBoundary + 1, topBoundary - 7, TileType.STONE);
        GenerateBox(leftBoundary + 2, topBoundary - 3, leftBoundary + 2, topBoundary - 6, TileType.STONE);
        GenerateBox(leftBoundary + 3, topBoundary - 5, leftBoundary + 3, topBoundary - 5, TileType.STONE);

        // Right pile
        GenerateBox(rightBoundary - 1, topBoundary - 1, rightBoundary - 1, topBoundary - 4, TileType.STONE);
        GenerateBox(rightBoundary - 2, topBoundary - 3, rightBoundary - 2, topBoundary - 4, TileType.STONE);
        GenerateBox(rightBoundary - 3, topBoundary - 4, rightBoundary - 3, topBoundary - 4, TileType.STONE);
    }

    public void GenerateBox(int topLeftX, int topLeftY, int botRightX, int botRightY, TileType type)
    {
        for (int y = topLeftY; y >= botRightY; y--)
            for (int x = topLeftX; x <= botRightX; x++)
                GameController.TilemapController.InitializeTile(x, y, type);
    }

    public void GenerateBox(int topLeftX, int topLeftY, int botRightX, int botRightY, TileType common, TileType rare)
    {
        for (int y = topLeftY; y >= botRightY; y--)
            for (int x = topLeftX; x <= botRightX; x++)
                GameController.TilemapController.InitializeTile(x, y, GetTileType(y, common, rare));
    }

    //Tbh not a lot of thought went into these numbers
    public TileType GetTileType(int depth, TileType common, TileType rare) {
      float rareChance = depth / 10 * 10;
      if(rareChance < 5) {
        rareChance = 5;
      }
      float roll = UnityEngine.Random.Range(0.0f, 100.0f);
      if(roll > rareChance) {
        return common;
      } else {
        return rare;
    }
    }
}
