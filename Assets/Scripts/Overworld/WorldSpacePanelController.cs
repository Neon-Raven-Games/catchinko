using UnityEngine;
using Overworld;
using TMPro;
using UnityEngine.SceneManagement;

public class WorldSpacePanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float popDuration = 0.5f;
    [SerializeField] private float minimizeDuration = 0.3f;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private TextMeshProUGUI enemyNameText;
    private Vector3 _shownScale;
    private Vector3 _defaultPosition;

    private void Start()
    {
        if (OverWorldGameManager.CurrentLevel == null)
        {
            Debug.LogWarning("OverWorldGameManager.CurrentLevel == null");
            return;
        }
        enemyNameText.text = OverWorldGameManager.CurrentLevel.data.enemyName;
    }

    public void PlayButtonLoadScene()
    {
        SceneManager.LoadScene(OverWorldGameManager.CurrentLevel.data.index);
    }

    /// <summary>
    /// Animates the panel to pop out of the player's position.
    /// </summary>
    public void UpdateEnemyName()
    {
        enemyNameText.text = OverWorldGameManager.CurrentLevel.data.enemyName;
    }

    /// <summary>
    /// Animates the panel to minimize and hide. Removed for button.
    /// </summary>
    public void MinimizePanel()
    {

    }
}
