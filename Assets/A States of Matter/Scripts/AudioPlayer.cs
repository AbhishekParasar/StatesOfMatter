using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip audioClips;

    public void SoundPlay()
    {
        _audioSource.clip = audioClips;
        _audioSource.Play();
    }
}
