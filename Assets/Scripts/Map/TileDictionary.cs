using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType {
  DIRT,
  STONE
}

public static class TileDictionary {
  private static Dictionary<TileType, TileData> tiles = new Dictionary<TileType, TileData>();

  public static void SetTile(TileType type, TileData tile) {
    if(!tiles.ContainsKey(type)) {
      tiles[type] = tile;
    }
  }
  
  public static TileData GetTileData(TileType type) {
    return tiles[type];
  }

  public static TileBase GenerateTile(TileType type) {
    // Temporarily generate only one type of tile; will later dynamically set the correct one
    return tiles[type].GenerateTile(TileNeighbors.Neighbors.X | TileNeighbors.Neighbors.W);
  }
}
