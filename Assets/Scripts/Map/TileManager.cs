using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {

    [SerializeField]
    private TileData none;
    [SerializeField]
    private TileData dirt;
    [SerializeField]
    private TileData stone;
    [SerializeField]
    private TileData booze;
    [SerializeField]
    private TileData dwarf;

    public void InitializeTileDictionary() {
        TileDictionary.SetTile(TileType.NONE, none);
        TileDictionary.SetTile(TileType.DIRT, dirt);
        TileDictionary.SetTile(TileType.STONE, stone);
        TileDictionary.SetTile(TileType.BOOZE, booze);
        TileDictionary.SetTile(TileType.DWARF, dwarf);
    }
}
