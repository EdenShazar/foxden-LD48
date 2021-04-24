using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {
    static GameController instance;

    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private BaseDwarf dwarf;
    private TileManager tileManager;
    private TilemapController tilemapController;
    private Dictionary<TileBase, TileData> tileToTileData;
    new private Camera camera;

    // Static getters for easy access to commonly used objects
    public static Tilemap Tilemap { get => instance.tilemap; }
    public static TilemapController TilemapController { get => instance.tilemapController; }

  private void Awake() {
    EnsureSingleton();

    tileManager = gameObject.GetComponent<TileManager>();
    tilemapController = gameObject.GetComponent<TilemapController>();
    camera = Camera.main;

    tileManager.InitializeTileDictionary();
    tilemapController.Initialize();
    tilemapController.GenerateMap();

    Application.targetFrameRate = 60;
  }

  private void Start() {
    
    Instantiate(dwarf);

    tilemapController.UpdateAllTiles();
  }

  void EnsureSingleton()
  {
      if (instance == null)
          instance = this;
      else if (instance != this)
      {
          Debug.LogWarning("GameManager instance already exists on " + instance.gameObject +
              ". Deleting instance from " + gameObject);

          DestroyImmediate(this);
      }
  }
}
