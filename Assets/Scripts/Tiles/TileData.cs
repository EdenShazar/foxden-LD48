using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject {

  public TileBase[] tiles;

  public bool canDig;

  /// <summary>Generate a random tile.</summary>
  public TileBase GenerateTile() {
    return tiles[UnityEngine.Random.Range(0, tiles.Length - 1)];
  }

  /// <summary>Generate a tile according to its specified <paramref name="neighbors"/>.</summary>
  public TileBase GenerateTile(TileNeighbors.Neighbors neighbors) {
    return tiles[TileNeighbors.GetTileIndex(neighbors)];
  }
}
