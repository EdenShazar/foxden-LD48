using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignJob : DwarfJob
{
    private BaseDwarf dwarf;
    private JobType type = JobType.STOP;

    public override float SobrietyScale { get { return 0.6f; } }

    public override bool JobAction(DwarfSurroundings surroundings)
    {
        dwarf.SnapToCurrentCell();

        Debug.Log(dwarf.CurrentCell);

        if (!GameController.TilemapController.IsCellOccupiedWithDwarf(dwarf.CurrentCell)) {
            GameController.TilemapController.OccupyCellWithDwarf(dwarf.CurrentCell);
        }
        
        //add animation for dwarf
        dwarf.animator.StopSign();

        return true;
    }

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        dwarf = incDwarf;
        dwarf.JobIcon.SetStopSignIcon();
        if(GameController.Score >= Constants.stopSignCost) {
            GameController.AddToScore(-Constants.stopSignCost);
        }
        
    }

    public override void FinalizeJobAction()
    {
        Debug.Log(dwarf.CurrentCell);
        GameController.TilemapController.UnoccupyCellWithDwarf(dwarf.CurrentCell);
    }

    public override JobType GetJobType()
    {
        return type;
    }

}
