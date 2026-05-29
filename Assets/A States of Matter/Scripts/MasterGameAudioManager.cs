using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MasterGameAudioManager : MonoBehaviour
{
    public bool isAudioPlayed;
    public AudioSource audioSource;

    public UnityEvent OnPlaybackCompleted;
    public UnityEvent OnPlaybackStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

   

    public void PlayAudio(AudioClip audioClip)
    {
        if(!isAudioPlayed)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            //isAudioPlayed = true;

        }

    }
    public void PlayAudioTransitions(AudioClip audioClip)
    {
      
       
            audioSource.clip = audioClip;
            audioSource.Play();
            //isAudioPlayed = true;
    }

    public void StopPlaying()
    {
       
    }

    public void MuteAudioPlay()
    {
        audioSource.mute = true;
    }

    public void UnMuteAudio()
    {
        audioSource.mute = false;
    }


    public void PlayAudio(AudioClip audioClip,bool playing)
    {
        StartCoroutine(WaitForAudioToEnd(audioClip));
    }

    IEnumerator WaitForAudioToEnd(AudioClip audioClip)
    {

        if (!isAudioPlayed)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            //isAudioPlayed = true;
            OnPlaybackStarted.Invoke();
        }
        yield return new WaitWhile(() => audioSource.isPlaying);

        Debug.Log("Audio finished playing");
        OnPlaybackCompleted?.Invoke();
    }

}
