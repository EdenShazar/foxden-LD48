using UnityEngine;

public enum JobType {
  NONE,
  DIG_DOWN,
  DIG_ACROSS,
  STOP,
  ROPE,
  GET_BOOZE,
  SET_KEG
}

public abstract class DwarfJob : ScriptableObject  {
  public abstract float SobrietyScale { get; }
  public abstract bool JobAction(DwarfSurroundings surroundings);
  public abstract void InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell);
  public virtual void FinalizeJobAction() { }
  public abstract JobType GetJobType();
  public virtual bool CanStopJob() => true;
}
