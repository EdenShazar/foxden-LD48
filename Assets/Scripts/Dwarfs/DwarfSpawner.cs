using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DwarfSpawner : MonoBehaviour
{
    [SerializeField] GameObject dwarfPrefab;
    [SerializeField] Transform dwarfParent;
    [SerializeField] Sprite wagonRegularSprite;
    [SerializeField] Sprite wagonHighlightSprite;
    [SerializeField] SpriteRenderer wagonSpriteRenderer;
    [SerializeField, Min(0f)] int dwarfCost = 10;
    [SerializeField, Min(0.01f)] float minimumSpeed = 6f;
    [SerializeField, Min(0.01f)] float maximumSpeed = 10f;

    public bool TrySpawnDwarf()
    {
        if (GameController.Score < dwarfCost)
            return false;

        GameObject newDwarf = Instantiate(dwarfPrefab, transform.position, Quaternion.identity, dwarfParent);
        newDwarf.GetComponent<Rigidbody2D>().velocity = transform.up * Random.Range(minimumSpeed, maximumSpeed);
        GameController.AddToScore(-dwarfCost);

        return true;
    }

    public void TrySpawnDwarves(int number)
    {
        StartCoroutine(SpawnDwarvesAtIntervals(number));
    }

    IEnumerator SpawnDwarvesAtIntervals(int numberOfDwarves)
    {
        for (int i = 0; i < numberOfDwarves; i++)
        {
            if (GameController.DwarfManager.OnBreak || !TrySpawnDwarf())
                yield break;

            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }

    void OnMouseDown()
    {
        if (GameController.DwarfManager.OnBreak)
            return;

        TrySpawnDwarf();
    }

    void OnMouseOver()
    {
        if (GameController.DwarfManager.OnBreak)
            return;

        wagonSpriteRenderer.sprite = wagonHighlightSprite;
    }

    void OnMouseExit()
    {
        wagonSpriteRenderer.sprite = wagonRegularSprite;
    }
}
