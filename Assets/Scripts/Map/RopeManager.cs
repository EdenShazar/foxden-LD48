using System.Collections.Generic;
using UnityEngine;

public enum ClimbableRopeTile { UNCMLIMBABLE, LEFT, RIGHT }

public class RopeManager : MonoBehaviour
{
    [SerializeField] GameObject anchorPrefab;
    [SerializeField] GameObject middlePrefab;
    [SerializeField] GameObject endPrefab;

    List<Rope> ropes = new List<Rope>();

    public void Initialize()
    {
        Rope.InitializePrefabReferences(anchorPrefab, middlePrefab, endPrefab);

        GameController.TilemapController.TileDestroyed += RemoveRope;
    }

    public bool HasRopeOnDirection(Vector3Int anchorCell, Direction anchorDirection, out Rope rope)
    {
        foreach (Rope currentRope in ropes)
            if (currentRope.HasAnchorOnDirection(anchorCell, anchorDirection))
            {
                rope = currentRope;
                return true;
            }

        rope = null;
        return false;
    }

    public bool HasRopeOnCell(Vector3Int anchorCell, out Rope leftRope, out Rope rightRope)
    {
        leftRope = null;
        rightRope = null;
        bool hasRope = false;

        foreach (Rope currentRope in ropes)
        {
            if (currentRope.HasAnchorOnDirection(anchorCell, Direction.LEFT))
            {
                leftRope = currentRope;
                hasRope = true;
            }
            else if (currentRope.HasAnchorOnDirection(anchorCell, Direction.RIGHT))
            {
                rightRope = currentRope;
                hasRope = true;
            }
        }

        return hasRope;
    }

    public bool HasAnchorOnDirection(Vector3Int cell, Direction direction)
    {
        foreach (Rope rope in ropes)
            if (rope.HasAnchorOnDirection(cell, direction))
                return true;

        return false;
    }

    public bool HasAnchorOnCell(Vector3Int cell)
    {
        foreach (Rope rope in ropes)
            if (rope.HasAnchorOnCell(cell))
                return true;

        return false;
    }

    public bool HasRopeOnDirection(Vector3Int cell, Direction direction)
    {
        foreach (Rope rope in ropes)
            if (rope.IsCellClimbableOnDirection(cell, direction))
                return true;

        return false;
    }

    public bool TryAnchorNewRope(Vector3Int anchorCell, Direction anchorDirection, out Rope rope)
    {
        if (HasRopeOnDirection(anchorCell, anchorDirection, out Rope existingRope))
        {
            rope = existingRope;
            return false;
        }

        rope = new Rope(anchorCell, anchorDirection);
        ropes.Add(rope);

        return true;
    }

    public bool TryExtend(Rope rope)
    {
        return rope.TryExtend();
    }


    public bool IsCellClimbable(Vector3Int cell, out Direction direction)
    {
        //int x = cell.x - GameController.TilemapController.LeftBoundaryCell;
        //int y = cell.y - GameController.TilemapController.BottomBoundaryCell;

        // Default value
        direction = Direction.RIGHT;

        foreach (Rope rope in ropes)
        {
            if (rope.IsCellClimbableOnDirection(cell, Direction.LEFT))
            {
                direction = Direction.LEFT;
                return true;
            }

            if (rope.IsCellClimbableOnDirection(cell, Direction.RIGHT))
                return true;
        }

        return false;
    }

    void RemoveRope(Vector3Int anchorCell, TileType _)
    {
        if (HasRopeOnCell(anchorCell, out Rope leftRope, out Rope rightRope))
        {
            if (leftRope != null)
            {
                leftRope.RemoveRope();
                ropes.Remove(leftRope);
            }

            if (rightRope != null)
            {
                rightRope.RemoveRope();
                ropes.Remove(rightRope);
            }
        }
    }
}
