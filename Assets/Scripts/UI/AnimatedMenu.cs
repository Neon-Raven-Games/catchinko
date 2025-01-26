using System.Collections;
using TMPro;
using UI.MenuStates;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedMenu : MonoBehaviour
{
    #region scrolling, refactor
[Header("Scrolling")]
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private float bottomPadding = 15;

    private Coroutine _scrollCoroutine;
    private VerticalLayoutGroup _layoutGroup;
    public void SetScrollingUp()
    {
        if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine);
        _scrollCoroutine = StartCoroutine(ScrollPanel(-scrollSpeed));
    }

    public void SetScrollingDown()
    {
        if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine);
        _scrollCoroutine = StartCoroutine(ScrollPanel(scrollSpeed));
    }

    public void StopScrolling()
    {
        if (_scrollCoroutine != null)
        {
            StopCoroutine(_scrollCoroutine);
            _scrollCoroutine = null;
        }
    }

    private IEnumerator ScrollPanel(float value)
    {
        var padding = _layoutGroup.padding.top - bottomPadding;
        while (true)
        {
            var contentPosition = scrollView.content.anchoredPosition;
            contentPosition.y += value * Time.deltaTime;

            var contentHeight = scrollView.content.rect.height - padding;
            contentPosition.y = Mathf.Clamp(contentPosition.y, 0, contentHeight);
            scrollView.content.anchoredPosition = contentPosition;
            yield return null;
        }
    }

    #endregion

    public MenuState State => currentState.state;
    public BaseMenuAnimationState currentState;

    public Vector2 targetPosition;
    public MenuType menuType;

    /// <summary>
    /// Additional space around the menu that should still be considered "safe" for the mouse.
    /// </summary>
    [ShowIf("isHover")] public float inputBoundsPadding = 20f;

    internal Vector2 originalPosition;
    internal RectTransform rectTransform;
    private StateController _stateController;

    public float moveDuration = 0.2f;

    /// <summary>
    /// The width of the sidebar. This is used to determine if the mouse is outside of the menu on the left side.
    /// </summary>
    [ShowIf("isHover")] public float sideBarWidth = 20;

    internal bool buttonClicked;

    [ShowIf("isClick")] public string minimizeText = "-";
    [ShowIf("isClick")] public string maximizedText = "+";
    [ShowIf("isClick")] public TextMeshProUGUI minimizedButtonText;

    [HideInInspector] public bool isHover;
    [HideInInspector] public bool isClick;

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
        _layoutGroup = scrollView.content.GetComponent<VerticalLayoutGroup>();
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