using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ActivityManager : MonoBehaviour
{
    [SerializeField] List<GameObject> activityGOlist = new List<GameObject>();
    [SerializeField] AudioSource audioSource;

    [SerializeField] MasterGameAudioManager masterGameAudioManager;
    public void ActivityControl(int index)
    {
        foreach( GameObject obj in activityGOlist)
        {
            obj.SetActive(false);
        }
        activityGOlist[index].SetActive(true);
    }

    void Awake()
    {
        masterGameAudioManager.MuteAudioPlay();
    }
   
}
