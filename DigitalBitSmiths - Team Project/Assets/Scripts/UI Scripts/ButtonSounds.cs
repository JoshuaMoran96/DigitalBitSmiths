using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler 
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UISounds.Instance != null)
            UISounds.Instance.PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UISounds.Instance != null)
            UISounds.Instance.StopHover();
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        if (UISounds.Instance != null)
            UISounds.Instance.PlayClick();
    }
}