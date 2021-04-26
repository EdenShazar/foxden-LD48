using UnityEngine;

public class SelfDestructor : MonoBehaviour
{
    [SerializeField, Min(0.01f)] float selfDestructTime;

    float endTime;

    void Start()
    {
        endTime = Time.time + selfDestructTime;
    }

    void Update()
    {
        if (Time.time >= endTime)
            Destroy(gameObject);
    }
}
