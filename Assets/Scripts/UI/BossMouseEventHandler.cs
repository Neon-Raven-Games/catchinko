using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BossMouseEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private AnimatedMenu animatedMenu;
    [SerializeField] private UnityEvent<CatBoss> onMouseEnter;
    [SerializeField] private UnityEvent onMouseExit;
    [SerializeField] private CatBoss boss;

    [SerializeField] private GameObject highlightObject;
    private OverWorldController _overWorldController;
    public void Initialize(CatBoss initBoss, AnimatedMenu menu)
    {
        boss = initBoss;
        animatedMenu = menu;
        _overWorldController = FindObjectOfType<OverWorldController>();
        
        onMouseEnter.AddListener((_ => highlightObject.SetActive(true)));
        onMouseEnter.AddListener(_overWorldController.TravelToLevel);
        
        onMouseExit.AddListener(() => highlightObject.SetActive(false));
        
        // add event listener for highlight object on mouse enter/exit
        // add event listener for OverworldController on enter
    }
    /// <summary>
    /// Called when the mouse pointer enters the UI element.
    /// </summary>
    /// <param name="eventData">Event data associated with the pointer.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animatedMenu.State != MenuState.Open) return;
        onMouseEnter?.Invoke(boss);
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

    public void OnPointerMove(PointerEventData eventData)
    {
        if (animatedMenu.State != MenuState.Open) return;
        if (!highlightObject.activeInHierarchy)
        {
            highlightObject.SetActive(true);
            onMouseEnter?.Invoke(boss);
        }
    }
}