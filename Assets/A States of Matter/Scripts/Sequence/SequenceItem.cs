using UnityEngine;
using UnityEngine.EventSystems;

public class SequenceDragItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int sequenceIndex;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [SerializeField] Vector2 startPos;
    private Transform startParent;

    public bool isLocked;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPos = rectTransform.anchoredPosition;
        startParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        transform.SetParent(startParent.root); // move above UI


        rectTransform.SetAsLastSibling();

    }

    public Canvas canvas;

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        //rectTransform.anchoredPosition += eventData.delta;

        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    canvas.transform as RectTransform,
        //    eventData.position,
        //    canvas.worldCamera,
        //    out localPoint
        //);

        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;

       // rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (!isLocked)
            ResetPosition();
    }

    public void LockToZone(Transform zone)
    {
        isLocked = true;
        transform.SetParent(zone);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void ResetPosition()
    {
        transform.SetParent(startParent);
        rectTransform.anchoredPosition = startPos;
    }
}
