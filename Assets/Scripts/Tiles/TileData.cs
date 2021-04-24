using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject {

  public TileBase[] tiles;

  public bool canDig;

  //TODO Get a random tile from the list
  //This also will function as a random sprite for the data
  public TileBase GenerateTile() {
    return tiles[0];
  }
}
