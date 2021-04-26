using System.Collections;
using UnityEngine;

public class GetBoozeJob : DwarfJob {
  const float pullUpRopeAnimationTime = 1f;
  const float drinkAnimationTime = 4f;
  const float climbSpeed = 1f;

  BaseDwarf dwarf;
  JobType type = JobType.GET_BOOZE;

  bool isClimbing = false;
  bool isPullingUpRope = false;
  public bool IsDrinking { get; private set; } = false;

  private float sobrietyScale = 0.1f;

  public override float SobrietyScale {get {return sobrietyScale;} }

    public override bool JobAction(DwarfSurroundings surroundings)
    {
        bool revertToDefaultAction;

        if (IsDrinking)
            return false;

        if (dwarf.isAtWagon || (dwarf.isAtKeg && dwarf.currentKeg.TryUseKeg()))
        {
            GameController.Instance.StartCoroutine(Drink());
            return false;
        }

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

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        sobrietyScale = 0.1f;
        dwarf = incDwarf;
        dwarf.ResetSpeed();
        dwarf.JobIcon.SetGetBoozeIcon();
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
        if (!GameController.RopeManager.IsCellClimbable(dwarf.CurrentCell, out Direction ropeDirection))
            return true;

        if (ropeDirection != dwarf.MoveDirection)
            dwarf.FlipDirection();

        StartClimbing();

        return false;
    }

    void StartClimbing()
    {
        isClimbing = true;
        dwarf.Rigidbody.gravityScale = 0f;
        dwarf.SnapToCurrentCell();
        dwarf.animator.ClimbRope();
    }

    void ContinueClimbing()
    {
        GameController.RopeManager.IsCellClimbable(dwarf.CurrentCell, out Direction ropeDirection, preferredDirection: dwarf.MoveDirection);
        
        if (ropeDirection != dwarf.MoveDirection)
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
        return GameController.RopeManager.IsCellClimbable(dwarf.CurrentCell + Vector3Int.up, out _);
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
        dwarf.animator.Walk();
        dwarf.ResetSpeed();
    }

    IEnumerator Drink()
    {
        // Ensure coroutine isn't running more than once
        if (IsDrinking)
            yield break;

        IsDrinking = true;
        dwarf.animator.Drink();

        dwarf.audioPlayer.PlaySound("drink", dwarf.CurrentCell);

        yield return new WaitForSeconds(drinkAnimationTime);

        if (!GameController.DwarfManager.OnBreak)
        {
            dwarf.ResetDrunk();
            dwarf.isAtWagon = false;
            dwarf.isAtKeg = false;
            dwarf.StopJob();
            
            if (!dwarf.isAtKeg)
                dwarf.FlipDirection();
        }
        
        // Final break
        sobrietyScale = 0f;
        dwarf.ResetDrunk();
    }
}
