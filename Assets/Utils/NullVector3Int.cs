//Vector3Int is a non nullable type, but I want to make it null
//Making a Vector3Int with 
using UnityEngine;

public static class NullVector3Int {
  private static Vector3Int nullVector3Int = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

  public static Vector3Int GetNullVector3Int() {
    return nullVector3Int;
  }

  public static bool IsVector3IntNull(Vector3Int vector3) {
    return vector3.x == int.MaxValue && vector3.y == int.MaxValue && vector3.z == int.MaxValue;
  }
}
