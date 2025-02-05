using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private SoundIntensityController _soundIntensityController;
    [SerializeField] private GoalType goal;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            other.gameObject.GetComponent<CatchinkoBall>().Despawn(goal);
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
        if (_soundIntensityController != null) _soundIntensityController.Stop();
    }
}