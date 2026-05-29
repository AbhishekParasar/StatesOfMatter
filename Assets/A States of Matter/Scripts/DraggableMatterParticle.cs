using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableMatterParticle : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ParticleState particleState;

    [HideInInspector] public Transform originalParent;

    private Canvas canvas;
    private RectTransform rectTransform;
    [SerializeField] Vector2 rectTransformOriginalPosition;
    private CanvasGroup canvasGroup;


    [SerializeField] ParticleArrengementManager ParticleArrengementManager;
    private void Awake()
    {
        rectTransformOriginalPosition = GetComponent<RectTransform>().anchoredPosition;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == canvas.transform)
        {
            ResetPosition();
        }
        else
        {
            //ParticleArrengementManager.CheckCompletion();
        }
    }

    public void ResetPosition()
    {
        transform.SetParent(originalParent);
        
        rectTransform.anchoredPosition = new Vector2(rectTransformOriginalPosition.x, rectTransformOriginalPosition.y);
        if(!canvasGroup.blocksRaycasts)
        canvasGroup.blocksRaycasts = true;

    }

}

public enum ParticleState
{
    Solid,
    Liquid,
    Gas
}