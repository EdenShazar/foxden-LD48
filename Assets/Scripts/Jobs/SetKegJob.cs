using UnityEngine;

public class SetKegJob : DwarfJob
{
    const float timeToLayRope = 2f;

    float timeElapsedToLayRope = 0f;

    BaseDwarf dwarf;
    JobType type = JobType.SET_KEG;

  public override float SobrietyScale { get { return 0.0f; } }

  public override bool JobAction(DwarfSurroundings surroundings)
    {
        // Can't set keg over another get or wagon
        if (dwarf.isAtWagon || dwarf.isAtKeg)
        {
            dwarf.StopJob();
            return true;
        }

        dwarf.SnapToCurrentCell();
        dwarf.animator.Hammer();

        dwarf.audioPlayer.PlaySound("hammerRope", dwarf.CurrentCell);

        timeElapsedToLayRope += Time.deltaTime;
        if (timeElapsedToLayRope >= timeToLayRope)
        {
            if (GameController.Score < Constants.kegCost)
            {
                dwarf.StopJob();
                return true;
            }

            Vector3 kegPosition = GameController.Tilemap.GetCellCenterWorld(dwarf.CurrentCell) + Vector3.down * 0.5f;
            Instantiate(JobSelector.KegPrefab, kegPosition, Quaternion.identity);
            GameController.AddToScore(-Constants.kegCost);

            dwarf.StopJob();
            return true;
        }

        return false;
    }

    public override void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell)
    {
        dwarf = incDwarf;
        dwarf.JobIcon.SetSetKegSprite();
    }

    public override JobType GetJobType()
    {
        return type;
    }
}
