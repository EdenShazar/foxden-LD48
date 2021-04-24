using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {

  [SerializeField]
  private TileData dirt;
  [SerializeField]
  private TileData stone;

  public void InitializeTileDictionary() {
    TileDictionary.SetTile(TileType.DIRT, dirt);
    TileDictionary.SetTile(TileType.STONE, stone);
  }
}
