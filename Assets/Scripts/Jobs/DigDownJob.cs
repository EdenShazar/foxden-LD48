using UnityEngine;

public class DigDownJob : DwarfJob
{
    public const float timeToDig = 2.0f;

    private float timeUntilNextDig;
    private BaseDwarf dwarf;
    private JobType type = JobType.DIG_DOWN;

    public override float SobrietyScale { get{ return 1.0f; } }

    public override bool JobAction(DwarfSurroundings surroundings)
    {
        if (GameController.TilemapController.HasTile(surroundings.cellBelow))
        {
            timeUntilNextDig -= Time.deltaTime;

            dwarf.audioPlayer.PlaySound("dig", dwarf.CurrentCell);

            if (timeUntilNextDig <= 0.0f)
            {
                if (GameController.Score < Constants.digCost)
                {
                    dwarf.StopJob();
                    return true;
                }

                GameController.TilemapController.RemoveTile(surroundings.cellBelow);
                GameController.AddToScore(-Constants.digCost);

                timeUntilNextDig = timeToDig;
            }
        }

        return false;
    }

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        timeUntilNextDig = timeToDig;
        dwarf = incDwarf;
        dwarf.SnapToCurrentCell();
        dwarf.animator.Dig();
        dwarf.JobIcon.SetDiggingIcon();
    }

    public override JobType GetJobType()
    {
        return type;
    }
}
