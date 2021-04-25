using System;
using UnityEngine;

public enum ClimbableRopeTile { UNCMLIMBABLE, LEFT, RIGHT }

[Serializable]
public class RopeManager : MonoBehaviour
{
    enum RopeTile { NONE, LEFT_ANCHOR, RIGHT_ANCHOR, LEFT_MIDDLE, RIGHT_MIDDLE, LEFT_END, RIGHT_END }

    [SerializeField] GameObject anchorPrefab;
    [SerializeField] GameObject middlePrefab;
    [SerializeField] GameObject endPrefab;

    RopeTile[,] ropeTiles;
    GameObject[,] ropeObjects;

    public void Initialize()
    {
        int mapWidth = GameController.TilemapController.WidthCell;
        int mapHeight = GameController.TilemapController.HeightCell;

        ropeTiles = new RopeTile[mapWidth, mapHeight];
        ropeObjects = new GameObject[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
            {
                ropeTiles[x, y] = RopeTile.NONE;
                ropeObjects[x, y] = null;
            }
    }

    /// <summary>Lay down rope or extend it if possible, and returns whether or not succeeded.</summary>
    public bool TryLayRopeTile(Vector3Int anchorCell, Direction lookingDirection)
    {
        Vector3Int firstRopeCell = anchorCell + Vector3Int.down + Vector3Int.right * (int)lookingDirection;

        if (!FindNextRopeCell(firstRopeCell, out Vector3Int cellToAddRope))
        {
            if (lookingDirection == Direction.LEFT)
                SetRopeTile(cellToAddRope, RopeTile.RIGHT_END);
            else
                SetRopeTile(cellToAddRope, RopeTile.LEFT_END);

            return false;
        }

        if (lookingDirection == Direction.LEFT)
            SetRopeTile(cellToAddRope, RopeTile.RIGHT_MIDDLE);
        else
            SetRopeTile(cellToAddRope, RopeTile.LEFT_MIDDLE);

        return true;
    }

    public void PlaceAnchor(Vector3Int anchorCell, Direction lookingDirection)
    {
        if (lookingDirection == Direction.LEFT)
            SetRopeTile(anchorCell, RopeTile.LEFT_ANCHOR);
        else
            SetRopeTile(anchorCell, RopeTile.RIGHT_ANCHOR);
    }

    public ClimbableRopeTile IsCellClimbable(Vector3Int cell)
    {
        RopeTile ropeTile = ropeTiles[cell.x, cell.y];

        if (ropeTile == RopeTile.LEFT_MIDDLE || ropeTile == RopeTile.LEFT_END)
            return ClimbableRopeTile.LEFT;
        if (ropeTile == RopeTile.RIGHT_MIDDLE || ropeTile == RopeTile.RIGHT_END)
            return ClimbableRopeTile.RIGHT;

        return ClimbableRopeTile.UNCMLIMBABLE;
    }

    void SetRopeTile(Vector3Int cell, RopeTile ropeTile)
    {
        ropeTiles[cell.x, cell.y] = ropeTile;

        // Clear rope that might already be there
        if (ropeObjects[cell.x, cell.y] != null)
        {
            Destroy(ropeObjects[cell.x, cell.y]);
            ropeObjects[cell.x, cell.y] = null;
        }

        Vector3 pivotPosition = GameController.Tilemap.layoutGrid.CellToWorld(cell) + Vector3.right * 0.5f; ;
        switch (ropeTile)
        {
            case RopeTile.LEFT_ANCHOR:
                ropeObjects[cell.x, cell.y] = Instantiate(anchorPrefab, pivotPosition, Quaternion.identity, transform);
                ropeObjects[cell.x, cell.y].GetComponent<SpriteRenderer>().flipX = true;
                break;
            case RopeTile.RIGHT_ANCHOR:
                ropeObjects[cell.x, cell.y] = Instantiate(anchorPrefab, pivotPosition, Quaternion.identity, transform);
                break;
            case RopeTile.LEFT_MIDDLE:
                ropeObjects[cell.x, cell.y] = Instantiate(middlePrefab, pivotPosition, Quaternion.identity, transform);
                break;
            case RopeTile.RIGHT_MIDDLE:
                ropeObjects[cell.x, cell.y] = Instantiate(middlePrefab, pivotPosition, Quaternion.identity, transform);
                ropeObjects[cell.x, cell.y].GetComponent<SpriteRenderer>().flipX = true;
                break;
            case RopeTile.LEFT_END:
                ropeObjects[cell.x, cell.y] = Instantiate(endPrefab, pivotPosition, Quaternion.identity, transform);
                break;
            case RopeTile.RIGHT_END:
                ropeObjects[cell.x, cell.y] = Instantiate(endPrefab, pivotPosition, Quaternion.identity, transform);
                ropeObjects[cell.x, cell.y].GetComponent<SpriteRenderer>().flipX = true;
                break;
        }
    }

    /// <summary>Return whether or not a next valid rope cell exists. If true, <paramref name="nextRopeCell"/> is that cell, if false, it's the last roped cell.</summary>
    bool FindNextRopeCell(Vector3Int initialCell, out Vector3Int nextRopeCell)
    {
        Vector3Int cell = initialCell;

        while (true)
        {
            // Hit ground
            if (GameController.Tilemap.HasTile(cell) || GameController.TilemapController.IsCellOutOfBounds(cell))
            {
                nextRopeCell = cell + Vector3Int.up;
                return false;
            }

            if (HasRopeSection(cell))
                cell.y -= 1;
            else
            {
                nextRopeCell = cell;
                return true;
            }
        }
    }

    bool HasRopeSection(Vector3Int cell)
    {
        RopeTile ropeTile = ropeTiles[cell.x, cell.y];

        if (ropeTile != RopeTile.NONE && ropeTile != RopeTile.LEFT_ANCHOR && ropeTile != RopeTile.RIGHT_ANCHOR)
            return true;

        return false;
    }
}
