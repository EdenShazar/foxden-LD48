using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignJob : DwarfJob
{
    private BaseDwarf dwarf;
    private JobType type = JobType.STOP;
    private Vector3Int cellPlantedFeet;

    public override float SobrietyScale { get { return 0.6f; } }

    public override bool JobAction(DwarfSurroundings surroundings)
    {
        if (GameController.Score >= Constants.stopSignCost) {
            dwarf.SnapToCurrentCell();

            if (dwarf.CurrentCell != cellPlantedFeet) {
                GameController.TilemapController.UnoccupyCellWithDwarf(cellPlantedFeet);
                cellPlantedFeet = dwarf.CurrentCell;
                GameController.TilemapController.OccupyCellWithDwarf(dwarf.CurrentCell);
            }
            else {
                if (!GameController.TilemapController.IsCellOccupiedWithDwarf(dwarf.CurrentCell)) {
                    cellPlantedFeet = dwarf.CurrentCell;
                    GameController.TilemapController.OccupyCellWithDwarf(dwarf.CurrentCell);
                }
            }

            //add animation for dwarf
            dwarf.animator.StopSign();
        }
        else {
            dwarf.StopJob();
        }

        return true;
        
    }

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        dwarf = incDwarf;
        
        if(GameController.Score >= Constants.stopSignCost) {
            cellPlantedFeet = dwarf.CurrentCell;
            dwarf.JobIcon.SetStopSignIcon();
            GameController.AddToScore(-Constants.stopSignCost);
        }
        
    }

    public override void FinalizeJobAction()
    {
        GameController.TilemapController.UnoccupyCellWithDwarf(dwarf.CurrentCell);
    }

    public override JobType GetJobType()
    {
        return type;
    }

}
