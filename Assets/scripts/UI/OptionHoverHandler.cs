using UnityEngine;
using UnityEngine.EventSystems;

public class OptionHoverHandler : MonoBehaviour, IPointerEnterHandler
{
    public int optionIndex;
    public SelectionArrow selectionArrow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectionArrow.ChangePositionByIndex(optionIndex);
    }
}