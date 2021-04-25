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
        dwarf.transform.position = GameController.Tilemap.layoutGrid.CellToWorld(currentCell) + new Vector3(0.5f, 0.5f, 0.0f);

        //add animation for dwarf
        dwarf.animator.StopSign();

        return type;
    }

    public override JobType GetJobType() {
        return type;
    }

}
