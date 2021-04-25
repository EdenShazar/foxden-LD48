using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignJob : DwarfJob
{
    private BaseDwarf dwarf;
    private JobType type = JobType.STOP;

    public override bool JobAction(DwarfSurroundings surroundings) {
        return false;
    }

    public override JobType InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell) {
        GameController.TilemapController.InitializeTile(currentCell.x, currentCell.y, TileType.DWARF);
        dwarf = incDwarf;
        dwarf.SnapToCurrentCell();

        //add animation for dwarf
        dwarf.animator.StopSign();
        dwarf.JobIcon.SetStopSignIcon();

        return type;
    }

    public override JobType GetJobType() {
        return type;
    }

}
