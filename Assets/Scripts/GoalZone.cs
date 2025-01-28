using Minimalist.Quantity;
using UnityEngine;
using UnityEngine.Events;

public class GoalZone : MonoBehaviour
{
    [SerializeField] private UnityEvent<GoalType> onScore;
    private SoundIntensityController _soundIntensityController;
    [SerializeField] private GoalType goal;

    [SerializeField] private QuantityBhv enemyQuantity;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // Destroy(other.gameObject);
            // GameResultsManager.GameOver(true);
            onScore.Invoke(goal);
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
