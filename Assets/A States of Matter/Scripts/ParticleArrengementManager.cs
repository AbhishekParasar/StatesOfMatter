using UnityEngine;
using System.Collections.Generic;

public class ParticleArrengementManager :  MonoBehaviour
{

    [SerializeField] TMPro.TextMeshProUGUI taskStatusTxt;
    public ParticleDragZone[] dropZones;
    [SerializeField] string taskComplete = "";
    [SerializeField] string taskFailed = "";
    [SerializeField] List<ParticleDragZone> taskState = new List<ParticleDragZone>();
    [SerializeField] int draggedParticlesCount = 0;

    [SerializeField] GameObject nextButtonGO, retryButtonGO;

    [SerializeField] List<DraggableMatterParticle> draggableMatterParticlesList = new List<DraggableMatterParticle>();
    public AudioSource tryAgainAudioSource;
    public AudioSource goodJobAudioSource;

    void Start()
    {
        goodJobAudioSource.Stop();
        tryAgainAudioSource.Stop();
    }

    public void CheckCompletion()
    {
        foreach (var zone in dropZones)
        {
            if (!zone.IsFilledCorrectly())
            {
                return;
            }
        }
        if (draggedParticlesCount == 0)
        {
            OnAllStatesMatched(true);
        }
        else
        {
            OnAllStatesMatched(false);
            retryButtonGO.SetActive(true);
        }
    }

    public void CheckFailed()
    {
        draggedParticlesCount++;
    }

    private void OnAllStatesMatched(bool isTaskStatus)
    {
        Debug.Log("✅ Solid, Liquid, and Gas matched correctly!");
        if (isTaskStatus)
        {
            taskStatusTxt.text = taskComplete;
            goodJobAudioSource.Play();
        }
        else
        {
            taskStatusTxt.text = taskFailed;
            tryAgainAudioSource.Play();
        }

        nextButtonGO.SetActive(true);
    }

    public void Reset()
    {
        taskStatusTxt.text = null;

        foreach (DraggableMatterParticle draggableMatterParticle in draggableMatterParticlesList)
        {
            draggableMatterParticle.gameObject.transform.SetParent(this.transform);
            draggableMatterParticle.enabled = true;
            draggableMatterParticle.ResetPosition();
        }

        foreach (ParticleDragZone particleDrag in dropZones)
        {
            particleDrag.Reset();
        }
       
        retryButtonGO.SetActive(false);
        draggedParticlesCount = 0;
        goodJobAudioSource.Stop();
        tryAgainAudioSource.Stop();

    }
}
