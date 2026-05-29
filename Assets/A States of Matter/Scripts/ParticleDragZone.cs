using UnityEngine;
using UnityEngine.EventSystems;
public class ParticleDragZone  : MonoBehaviour, IDropHandler
{
    public ParticleState expectedState;

    private DraggableMatterParticle currentParticle;

    [SerializeField] ParticleArrengementManager ParticleArrengementManager;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?
            .GetComponent<DraggableMatterParticle>();

        if (dragged == null)
            return;

        if (dragged.particleState == expectedState)
        {
            dragged.transform.SetParent(transform);
            dragged.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            currentParticle = dragged;
            GetComponent<UnityEngine.UI.Image>().color = Color.green;
           
            //ParticleArrengementManager.CheckFailed();

        }
        else
        {
            GetComponent<UnityEngine.UI.Image>().color = Color.red;
            // dragged.ResetPosition();
            currentParticle = dragged;
            dragged.transform.SetParent(transform);
            dragged.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
         
            ParticleArrengementManager.CheckFailed();
        }
        ParticleArrengementManager.CheckCompletion();
        currentParticle.enabled = false;
    }

    public bool IsFilledCorrectly()
    {
        return currentParticle != null;
    }

    public void Reset()
    {
        GetComponent<UnityEngine.UI.Image>().color = Color.white;
        currentParticle = null;
    }
}
