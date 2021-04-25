using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType {
  NONE,
  DIRT,
  STONE,
  GOLD,
  DWARF
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

  public static TileBase GenerateDefaultTile(TileType type) {
    return GenerateTile(type, TileNeighbors.Neighbors.NONE);
  }

  public static TileBase GenerateTile(TileType type, TileNeighbors.Neighbors neighbors) {
    // Temporarily generate only one type of tile; will later dynamically set the correct one
    return tiles[type].GenerateTile(neighbors);
  }
}
