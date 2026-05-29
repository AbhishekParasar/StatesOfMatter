using UnityEngine;

public class IsPopUpShown : MonoBehaviour
{
    [SerializeField] private bool isPopUpShown;

    public void ShowPopUP(GameObject popupGameObject)
    {
        if (!isPopUpShown)
        {
            popupGameObject.SetActive(true);
            isPopUpShown = true;
        }
    }

    public void HidePopUP(GameObject popupGameObject)
    {
        if (isPopUpShown)
        {
            popupGameObject.SetActive(false);
            isPopUpShown = false;
        }
    }
}
