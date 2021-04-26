using UnityEngine;
using TMPro;

public class EndScreenController : MonoBehaviour
{
    [SerializeField] SpriteRenderer background;
    [SerializeField] TextMeshPro winText;
    [SerializeField] TextMeshPro loseText;
    [SerializeField] TextMeshPro scoreText;
    [SerializeField] SpriteRenderer scoreIcon;

    public void Hide()
    {
        background.color = Constants.clearColor;
        winText.color = Constants.clearColor;
        loseText.color = Constants.clearColor;
        scoreText.color = Constants.clearColor;
        scoreIcon.color = Constants.clearColor;
    }

    public void ShowWin()
    {
        background.color = Color.white;
        winText.color = Color.white;
        scoreText.color = Color.white;
        scoreIcon.color = Color.white;

        loseText.color = Constants.clearColor;

        scoreText.text = GameController.Score.ToString();
    }

    public void ShowLoss()
    {
        background.color = Color.white;
        loseText.color = Color.white;

        winText.color = Constants.clearColor;
        scoreText.color = Constants.clearColor;
        scoreIcon.color = Constants.clearColor;
    }
}
