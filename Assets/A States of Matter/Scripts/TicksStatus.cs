using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TicksStatus : MonoBehaviour
{
    public Image image;
    [SerializeField] Button button;
    [SerializeField] Button nextSceneButton;

    public Molecules molecules;
    Scene scene;
    void Awake()
    {
        scene = SceneManager.GetActiveScene();
        SetTickState();
    }

    public void SetTickState()
    {
        switch (molecules)
        {
            case Molecules.Argon:
                image.gameObject.SetActive(StateStatus.Instance.isArgonCompleted);
                button.interactable = false;
                
                break;
            case Molecules.Nitrogen:
                image.gameObject.SetActive(StateStatus.Instance.isNitrogenCompleted);
                button.interactable = false;
                
                break;
            case Molecules.Oxygen:
                image.gameObject.SetActive(StateStatus.Instance.isOxygenCompleted);
                button.interactable = false;
               
                break;
            case Molecules.Water:
                image.gameObject.SetActive(StateStatus.Instance.isWaterCompleted);
                button.interactable = false;
                break;
        }
    }

    
}
