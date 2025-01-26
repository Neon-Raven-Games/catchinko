using UnityEngine;
using DG.Tweening;
using Overworld;
using UnityEngine.SceneManagement;

public class WorldSpacePanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float popDuration = 0.5f;
    [SerializeField] private float minimizeDuration = 0.3f;
    [SerializeField] private Vector3 hiddenScale = Vector3.zero;
    [SerializeField] private Transform playerTransform;
    
    private Vector3 _shownScale;
    private Vector3 _defaultPosition;

    private void Awake()
    {
        _shownScale = panelRect.localScale;
        if (panelRect == null) panelRect = GetComponent<RectTransform>();
        panelRect.localScale = hiddenScale;
        _defaultPosition = panelRect.localPosition;
    }

    public void PlayButtonLoadScene()
    {
        SceneManager.LoadScene(OverWorldGameManager.CurrentLevel.data.index);
    }

    /// <summary>
    /// Animates the panel to pop out of the player's position.
    /// </summary>
    public void ShowPanel()
    {
        panelRect.position = playerTransform.position;
        panelRect.localScale = hiddenScale;

        panelRect.DOScale(_shownScale, popDuration).SetEase(Ease.OutBack);
        panelRect.DOAnchorPosY(12f, popDuration).SetEase(Ease.OutBack);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, popDuration);
        }
    }

    /// <summary>
    /// Animates the panel to minimize and hide.
    /// </summary>
    public void MinimizePanel()
    {
        panelRect.DOAnchorPosX(0, minimizeDuration).SetEase(Ease.InBack);
        panelRect.DOScale(hiddenScale, minimizeDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            if (canvasGroup) canvasGroup.alpha = 0f;
            panelRect.localPosition = _defaultPosition;
        });
    }
}
