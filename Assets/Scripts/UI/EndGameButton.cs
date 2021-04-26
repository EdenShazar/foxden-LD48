using UnityEngine;

public class EndGameButton : MonoBehaviour
{
    [SerializeField] SpriteRenderer border;

    void Start()
    {
        border.color = Constants.clearColor;
    }

    void OnMouseOver()
    {
        border.color = Color.white;
    }

    void OnMouseExit()
    {
        border.color = Constants.clearColor;
    }

    void OnMouseDown()
    {
        if (GameController.DwarfManager.OnBreak)
            return;

        GameController.AudioManager.PlaySoundOnce("endBell");
        GameController.CallBreak();
    }
}
