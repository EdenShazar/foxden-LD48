using UnityEngine;

public class RopeJob : DwarfJob
{
    const float timeToLayRope = 1f;
    private float timeUntilNextRope;
    BaseDwarf dwarf;
    JobType type = JobType.ROPE;

    bool hasPlacedAnchor = false;

    Rope rope;

  public override float SobrietyScale { get { return 1.0f; } }

  public override bool JobAction(DwarfSurroundings surroundings)
    {
        if (!surroundings.hasTileInFront && !surroundings.hasTileBelowInFront && surroundings.hasTileBelow)
        {
            // Valid rope laying conditions
            dwarf.SnapToCurrentCell();
            dwarf.animator.Hammer();

            dwarf.audioPlayer.PlaySound("hammerRope", dwarf.CurrentCell);

            timeUntilNextRope -= Time.deltaTime;
            if (timeUntilNextRope <= 0.0f)
            {
                if (!hasPlacedAnchor)
                {
                    if (GameController.Score < Constants.ropeCost)
                    {
                        dwarf.StopJob();
                        return true;
                    }

                    GameController.RopeManager.TryAnchorNewRope(dwarf.CurrentCell, dwarf.MoveDirection, out rope);
                    GameController.AddToScore(-Constants.ropeCost);

                    hasPlacedAnchor = true;
                }
                else
                {
                    if (!GameController.RopeManager.TryExtend(rope))
                    {
                        dwarf.StopJob();
                        return true;
                    }

                }

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
