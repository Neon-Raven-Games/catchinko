using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// todo, whenever we get the cosmetics, armor and stats worked out,
// we need to come up with a data object to ship the player to a scene with proper stats

// after the game is over, we need to reward them based off of performance here.
// Create a new performance data object when we have more measurable markers

public class GameResultsManager : MonoBehaviour
{
    public static int ballCount = 25;
    private static GameResultsManager _instance;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    
    // todo, serialized for testing purposes
    [SerializeField] private float _lastAward;
    [SerializeField] private TextMeshProUGUI ballCountText;
    public static bool IsGameOver => _instance.winScreen.transform.localScale.x > 0 || _instance.loseScreen.transform.localScale.x > 0;
    public static bool lastGameWon;
    
    public static void GameOver(bool won)
    {
        if (!_instance) return;
        lastGameWon = won;
        if (won)
        {
            _instance.winScreen.transform.DOScale(_instance._canvasScale, 0.5f).SetEase(Ease.OutBack);
            _instance.ProcessPlayerAwards();
        }
        else
        {
            _instance.loseScreen.transform.DOScale(_instance._canvasScale, 0.5f).SetEase(Ease.OutBack);
            _instance.ProcessPlayerLoss();
        }
    }
    
    public void BackToLevelSelect()
    {
        _instance.winScreen.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => SceneManager.LoadScene(0));
        _instance.loseScreen.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => SceneManager.LoadScene(0));
    }

    public static float FetchLastAward()
    {
        var award = _instance._lastAward;
        _instance._lastAward = 0f;
        Debug.Log("Gave out the last award: " + award);
        return award;
    }

    private void ProcessPlayerLoss()
    {
        if (ballCount > 25)
        {
            ballCount -= 5;
            ballCountText.text = ballCount.ToString();
        }
        else
        {
            ballCount = 25;
        }
        
        // can we remove this?
        _lastAward = -1f;
        Debug.Log($"Reward: {_lastAward}. Player Lost :( pass performance stats for mercy or something?");
    }

    private void ProcessPlayerAwards()
    {
        // add balls? health?
        ballCount += 20;
        ballCountText.text = ballCount.ToString();
        
        // remove award?
        _lastAward = 1f;
        Debug.Log($"Reward: {_lastAward} Player won the game! pass performance stats to game over method");
    }

    private Vector3 _canvasScale;
    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        _canvasScale = winScreen.transform.localScale;
        
        winScreen.gameObject.SetActive(true);
        winScreen.transform.localScale = Vector3.zero;
        
        loseScreen.gameObject.SetActive(true);
        loseScreen.transform.localScale = Vector3.zero;
        ballCountText.text = ballCount.ToString();
    }

    public static void ReduceBallCount()
    {
        ballCount -= 1;
        _instance.ballCountText.text = ballCount.ToString();
    }
}
