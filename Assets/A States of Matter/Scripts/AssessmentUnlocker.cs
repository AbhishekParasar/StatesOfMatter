using UnityEngine;
using UnityEngine.UI;

public class AssessmentUnlocker : MonoBehaviour
{
    public Button assessmentButton;
    public Button particleArrangementButton;

    void Start()
    {
      //  CheckAllActivities();
    }

    void CheckAllActivities()
    {
        bool allDone = true;

        for (int i = 1; i <= 4; i++)
        {
            if (PlayerPrefs.GetInt("Activity_" + i, 0) == 0)
            {
                allDone = false;
                break;
            }
            else
            {
                Debug.Log("Activity " + i + PlayerPrefs.GetInt("Activity_"));
            }
        }

        assessmentButton.gameObject.SetActive(allDone);
        particleArrangementButton.gameObject.SetActive(allDone);
    }

    public void CheckAllActivitiesComplete()
    {
        CheckAllActivities();
    }
}
