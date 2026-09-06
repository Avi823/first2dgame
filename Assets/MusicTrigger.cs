using System.Collections;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    private AudioSource myAudioSource;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only activate if the Player enters and this track isn't already playing
        if (other.CompareTag("Player") && !myAudioSource.isPlaying)
        {
            // Find all playing AudioSources in the scene
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

            foreach (AudioSource audio in allAudioSources)
            {
                // Fade out and stop any other active music
                if (audio != myAudioSource && audio.isPlaying)
                {
                    StartCoroutine(FadeOutAndStop(audio));
                }
            }

            // Start playing the new track
            myAudioSource.Play();
        }
    }

    private IEnumerator FadeOutAndStop(AudioSource audio)
    {
        float startVolume = audio.volume;

        // Smoothly reduce volume over the fadeDuration
        while (audio.volume > 0)
        {
            audio.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audio.Stop();
        audio.volume = startVolume; // Reset volume for future playback
    }
}