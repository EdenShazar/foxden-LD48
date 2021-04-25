using UnityEngine;

public class ScoreDisplay : MonoBehaviour {
  private static ScoreDisplay instance;
  private TextMesh mesh;

  private void Start() {
    EnsureSingleton();
    mesh = gameObject.GetComponent<TextMesh>();
  }

  public static void UpdateScore(int newScore) {
    instance.mesh.text = $"Score:{newScore}";
  }

  private void EnsureSingleton() {
    if (instance == null) {
      instance = this;
    } else {
      Debug.LogWarning("ScoreDisplay instance already exists on " + instance.gameObject +
              ". Deleting instance from " + gameObject);
      DestroyImmediate(this);
    }
  }
}
