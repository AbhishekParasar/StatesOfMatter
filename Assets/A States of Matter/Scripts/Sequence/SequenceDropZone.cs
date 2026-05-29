using UnityEngine;
using UnityEngine.EventSystems;

public class SequenceDropZone : MonoBehaviour, IDropHandler
{
    public int zoneIndex;
    private SequenceDragItem placedItem;

    public void OnDrop(PointerEventData eventData)
    {
        if (placedItem != null) return;

        SequenceDragItem item =
            eventData.pointerDrag.GetComponent<SequenceDragItem>();

        if (item == null || item.isLocked) return;

        placedItem = item;
        item.LockToZone(transform);

        SequenceMatchManager.Instance.OnItemDropped();
    }

    public bool IsCorrect()
    {
        return placedItem != null &&
               placedItem.sequenceIndex == zoneIndex;
    }

    public void Reset()
    {
        if (placedItem != null)
        {
            placedItem = null;
        }
    }
}
