using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip pickHit;

    public void PlaySound(Vector3Int dwarfPosition) {
        AudioSource.PlayClipAtPoint(pickHit, dwarfPosition);
    }
}
