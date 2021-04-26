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

                    pickAudioSource.volume = 0.5f;
                    pickAudioSource.PlayOneShot(audioManager.audioClips[audioClipToPlay]);
                    Destroy(tempPickAudioSource, pickHitAudioDelay);
                    audioClipTimings[audioClipToPlay] = Time.time;
                }

                break;
            case "drink":
                GameObject tempDrinkAudioSource = new GameObject("Temp Audio Object");
                AudioSource drinkAudioSource = tempDrinkAudioSource.AddComponent<AudioSource>();

                drinkAudioSource.volume = 0.5f;
                drinkAudioSource.PlayOneShot(audioManager.audioClips[audioClipToPlay]);
                Destroy(tempDrinkAudioSource, 4);


                break;
            case "hammerRope":
                float hammerRopeAudioDelay = 0.5f;
                float lastTimeHammerRopePlayedAtPosition = audioClipTimings[audioClipToPlay];

                if (lastTimeHammerRopePlayedAtPosition + hammerRopeAudioDelay < Time.time) {
                    GameObject tempAudioSource = new GameObject("Temp Audio Object");
                    AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();

                    audioSource.volume = 0.3f;
                    audioSource.PlayOneShot(audioManager.audioClips[audioClipToPlay]);
                    Destroy(tempAudioSource, hammerRopeAudioDelay);
                    audioClipTimings[audioClipToPlay] = Time.time;
                }
                break;
            case "dig":
                float digAudioDelay = 1f;
                float lastDigPlayedAtPosition = audioClipTimings[audioClipToPlay];

                if (lastDigPlayedAtPosition + digAudioDelay < Time.time) {
                    GameObject tempAudioSource = new GameObject("Temp Audio Object");
                    AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();

                    audioSource.volume = 0.2f;
                    audioSource.PlayOneShot(audioManager.audioClips[audioClipToPlay]);
                    Destroy(tempAudioSource, digAudioDelay);
                    audioClipTimings[audioClipToPlay] = Time.time;
                }
                break;
            case "endBell":
                GameObject tempEndBellAudioSource = new GameObject("Temp Audio Object");
                AudioSource endBellAudioSource = tempEndBellAudioSource.AddComponent<AudioSource>();

                endBellAudioSource.volume = 0.5f;
                endBellAudioSource.PlayOneShot(audioManager.audioClips[audioClipToPlay]);
                Destroy(tempEndBellAudioSource, 4);
                break;
        }

     }
}
