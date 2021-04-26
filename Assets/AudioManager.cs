using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip pickHit;
    public AudioClip mainTheme;
    public AudioClip drink;
    public AudioClip dig;
    public AudioClip hammerRope;
    public Dictionary<string, AudioClip> audioClips;
    public Dictionary<string, float> audioClipTimings;

    private void Start() {
        audioClipTimings = new Dictionary<string, float>();
        Dictionary<Vector3Int, float> audioHitLocationAndTime = new Dictionary<Vector3Int, float>();
        audioClipTimings["pickHit"] = 0f;
        audioClipTimings["dig"] = 0f;
        audioClipTimings["hammerRope"] = 0f;

        audioClips = new Dictionary<string, AudioClip>();
        audioClips.Add("pickHit", pickHit);
        audioClips.Add("mainTheme", mainTheme);
        audioClips.Add("drink", drink);
        audioClips.Add("dig", dig);
        audioClips.Add("hammerRope", hammerRope);

        GameObject soundGameObject = new GameObject("Sound");
        AudioSource mainThemeSource = soundGameObject.AddComponent<AudioSource>();
        mainThemeSource.clip = mainTheme;
        mainThemeSource.volume = 0.2f;
        mainThemeSource.loop = true;
        mainThemeSource.Play();
    }


    
}
    