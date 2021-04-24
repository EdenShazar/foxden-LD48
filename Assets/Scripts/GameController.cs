using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {
  [SerializeField]
  private Tilemap tilemap;
  [SerializeField]
  private BaseDwarf dwarf;
  private TileManager tileManager;
  private MapGenerator mapGenerator;
  private Dictionary<TileBase, TileData> tileToTileData;

  private void Start() {
    tileManager = gameObject.GetComponent<TileManager>();
    mapGenerator = gameObject.GetComponent<MapGenerator>();
    tileManager.InitializeTileDictionary();
    mapGenerator.setTilemap(tilemap);
    mapGenerator.GenerateMap();
    Instantiate(dwarf, Vector3.up, Quaternion.identity);
  }
}
