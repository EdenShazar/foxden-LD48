using UnityEngine;

public abstract class DwarfJob : ScriptableObject  {
  public abstract bool JobAction(DwarfSurroundings surroundings);
  public abstract void InitializeJobAction(BaseDwarf dwarf, Vector3Int currentCell);
}
