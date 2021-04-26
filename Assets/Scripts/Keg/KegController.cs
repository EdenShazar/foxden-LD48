using UnityEngine;
using TMPro;

public class KegController : MonoBehaviour
{
    [SerializeField, Min(1)] int maxUses;

    TextMeshPro usesText;

    int usesLeft;

    void Start()
    {
        usesLeft = maxUses;
        usesText = GetComponentInChildren<TextMeshPro>();
        usesText.text = usesLeft.ToString();
    }

    public bool TryUseKeg()
    {
        if (usesLeft <= 0)
            return false;

        usesLeft--;
        usesText.text = usesLeft.ToString();

        if (usesLeft <= 0)
            Destroy(gameObject);

        return true;
    }
}
