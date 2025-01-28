using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private SoundIntensityController _soundIntensityController;
    [SerializeField] private GoalType goal;
    private BallDropper _dropper;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            _dropper.ScoreSpawn(goal);
        }
    }

    private void Start()
    {
        _dropper = FindObjectOfType<BallDropper>();
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