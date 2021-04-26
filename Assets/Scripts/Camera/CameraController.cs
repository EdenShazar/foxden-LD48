using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, Range(0f, 0.5f)] float scrollRegionWidth = 0.15f;
    [SerializeField, Min(0.01f)] float scrollSensitivity = 1f;
    [SerializeField] float topBound = 9f;

    new Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();

        camera.orthographicSize = GameController.TilemapController.WidthWorld / camera.aspect * 0.5f;
    }

    void Update()
    {
        float mouseY = Input.mousePosition.y / camera.pixelHeight;

        float scrollFactor;
        float scrollDirection;
        if (mouseY <= 0.5f)
        {
            scrollFactor = Mathf.Lerp(1f, 0f, mouseY / scrollRegionWidth);
            scrollDirection = -1f;
        }
        else
        {
            scrollFactor = Mathf.Lerp(0f, 1f, (mouseY + scrollRegionWidth - 1f) / scrollRegionWidth);
            scrollDirection = 1f;
        }

        // Parabolic curve feels better than linear
        scrollFactor *= scrollFactor;

        camera.transform.Translate(Vector3.up * scrollDirection * scrollSensitivity * scrollFactor * Time.deltaTime);

        float bottomBound = GameController.DwarfManager.GetLowestDwarfHeight();
        if (bottomBound == Mathf.NegativeInfinity)
            bottomBound = -10f;

        float clampedCameraY = Mathf.Clamp(camera.transform.position.y, bottomBound, topBound);
        camera.transform.Translate(Vector3.up * (clampedCameraY - camera.transform.position.y));
    }
}
