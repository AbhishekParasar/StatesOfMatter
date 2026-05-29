using UnityEngine;
public class PopUpManager : MonoBehaviour
{
    public GameObject popUpGameObject;
    public IsPopUpShown isPopUpShown;

    void Start()
    {
        if (isPopUpShown == null)
        {
            Debug.LogError("isPopUpShown is not assigned in the Inspector.");
        }

        if (popUpGameObject == null)
        {
            Debug.LogError("popUpGameObject is not assigned in the Inspector.");
        }
    }

    public void ShowPopUpScreen()
    {
        if (isPopUpShown == null || popUpGameObject == null)
        {
            Debug.LogError("Cannot show popup: Missing references.");
            return;
        }

        isPopUpShown.ShowPopUP(popUpGameObject);
    }
}
