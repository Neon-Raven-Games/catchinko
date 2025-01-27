using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeSoundIntensity
{
    public float volume;
    public float pitch;
    public float distance;
    public float time;
}

[Serializable]
public class SoundIntensityController
{
    private readonly AudioSource _audioSource;
    private readonly CatchinkoBall _catchinkoBall;

    // Animation Curves for dynamic control
    [SerializeField] private AnimationCurve volumeOverTime = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve pitchOverTime = AnimationCurve.EaseInOut(0, 2, 1, 1);
    [SerializeField] private AnimationCurve volumeOverDistance = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private AnimationCurve pitchOverDistance = AnimationCurve.Linear(0, 1.5f, 1, 1);

    private RuntimeSoundIntensity _currentIntensity;

    // Constructor
    public SoundIntensityController(AudioSource audioSource, CatchinkoBall ball)
    {
        _audioSource = audioSource;
        _catchinkoBall = ball;

        // Default runtime intensity
        _currentIntensity = new RuntimeSoundIntensity
        {
            volume = 0,
            pitch = 1,
            distance = 0,
            time = 0
        };
    }

    public void Stop()
    {
        if (_audioSource.isPlaying) _catchinkoBall.StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float fadeDuration = 0.5f; // Duration of the fade-out
        float startVolume = _audioSource.volume; // Record the starting volume

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            // Lerp the volume down over time
            _audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        // Ensure the volume reaches 0 and stop the audio
        _audioSource.volume = 0;
        _audioSource.Stop();

        // Reset pitch for future use
        _audioSource.pitch = 1;
    }

    public void Collect()
    {
        _catchinkoBall.StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float fadeDuration = 0.2f; // Quick fade-out duration
        float startVolume = _audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        _audioSource.volume = 0;
        Stop();
    }

    public void SetIntensity(float distance, float time)
    {
        if (!_audioSource.isPlaying) _audioSource.Play();

        var normalizedTime = Mathf.Clamp01(time);
        var normalizedDistance = Mathf.Clamp01(distance);

        var timeVolume = volumeOverTime.Evaluate(normalizedTime);
        var distanceVolume = volumeOverDistance.Evaluate(normalizedDistance);
        _audioSource.volume = Mathf.Clamp01(timeVolume * distanceVolume);

        var timePitch = pitchOverTime.Evaluate(normalizedTime);
        var distancePitch = pitchOverDistance.Evaluate(normalizedDistance);
        _audioSource.pitch = Mathf.Max(0.1f, timePitch * distancePitch);
    }
}


public class CatchinkoBall : MonoBehaviour
{
    [SerializeField] private float smokeDelay;
    [SerializeField] private float explosionDelay;
    [SerializeField] private float despawnDelay;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject explosion;

    [SerializeField] private AudioSource dropperSound;

    [Header("Sound Intensity")]
    [SerializeField] private List<GoalZone> goals;

    [SerializeField] private AudioSource positiveGoalIntensitySound;
    [SerializeField] private AudioSource negativeGoalIntensitySound;
    [SerializeField] private AudioSource missGoalIntensitySound;

    [SerializeField] private List<GameObject> powerUps;
    [SerializeField] private AudioSource positivePowerUpIntensitySound;
    [SerializeField] private AudioSource negativePowerUpIntensitySound;

    private SoundIntensityController _positiveGoalIntensityController;
    private SoundIntensityController _negativeGoalIntensityController;
    private SoundIntensityController _missGoalIntensityController;

    private SoundIntensityController _positivePowerUpIntensityController;
    private SoundIntensityController _negativePowerUpIntensityController;

    public void Despawn() => StartCoroutine(DespawnRoutine());

    private void OnEnable()
    {
        // todo, extract this to a controller so we don't need to do this every time
        goals = FindObjectsOfType<GoalZone>().ToList();
    }

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(smokeDelay);
        smoke.SetActive(true);
        yield return new WaitForSeconds(explosionDelay);
        explosion.SetActive(true);
        yield return new WaitForSeconds(despawnDelay);
        Destroy(gameObject);
    }

    private void Start()
    {
        // Initialize sound intensity controllers
        _positiveGoalIntensityController = new SoundIntensityController(positiveGoalIntensitySound, this);
        _negativeGoalIntensityController = new SoundIntensityController(negativeGoalIntensitySound, this);
        _missGoalIntensityController = new SoundIntensityController(missGoalIntensitySound, this);

        _positivePowerUpIntensityController = new SoundIntensityController(positivePowerUpIntensitySound, this);
        _negativePowerUpIntensityController = new SoundIntensityController(negativePowerUpIntensitySound, this);
    }

    private void Update()
    {
        UpdateDropperSound();
        UpdateGoalIntensitySound();
        UpdatePowerUpIntensitySound();
    }

    [SerializeField] private float maxGoalSoundRange = 5f;

    private void UpdateGoalIntensitySound()
    {
        foreach (var goal in goals)
        {
            var distance = Vector2.Distance(goal.transform.position, transform.position);

            if (distance <= maxGoalSoundRange)
            {
                var normalizedDistance = Mathf.Clamp01(distance / maxGoalSoundRange);

                if (goal.scoreValue > 0)
                {
                    _positiveGoalIntensityController.SetIntensity(normalizedDistance, Time.timeSinceLevelLoad);
                }
                else if (goal.scoreValue < 0)
                {
                    _negativeGoalIntensityController.SetIntensity(normalizedDistance, Time.timeSinceLevelLoad);
                }
                else
                {
                    _missGoalIntensityController.SetIntensity(normalizedDistance, Time.timeSinceLevelLoad);
                }
            }
            else
            {
                // Fade out sounds for goals that are out of range
                if (goal.scoreValue > 0)
                {
                    _positiveGoalIntensityController.Stop();
                }
                else if (goal.scoreValue < 0)
                {
                    _negativeGoalIntensityController.Stop();
                }
                else
                {
                    _missGoalIntensityController.Stop();
                }
            }
        }
    }


    private void UpdatePowerUpIntensitySound()
    {
        var closestPowerUp = powerUps.OrderBy(powerUp => Vector2.Distance(powerUp.transform.position, transform.position)).FirstOrDefault();
        if (closestPowerUp == null) return;

        float distance = Vector2.Distance(transform.position, closestPowerUp.transform.position);

        if (closestPowerUp.CompareTag("PositivePowerUp"))
        {
            _positivePowerUpIntensityController.SetIntensity(distance, Time.timeSinceLevelLoad);
            _negativePowerUpIntensityController.Stop();
        }
        else if (closestPowerUp.CompareTag("NegativePowerUp"))
        {
            _negativePowerUpIntensityController.SetIntensity(distance, Time.timeSinceLevelLoad);
            _positivePowerUpIntensityController.Stop();
        }
    }

    private void UpdateDropperSound()
    {
        // Adjust dropper sound pitch and volume based on ball velocity
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null || dropperSound == null) return;

        float speed = rb.velocity.magnitude;
        dropperSound.volume = Mathf.Clamp01(speed / 10f); // Adjust volume based on speed
        dropperSound.pitch = Mathf.Clamp(speed / 10f, 0.8f, 1.2f); // Adjust pitch slightly based on speed
    }
}
