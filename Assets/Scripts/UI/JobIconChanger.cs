using UnityEngine;

public class JobIconChanger : MonoBehaviour
{
    [SerializeField] Sprite diggingSprite;
    [SerializeField] Sprite miningSprite;
    [SerializeField] Sprite stopSignSprite;
    [SerializeField] Sprite ropeSprite;
    [SerializeField] Sprite getBoozeSprite;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RemoveIcon() => spriteRenderer.sprite = null;
    public void SetDiggingIcon() => spriteRenderer.sprite = diggingSprite;
    public void SetMiningIcon() => spriteRenderer.sprite = miningSprite;
    public void SetStopSignIcon() => spriteRenderer.sprite = stopSignSprite;
    public void SetRopeIcon() => spriteRenderer.sprite = ropeSprite;
    public void SetGetBoozeIcon() => spriteRenderer.sprite = getBoozeSprite;
}
