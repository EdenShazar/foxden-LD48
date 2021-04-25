using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigSideJob : DwarfJob {
  public const float timeToDig = 2.0f;
  private float timeUntilNextDig;
  private BaseDwarf dwarf;
  private JobType type = JobType.DIG_ACROSS;

  public override bool JobAction(DwarfSurroundings surroundings) {
    if(surroundings.hasTileInFront) {
      if(GameController.TilemapController.GetTypeOfTile(surroundings.cellInFront) == TileType.STONE) {
        dwarf.StopJob();
        return false;
      }
      dwarf.animator.Mine();
      timeUntilNextDig -= Time.deltaTime;
      if(timeUntilNextDig <= 0.0f) {
        GameController.TilemapController.RemoveTile(surroundings.cellInFront);
        dwarf.ResetSpeed();
        dwarf.animator.Walk();
        timeUntilNextDig = timeToDig;
      }
      return false;
    }
    return true;
  }

  public override JobType InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell) {
    timeUntilNextDig = timeToDig;
    dwarf = incDwarf;
    dwarf.JobIcon.SetMiningIcon();
    return type;
  }

  public override JobType GetJobType() {
    return type;
  }
}
