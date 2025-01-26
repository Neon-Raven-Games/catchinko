using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MouseOverEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AnimatedMenu animatedMenu;
    [SerializeField] private UnityEvent onMouseEnter;
    [SerializeField] private UnityEvent onMouseExit;
    
    /// <summary>
    /// Called when the mouse pointer enters the UI element.
    /// </summary>
    /// <param name="eventData">Event data associated with the pointer.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animatedMenu.State != MenuState.Open) return;
        onMouseEnter?.Invoke();
    }

    /// <summary>
    /// Called when the mouse pointer exits the UI element.
    /// </summary>
    /// <param name="eventData">Event data associated with the pointer.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (animatedMenu.State != MenuState.Open) return;
        onMouseExit?.Invoke();
    }
}