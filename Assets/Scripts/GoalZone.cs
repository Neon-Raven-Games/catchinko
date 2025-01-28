using UnityEngine;
using UnityEngine.Events;

public class GoalZone : MonoBehaviour
{
    public int scoreValue = 1;
    [SerializeField] private UnityEvent onScore;
    private SoundIntensityController _soundIntensityController;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // Destroy(other.gameObject);
            // GameResultsManager.GameOver(true);
            onScore.Invoke();
        }
    }

    private void Start()
    {
        var audioSource = GetComponent<AudioSource>();
        _soundIntensityController = new SoundIntensityController(audioSource, this);
    }

    public void SetIntensity(float normalizedDistance)
    {
        _soundIntensityController.SetIntensity(normalizedDistance);
    }

    public void Stop()
    {
        _soundIntensityController.Stop();
    }
}
