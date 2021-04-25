using System;
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
        GenerateBox(rightBoundary, topBoundary, rightBoundary, bottomBoundary, TileType.STONE);

        // Fill the center
        GenerateBox(leftBoundary + 1, topBoundary - 2, rightBoundary - 1, bottomBoundary, TileType.DIRT, TileType.BOOZE);
    }

    public void GenerateBox(int topLeftX, int topLeftY, int botRightX, int botRightY, TileType type)
    {
        for (int y = topLeftY; y >= botRightY; y--)
            for (int x = topLeftX; x <= botRightX; x++)
                GameController.TilemapController.InitializeTile(x, y, type);
    }

    public void GenerateBox(int topLeftX, int topLeftY, int botRightX, int botRightY, params TileType[] types)
    {
        for (int y = topLeftY; y >= botRightY; y--)
            for (int x = topLeftX; x <= botRightX; x++)
                GameController.TilemapController.InitializeTile(x, y, types[UnityEngine.Random.Range(0, types.Length)]);
    }
}
