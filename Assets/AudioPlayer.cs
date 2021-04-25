using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    AudioManager audioManager;
    public Dictionary<string, float> audioClipTimings;

    private void Start() {
        audioManager = GameController.AudioManager;
        audioClipTimings = audioManager.audioClipTimings;
    }

    public void PlaySound(string audioClipToPlay, Vector3Int dwarfPosition) {

        switch (audioClipToPlay) {
            case "pickHit":
                float pickHitAudioDelay = 2f;
                float lastTimePlayedAtPosition = audioClipTimings[audioClipToPlay];

                if (lastTimePlayedAtPosition + pickHitAudioDelay < Time.time) {
                    GameObject tempPickAudioSource = new GameObject("Temp Audio Object");
                    AudioSource pickAudioSource = tempPickAudioSource.AddComponent<AudioSource>();

                    pickAudioSource.PlayOneShot(audioManager.audioClips[audioClipToPlay]);
                    Destroy(tempPickAudioSource, pickHitAudioDelay);
                    audioClipTimings[audioClipToPlay] = Time.time;
                }

                break;
            case "drink":
                GameObject tempAudioSource = new GameObject("Temp Audio Object");
                AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();

                audioSource.PlayOneShot(audioManager.audioClips[audioClipToPlay]);
                Destroy(tempAudioSource, 4);


                break;
        }

     }
}
