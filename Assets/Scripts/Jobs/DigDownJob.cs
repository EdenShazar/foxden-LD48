using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigDownJob : DwarfJob {
  public const float timeToDig = 2.0f;
  private float timeUntilNextDig;
  private BaseDwarf dwarf;
  private JobType type = JobType.DIG_DOWN;

  public override bool JobAction(DwarfSurroundings surroundings) {
    if(GameController.Tilemap.HasTile(surroundings.cellBelow)) {
      timeUntilNextDig -= Time.deltaTime;
      if(timeUntilNextDig <= 0.0f) {
        GameController.TilemapController.RemoveTile(surroundings.cellBelow);
        timeUntilNextDig = timeToDig;
      }
    }
    return false;
  }

  public override JobType InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell) {
    timeUntilNextDig = timeToDig;
    dwarf = incDwarf;
    dwarf.transform.position = GameController.Tilemap.layoutGrid.CellToWorld(currentCell) + new Vector3(0.5f, 0.5f, 0.0f);
    dwarf.animator.Dig();
    return type;
  }

  public override JobType GetJobType() {
    return type;
  }
}
