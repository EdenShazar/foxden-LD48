using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    Tilemap tilemap;

    int leftBoundary;
    int rightBoundary;
    int topBoundary;
    int bottomBoundary;

    TileType[,] tileTypes;

    public void Initialize()
    {
        tilemap = GameController.Tilemap;

        leftBoundary = GameController.MapGenerator.LeftBoundary;
        rightBoundary = GameController.MapGenerator.RightBoundary;
        topBoundary = GameController.MapGenerator.TopBoundary;
        bottomBoundary = GameController.MapGenerator.BottomBoundary;

        int horizontalTileCount = rightBoundary - leftBoundary + 1;
        int verticalTileCount = topBoundary - bottomBoundary + 1;

        tileTypes = new TileType[horizontalTileCount, verticalTileCount];
        for (int x = 0; x < horizontalTileCount; x++)
            for (int y = 0; y < verticalTileCount; y++)
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

    public void UpdateAllTiles()
    {
        for (int x = leftBoundary; x <= rightBoundary; x++)
            for (int y = bottomBoundary; y <= topBoundary; y++)
                UpdateTile(x, y);
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
}
