using System.Collections;
using Overworld;
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
    [SerializeField] private float originalY;

    private Coroutine _scrollCoroutine;
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
        while (true)
        {
            var contentPosition = scrollView.content.anchoredPosition;
            contentPosition.y += value * Time.deltaTime;

            contentPosition.y = Mathf.Clamp(contentPosition.y, 0, originalY);
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
        originalY = scrollView.content.anchoredPosition.y;
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        currentState.OnUpdate();
        
        if (currentState.state != MenuState.Open) return;
        if (Input.GetKeyUp(KeyCode.W)) OverWorldGameManager.SetCurrentBoss(OverWorldGameManager.GetNextBoss());
        if (Input.GetKeyUp(KeyCode.S)) OverWorldGameManager.SetCurrentBoss(OverWorldGameManager.GetPreviousBoss());
    }

    public void ChangeState(MenuState state) =>
        _stateController.ChangeState(state);

    public void SetButtonText(MenuState state)
    {
        if (state == MenuState.Open) minimizedButtonText.text = minimizeText;
        if (state == MenuState.Closed) minimizedButtonText.text = maximizedText;
    }
}