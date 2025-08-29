using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHolding : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool _isBtnHolding = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        _isBtnHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isBtnHolding = false;
    }

}
