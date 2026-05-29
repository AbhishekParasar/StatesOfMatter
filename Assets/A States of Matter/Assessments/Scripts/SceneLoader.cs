using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MatterManager;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Molecules molecules;
    [SerializeField] TicksStatus ticksStatus;

    [SerializeField] bool isReloadSimulation;
    Scene scene;
    public void LoadScene(int sceneIndex)
    {
        // Optional safety: ensure index exists in Build Settings
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Scene index {sceneIndex} is out of range.");
            return;
        }

        SceneManager.LoadScene(sceneIndex);
    }

    public void ResetData()
    {
        if (StateStatus.Instance != null)
        {
            StateStatus.Instance.ResetData();
        }
    }

    public void ReloadScene()
    {
        
            scene = SceneManager.GetActiveScene();
        
        GetComponent<Button>().interactable = false;
        if(ticksStatus != null)
        ticksStatus.image.enabled = false;
        Invoke("ResetStatesData", 1.0f);
    }

    void ResetStatesData()
    {
        GetComponent<Button>().interactable = true;
        if (isReloadSimulation)
        {
            SceneManager.LoadScene(0);
            StateStatus.Instance.isArgonCompleted = false;
            StateStatus.Instance.isNitrogenCompleted = false;
            StateStatus.Instance.isOxygenCompleted = false;
            StateStatus.Instance.isWaterCompleted = false;
        }
        else
        {
            SceneManager.LoadScene(scene.buildIndex);

            switch (molecules)
            {
                case Molecules.Argon:
                    StateStatus.Instance.isArgonCompleted = false;
                    break;
                case Molecules.Nitrogen:
                    StateStatus.Instance.isNitrogenCompleted = false;
                    break;
                case Molecules.Oxygen:
                    StateStatus.Instance.isOxygenCompleted = false;
                    break;
                case Molecules.Water:
                    StateStatus.Instance.isWaterCompleted = false;
                    break;
            }
        }
        if (ticksStatus != null)
            ticksStatus.SetTickState();
    }

    
}
