using UnityEngine;

public enum JobType {
  NONE,
  DIG_DOWN,
  DIG_ACROSS,
  STOP,
  ROPE,
  BOOZE
}

public abstract class DwarfJob : ScriptableObject  {
  public abstract bool JobAction(DwarfSurroundings surroundings);
  public abstract JobType InitializeJobAction(BaseDwarf incDwarf, Vector3Int currentCell);
  public abstract JobType GetJobType();
}
