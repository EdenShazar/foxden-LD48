using UnityEngine;

public class RopeJob : DwarfJob
{
    const float timeToLayRope = 1f;
    private float timeUntilNextRope;
    BaseDwarf dwarf;
    JobType type = JobType.ROPE;

    bool hasPlacedAnchor = false;

  public override float sobrietyScale { get { return 0.0f; } }
  public override int jobCost {get {return 1;} }

  public override bool JobAction(DwarfSurroundings surroundings)
    {
        if (!surroundings.hasTileInFront && !surroundings.hasTileBelowInFront && surroundings.hasTileBelow)
        {
            // Valid rope laying conditions
            dwarf.SnapToCurrentCell();
            dwarf.animator.Hammer();

            timeUntilNextRope -= Time.deltaTime;
            if (timeUntilNextRope <= 0.0f)
            {
                if (!hasPlacedAnchor)
                {
                    GameController.RopeManager.PlaceAnchor(dwarf.CurrentCell, dwarf.MoveDirection);
                    hasPlacedAnchor = true;
                }

                if (!GameController.RopeManager.TryLayRopeTile(dwarf.CurrentCell, dwarf.MoveDirection))
                    dwarf.StopJob();

                dwarf.audioPlayer.PlaySound("drink", dwarf.CurrentCell);

                timeUntilNextRope = timeToLayRope;
            }

            return false;
        }

        return true;
    }

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        timeUntilNextRope = timeToLayRope;
        dwarf = incDwarf;
        dwarf.JobIcon.SetRopeIcon();
    }

    public override JobType GetJobType()
    {
        return type;
    }
}
