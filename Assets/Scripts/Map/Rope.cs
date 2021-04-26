using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rope
{
    Direction anchorDirection;
    Vector3Int anchorCell;
    Vector3Int startCell;
    Vector3Int endCell;

    static GameObject anchorPrefab;
    static GameObject middlePrefab;
    static GameObject endPrefab;

    GameObject anchor;
    List<GameObject> middleSections = new List<GameObject>();
    GameObject end;

    bool flipX;

    public Rope(Vector3Int anchorCell, Direction anchorDirection)
    {
        this.anchorDirection = anchorDirection;
        this.anchorCell = anchorCell;

        flipX = anchorDirection == Direction.LEFT;

        startCell = anchorCell + Vector3Int.right * (int)anchorDirection + Vector3Int.down;
        endCell = startCell;

        Vector3 anchorPosition = GameController.Tilemap.GetCellCenterWorld(anchorCell) + Vector3.down * 0.5f;
        anchor = GameObject.Instantiate(anchorPrefab, anchorPosition, Quaternion.identity, GameController.RopeManager.transform);

        Vector3 endPosition = GameController.Tilemap.GetCellCenterWorld(endCell) + Vector3.down * 0.5f;
        end = GameObject.Instantiate(endPrefab, endPosition, Quaternion.identity, GameController.RopeManager.transform);

        if (flipX)
        {
            anchor.GetComponent<SpriteRenderer>().flipX = flipX;
            end.GetComponent<SpriteRenderer>().flipX = flipX;
        }
    }

    public static void InitializePrefabReferences(GameObject anchor, GameObject middle, GameObject end)
    {
        anchorPrefab = anchor;
        middlePrefab = middle;
        endPrefab = end;
    }

    public static bool CanAnchor(Vector3Int cell, Direction direction)
    {
        return !GameController.TilemapController.HasTile(cell) && !GameController.RopeManager.HasAnchorOnDirection(cell, direction);
    }

    public bool HasAnchorOnDirection(Vector3Int cell, Direction direction)
    {
        return anchorDirection == direction && cell == anchorCell;
    }

    public bool HasAnchorOnCell(Vector3Int cell)
    {
        return cell == anchorCell;
    }

    public bool IsCellClimbableOnDirection(Vector3Int cell, Direction direction)
    {
        return anchorDirection != direction && HasRopeOnCell(cell);
    }

    public bool TryExtend()
    {
        Vector3Int newCell = endCell + Vector3Int.down;

        if (GameController.TilemapController.HasTile(newCell))
            return false;

        if (GameController.RopeManager.HasRopeOnDirection(newCell, (Direction)((int)anchorDirection * -1)))
            return false;

        Vector3 position = GameController.Tilemap.GetCellCenterWorld(endCell) + Vector3.down * 0.5f;
        GameObject newMiddleSection = GameObject.Instantiate(middlePrefab, position, Quaternion.identity, GameController.RopeManager.transform);

        if (flipX)
            newMiddleSection.GetComponent<SpriteRenderer>().flipX = flipX;

        middleSections.Add(newMiddleSection);

        endCell += Vector3Int.down;
        end.transform.Translate(Vector3.down);

        return true;
    }

    public void RemoveRope()
    {
        GameObject.Destroy(anchor);
        
        foreach (GameObject middleSection in middleSections)
            GameObject.Destroy(middleSection);

        GameObject.Destroy(end);
    }

    bool HasRopeOnCell(Vector3Int cell)
    {
        return endCell.y <= cell.y && cell.y <= startCell.y;
    }
}
