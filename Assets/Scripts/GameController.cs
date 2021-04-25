using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CursorMode { REGULAR, CLICK, STOP }

public class GameController : MonoBehaviour {
    static GameController instance;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private BaseDwarf dwarf;
    [SerializeField] private Texture2D regularCursor;
    [SerializeField] private Texture2D clickCursor;
    [SerializeField] private Texture2D stopCursor;
    private TileManager tileManager;
    private TilemapController tilemapController;
    private RopeManager ropeManager;
    private AudioManager audioManager;
    private Dictionary<TileBase, TileData> tileToTileData;
    new private Camera camera;
    private CursorMode currentCursorMode = CursorMode.REGULAR;

    // Static getters for easy access to commonly used objects
    public static GameController Instance { get => instance; }
    public static Tilemap Tilemap { get => instance.tilemap; }
    public static TilemapController TilemapController { get => instance.tilemapController; }
    public static RopeManager RopeManager { get => instance.ropeManager; }

    public static List<BaseDwarf> workingDwarvesHoveredOver { get; private set; } = new List<BaseDwarf>();

    private void Awake() {
    EnsureSingleton();

    tileManager = GetComponent<TileManager>();
    tilemapController = GetComponent<TilemapController>();
    ropeManager = FindObjectOfType<RopeManager>();
    audioManager = FindObjectOfType<AudioManager>();
    camera = Camera.main;

    tileManager.InitializeTileDictionary();
    tilemapController.Initialize();
    ropeManager.Initialize();
    tilemapController.GenerateMap();

    Application.targetFrameRate = 60;
  }

  private void Start() {
    
    Instantiate(dwarf);

    tilemapController.UpdateAllTiles();
  }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Instantiate(dwarf);

        if (Input.GetMouseButton(0))
        {
            if (currentCursorMode != CursorMode.STOP)
                SetCursor(CursorMode.CLICK);
        }
        else if (workingDwarvesHoveredOver.Count > 0)
            SetCursor(CursorMode.STOP);
        else
            SetCursor(CursorMode.REGULAR);
    }

    public void SetCursor(CursorMode cursorMode)
    {
        currentCursorMode = cursorMode;

        Vector2 hotSpot = new Vector2(6f, 4f);
        switch (cursorMode)
        {
            case CursorMode.REGULAR:
                Cursor.SetCursor(regularCursor, hotSpot, UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.CLICK:
                Cursor.SetCursor(clickCursor, hotSpot, UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.STOP:
                Cursor.SetCursor(stopCursor, hotSpot, UnityEngine.CursorMode.Auto);
                break;
        }
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
    