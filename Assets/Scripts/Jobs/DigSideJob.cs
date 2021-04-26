using UnityEngine;

public class DigSideJob : DwarfJob {
  public const float timeToDig = 2.0f;
  private float timeUntilNextDig;
  private BaseDwarf dwarf;
  private JobType type = JobType.DIG_ACROSS;

  public override float SobrietyScale { get { return 1.0f; } }

    public override bool JobAction(DwarfSurroundings surroundings)
    {
        if(surroundings.hasTileInFront && surroundings.cellDistanceToTileInFront <= Constants.horizontalInteractionDistance)
        {
            if(GameController.TilemapController.GetTypeOfTile(surroundings.cellInFront) == TileType.STONE)
            {
                dwarf.StopJob();
                return false;
            }

            dwarf.animator.Mine();

            dwarf.audioPlayer.PlaySound("pickHit", dwarf.CurrentCell);

            timeUntilNextDig -= Time.deltaTime;
            if(timeUntilNextDig <= 0.0f)
            {
                if (GameController.Score < Constants.digCost)
                {
                    dwarf.StopJob();
                    return true;
                }

                GameController.TilemapController.RemoveTile(surroundings.cellInFront);
                GameController.AddToScore(-Constants.digCost);

                dwarf.ResetSpeed();
                dwarf.animator.Walk();

                timeUntilNextDig = timeToDig;
            }

          return false;
        }

        return true;
    }

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        timeUntilNextDig = timeToDig;
        dwarf = incDwarf;
        dwarf.JobIcon.SetMiningIcon();
    }

    public override JobType GetJobType()
    {
        return type;
    }
}
