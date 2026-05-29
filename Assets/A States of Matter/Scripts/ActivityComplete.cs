using UnityEngine;

public class ActivityComplete : MonoBehaviour
{
    // Har activity ka unique ID (1 to 4)
    public int ExpermentID;

    public void OnActivityButtonClick()
    {
        // Save activity completion
        PlayerPrefs.SetInt("Activity_" + ExpermentID, 1);
        PlayerPrefs.Save();

        Debug.Log("Activity " + ExpermentID + " Completed");
    }
}
