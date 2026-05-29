using UnityEngine;
using System.Collections.Generic;
public class SequenceMatchManager : MonoBehaviour
{
    public static SequenceMatchManager Instance;

    public SequenceDropZone[] dropZones;

    [SerializeField] string taskComplete = "";
    [SerializeField] string taskFailed = "";

    [SerializeField] TMPro.TextMeshProUGUI taskStatusTxt;
   public int noOfMatchmakers;
    [SerializeField] GameObject nextButtonGO;
    [SerializeField] GameObject retryButtonGO;

    [SerializeField] List<SequenceDragItem> sequenceDragItems;

    public AudioSource tryAgainAudioSource;
    public AudioSource goodJobAudioSource;

    void OnEnable()
    {
       // ShuffleUI();
    }

   
    void Awake()
    {
        goodJobAudioSource.Stop();
        tryAgainAudioSource.Stop();
        taskStatusTxt.text = null;
        Instance = this;
        dropZones = GetComponentsInChildren<SequenceDropZone>();
        nextButtonGO.gameObject.SetActive(false);
    }

    [SerializeField] private List<RectTransform> uiItems;

    public void ShuffleUI()
    {

        // Cache positions
        List<Vector2> positions = new List<Vector2>();
        foreach (var item in uiItems)
            positions.Add(item.anchoredPosition);

        // Fisher-Yates shuffle
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        // Apply shuffled positions
        for (int i = 0; i < uiItems.Count; i++)
            uiItems[i].anchoredPosition = positions[i];

    }

    public void OnItemDropped()
    {
        CheckSequence();
    }

    private void CheckSequence()
    {
        foreach (var zone in dropZones)
        {
            if (!zone.IsCorrect())
            {
                noOfMatchmakers++;
                Debug.Log("❌ Sequence incorrect");
                if(noOfMatchmakers == 4)
                {
                    OnSequenceCompleted(false);
                    retryButtonGO.SetActive(true);
                    nextButtonGO.gameObject.SetActive(false);
                }
                return;
            }
            
        }

        Debug.Log("✅ Sequence completed!");
            OnSequenceCompleted(true);
           
    }

    private void OnSequenceCompleted(bool isTaskStatus)
    {
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

        nextButtonGO.gameObject.SetActive(isTaskStatus);
    }

    void Reset()
    {
        foreach(SequenceDragItem sequenceDragItem in sequenceDragItems)
        {
            sequenceDragItem.ResetPosition();
            sequenceDragItem.isLocked = false;
            goodJobAudioSource.Stop();
            tryAgainAudioSource.Stop();
        }
    }

    public void TryAgain()
    {
        Reset();
        taskStatusTxt.text = null;
        retryButtonGO.SetActive(false);
        nextButtonGO.gameObject.SetActive(false);
        noOfMatchmakers = 0;

        foreach(SequenceDropZone sequenceDropZone in dropZones)
        {
            sequenceDropZone.Reset();
        }
    }
}
