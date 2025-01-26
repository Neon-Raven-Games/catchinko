using TMPro;
using UI.MenuStates;
using UnityEngine;

public class AnimatedMenu : MonoBehaviour
{
    public MenuState State => currentState.state;
    public BaseMenuAnimationState currentState;
    
    public Vector2 targetPosition;
    public MenuType menuType;
    
    /// <summary>
    /// Additional space around the menu that should still be considered "safe" for the mouse.
    /// </summary>
    [ShowIf("isHover")]
    public float inputBoundsPadding = 20f;

    internal Vector2 originalPosition;
    internal RectTransform rectTransform;
    private StateController _stateController;
    
    public float moveDuration = 0.2f;
    
    /// <summary>
    /// The width of the sidebar. This is used to determine if the mouse is outside of the menu on the left side.
    /// </summary>
    [ShowIf("isHover")]
    public float sideBarWidth = 20;

    internal bool buttonClicked;
    
    [ShowIf("isClick")]
    public string minimizeText = "-";
    [ShowIf("isClick")]
    public string maximizedText = "+";
    [ShowIf("isClick")]
    public TextMeshProUGUI minimizedButtonText;
    
    [HideInInspector]
    public bool isHover;
    [HideInInspector]
    public bool isClick;

    private void OnValidate()
    {
        isHover = menuType == MenuType.HoverActivated;
        isClick = menuType == MenuType.ClickActivated;
    }

    public void ToggleButtonClicked() =>
        buttonClicked = !buttonClicked;

    private void Awake()
    {
        _stateController = new StateController(this);
    }
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        currentState.OnUpdate();
    }

    public void ChangeState(MenuState state) =>
        _stateController.ChangeState(state);

    public void SetButtonText(MenuState state)
    {
        if (state == MenuState.Open) minimizedButtonText.text = minimizeText;
        if (state == MenuState.Closed) minimizedButtonText.text = maximizedText;
    }
}
