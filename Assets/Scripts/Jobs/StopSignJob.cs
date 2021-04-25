using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignJob : DwarfJob
{
    private BaseDwarf dwarf;
    private JobType type = JobType.STOP;

  public override float sobrietyScale { get { return 1.0f; } }

  public override bool JobAction(DwarfSurroundings surroundings) {
        return false;
    }

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell) {
        GameController.TilemapController.InitializeTile(currentCell.x, currentCell.y, TileType.DWARF);
        dwarf = incDwarf;
        dwarf.SnapToCurrentCell();

        //add animation for dwarf
        dwarf.animator.StopSign();
        dwarf.JobIcon.SetStopSignIcon();
    }

  public override JobType GetJobType() {
        return type;
    }

}
