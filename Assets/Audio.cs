using UnityEngine;

public class AudioController : MonoBehaviour
{
    
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}