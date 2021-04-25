using System.Collections;
using UnityEngine;

public class GetBoozeJob : DwarfJob
{
    const float pullUpRopeAnimationTime = 1f;
    const float climbSpeed = 1f;

    BaseDwarf dwarf;
    JobType type = JobType.GET_BOOZE;

    bool isClimbing = false;
    bool isPullingUpRope = false;

    public override bool JobAction(DwarfSurroundings surroundings)
    {
        bool revertToDefaultAction;

        // Start climbing when possible
        if (!isClimbing && !isPullingUpRope)
        {
            revertToDefaultAction = CheckForNewClimb();

            if (revertToDefaultAction)
                return true;
        }

        if (isClimbing)
        {
            ContinueClimbing();
            return false;
        }

        return !isPullingUpRope;
    }

    public override JobType InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        dwarf = incDwarf;
        dwarf.JobIcon.SetGetBoozeIcon();
        return type;
    }

    public override JobType GetJobType()
    {
        return type;
    }

    public override bool CanStopJob()
    {
        return !isClimbing && !isPullingUpRope;
    }

    /// <summary>Start a new climb if possible, return whether or not to revert to default action.</summary>
    bool CheckForNewClimb()
    {
        Vector3Int currentCell = dwarf.CurrentCell;
        ClimbableRopeTile ropeTile = GameController.RopeManager.IsCellClimbable(currentCell);

        if (ropeTile == ClimbableRopeTile.UNCMLIMBABLE)
            return true;

        if (dwarf.MoveDirection == Direction.LEFT)
        {
            if (ropeTile == ClimbableRopeTile.RIGHT)
                dwarf.FlipDirection();

            StartClimbing();
        }
        else
        {
            if (ropeTile == ClimbableRopeTile.LEFT)
                dwarf.FlipDirection();

            StartClimbing();
        }

        return false;
    }

    void StartClimbing()
    {
        isClimbing = true;
        dwarf.Rigidbody.gravityScale = 0f;
        dwarf.animator.ClimbRope();
    }

    void ContinueClimbing()
    {
        // If current cell has rope on wrong side, flip dwarf
        Vector3Int currentCell = dwarf.CurrentCell;
        ClimbableRopeTile ropeTile = GameController.RopeManager.IsCellClimbable(currentCell);
        if ((ropeTile == ClimbableRopeTile.LEFT && dwarf.MoveDirection == Direction.RIGHT)
            || (ropeTile == ClimbableRopeTile.RIGHT && dwarf.MoveDirection == Direction.LEFT))
            dwarf.FlipDirection();


        if (CellAboveIsClimbable())
        {
            // Keep climbing
            dwarf.transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
            return;
        }
        
        GameController.Instance.StartCoroutine(EndClimb());
    }

    bool CellAboveIsClimbable()
    {
        Debug.Log(GameController.RopeManager.IsCellClimbable(dwarf.CurrentCell + Vector3Int.up) != ClimbableRopeTile.UNCMLIMBABLE);
        return GameController.RopeManager.IsCellClimbable(dwarf.CurrentCell + Vector3Int.up) != ClimbableRopeTile.UNCMLIMBABLE;
    }

    IEnumerator EndClimb()
    {
        // Ensure coroutine isn't running more than once
        if (isPullingUpRope)
            yield break;

        isClimbing = false;
        isPullingUpRope = true;

        dwarf.SnapToCurrentCell();
        dwarf.animator.PullUpRope();

        yield return new WaitForSeconds(pullUpRopeAnimationTime);

        isPullingUpRope = false;

        dwarf.SnapToRelativeCell(Vector3Int.up + Vector3Int.right * (int)dwarf.MoveDirection);
        dwarf.Rigidbody.gravityScale = 1f;
        dwarf.StopJob();
    }
}
