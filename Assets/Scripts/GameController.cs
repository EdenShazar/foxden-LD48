using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum CursorMode { REGULAR, CLICK, STOP }

public class GameController : MonoBehaviour {
    static GameController instance;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Texture2D regularCursor;
    [SerializeField] private Texture2D clickCursor;
    [SerializeField] private Texture2D stopCursor;
    [SerializeField] private GameObject pauseMenu;
    private TileManager tileManager;
    private TilemapController tilemapController;
    private RopeManager ropeManager;
    private AudioManager audioManager;
    private DwarfSpawner dwarfSpawner;
    private DwarfManager dwarfManager;
    private EndScreenController endScreenController;
    private Dictionary<TileBase, TileData> tileToTileData;
    new private Camera camera;
    private int score;
    private CursorMode currentCursorMode = CursorMode.REGULAR;

    // Static getters for easy access to commonly used objects
    public static GameController Instance { get => instance; }
    public static Tilemap Tilemap { get => instance.tilemap; }
    public static TilemapController TilemapController { get => instance.tilemapController; }
    public static RopeManager RopeManager { get => instance.ropeManager; }
    public static AudioManager AudioManager { get => instance.audioManager; } 
    public static DwarfSpawner DwarfSpawner { get => instance.dwarfSpawner; } 
    public static DwarfManager DwarfManager { get => instance.dwarfManager; } 
    public static EndScreenController EndScreenController { get => instance.endScreenController; } 
    public static int Score { get => instance.score; } 
    
    public static bool GamePaused { get; private set; } 
    public static bool GameEnded { get; private set; } 

    public static List<BaseDwarf> workingDwarvesHoveredOver { get; private set; } = new List<BaseDwarf>();

    private void Awake()
    {
        EnsureSingleton();

        tileManager = GetComponent<TileManager>();
        tilemapController = GetComponent<TilemapController>();
        ropeManager = FindObjectOfType<RopeManager>();
        audioManager = FindObjectOfType<AudioManager>();
        dwarfSpawner = FindObjectOfType<DwarfSpawner>();
        dwarfManager = FindObjectOfType<DwarfManager>();
        endScreenController = FindObjectOfType<EndScreenController>();
        camera = Camera.main;

        tileManager.InitializeTileDictionary();
        tilemapController.Initialize();
        ropeManager.Initialize();
        tilemapController.GenerateMap();

        Application.targetFrameRate = 60;
    }

    void Start()
    {
        score = 200;
        StartCoroutine(SpawnDwarvesDelayed(delay: 3.5f, numberOfDwarves: 10));
    
        tilemapController.UpdateAllTiles();

        endScreenController.Hide();

        dwarfManager.NoDwarvesLeft += GameOverFail;
        dwarfManager.AllDwarvesSafe += GameOverSuccess;
    }

    private void OnDestroy()
    {
        score = 200;
        GameEnded = false;

        dwarfManager.NoDwarvesLeft -= GameOverFail;
        dwarfManager.AllDwarvesSafe -= GameOverSuccess;
    }

    private void Update()
    {
        if (GameEnded)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0)
                || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.P))
                ReloadGame();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.P))
            GamePaused = !GamePaused;

        if (Input.GetMouseButton(0))
        {
            if (GamePaused)
                GamePaused = false;

            if (currentCursorMode != CursorMode.STOP)
                SetCursor(CursorMode.CLICK);
        }
        else if (workingDwarvesHoveredOver.Count > 0 && !dwarfManager.OnBreak)
            SetCursor(CursorMode.STOP);
        else
            SetCursor(CursorMode.REGULAR);

        if (GamePaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = dwarfManager.OnBreak ? 4f : 1f;
            pauseMenu.SetActive(false);
        }
    }

    IEnumerator SpawnDwarvesDelayed(float delay, int numberOfDwarves)
    {
        yield return new WaitForSeconds(delay);

        dwarfSpawner.TrySpawnDwarves(numberOfDwarves);
    }

    public void SetCursor(CursorMode cursorMode)
    {
        currentCursorMode = cursorMode;
        
        switch (cursorMode)
        {
            case CursorMode.REGULAR:
                Cursor.SetCursor(regularCursor, new Vector2(5f, 4f), UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.CLICK:
                Cursor.SetCursor(clickCursor, new Vector2(5f, 4f), UnityEngine.CursorMode.Auto);
                break;
            case CursorMode.STOP:
                Cursor.SetCursor(stopCursor, new Vector2(10f, 10f), UnityEngine.CursorMode.Auto);
                break;
        }
    }

  public static void AddToScore(int amountToAdd) {
    instance.score += amountToAdd;
    ScoreDisplay.UpdateScore(instance.score);
  }

    public static void CallBreak()
    {
        DwarfManager.CallBreak();
        Time.timeScale = 4f;
    }

    public static void GameOverSuccess()
    {
        GameEnded = true;

        EndScreenController.ShowWin();
        Time.timeScale = 1f;
    }

    public static void GameOverFail()
    {
        GameEnded = true;

        EndScreenController.ShowLoss();
        Time.timeScale = 1f;
    }

    public static void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    