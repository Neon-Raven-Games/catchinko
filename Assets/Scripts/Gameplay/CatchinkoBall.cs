using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class SoundIntensityController
{
    private readonly AudioSource _audioSource;
    private readonly MonoBehaviour _catchinkoBall;

    [SerializeField] private AnimationCurve volumeOverDistance = AnimationCurve.Linear(0.1f, 0, 1, 1);
    [SerializeField] private AnimationCurve pitchOverDistance = AnimationCurve.Linear(0, 1.5f, 1, 1);

    public SoundIntensityController(AudioSource audioSource, MonoBehaviour zone)
    {
        _audioSource = audioSource;
        _catchinkoBall = zone;
    }

    public void Stop()
    {
        if (_audioSource.isPlaying) _catchinkoBall.StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float fadeDuration = 0.5f;
        float startVolume = _audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        _audioSource.volume = 0;
        _audioSource.Stop();
        _audioSource.pitch = 1;
    }

    public void Collect()
    {
        _catchinkoBall.StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float fadeDuration = 0.2f;
        float startVolume = _audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        _audioSource.volume = 0;
        _audioSource.Stop();
    }

    public void SetIntensity(float distance)
    {
        if (!_audioSource.isPlaying) _audioSource.Play();

        var distanceVolume = volumeOverDistance.Evaluate(distance);
        _audioSource.volume = Mathf.Clamp01(distanceVolume);
        var distancePitch = pitchOverDistance.Evaluate(distance);
        _audioSource.pitch = Mathf.Max(0.1f, distancePitch);
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

    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private AudioSource spawnSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource dieSound;
    [SerializeField] private AudioSource missSound;

    [Header("Sound Intensity")] 

    // todo, powerups should have their own for oscillation
    [SerializeField] private List<GameObject> powerUps;
    [SerializeField] private AudioSource positivePowerUpIntensitySound;
    [SerializeField] private AudioSource negativePowerUpIntensitySound;
    private SoundIntensityController _positivePowerUpIntensityController;
    private SoundIntensityController _negativePowerUpIntensityController;

    [SerializeField] private float explosionSoundDelay = 0.15f;
    
    private bool _stopped;
    private List<GoalZone> _goals;

    Rigidbody2D rb;
    public void Despawn(GoalType goal) => StartCoroutine(DespawnRoutine(goal));

    [SerializeField] private float hitSoundFactor = 25;
    private void OnCollisionEnter2D(Collision2D col)
    {
        hitSound.volume = Mathf.Clamp01(col.relativeVelocity.magnitude / hitSoundFactor);
        hitSound.Play();
    }
    
    [SerializeField] private GameObject spawnEffect;

    private void OnEnable()
    {
        // todo, extract this to a controller so we don't need to do this every time
        transform.localScale = Vector3.zero;
        spawnEffect.SetActive(true);
        
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() => 
            spawnEffect.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() => spawnEffect.gameObject.SetActive(false)));
        
        _goals = FindObjectsOfType<GoalZone>().ToList();
        spawnSound.Play();
    }

    private IEnumerator DespawnRoutine(GoalType goal)
    {
        if (goal == GoalType.Hit) attackSound.Play();
        else if (goal == GoalType.Die) dieSound.Play();
        else missSound.Play();
       
        DeactivateIntensityControllers();
        yield return new WaitForSeconds(smokeDelay);

        smoke.transform.localScale = Vector3.zero;
        smoke.SetActive(true);
        smoke.transform.DOScale(0.06f, explosionDelay).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(explosionDelay);
        explosion.SetActive(true);
        smoke.transform.DOScale(Vector3.zero, despawnDelay).SetEase(Ease.InBack);
        yield return new WaitForSeconds(explosionSoundDelay);
        explosionSound.Play();
        transform.DOScale(Vector3.zero, despawnDelay).SetEase(Ease.InBack);
        yield return new WaitForSeconds(despawnDelay);
        Destroy(gameObject);
    }


    private void DeactivateIntensityControllers()
    {
        _stopped = true;
        dropperSound.Stop();
        StopGoalIntensities();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _positivePowerUpIntensityController = new SoundIntensityController(positivePowerUpIntensitySound, this);
        _negativePowerUpIntensityController = new SoundIntensityController(negativePowerUpIntensitySound, this);
    }

    private void StopGoalIntensities()
    {
        foreach (var goal in _goals)
        {
            goal.Stop();
        }
    }
    private void Update()
    {
        if (_stopped) return;
        UpdateDropperSound();
        UpdateGoalIntensitySound();
        UpdatePowerUpIntensitySound();
    }

    [SerializeField] private float maxGoalSoundRange = 5f;

    private void UpdateGoalIntensitySound()
    {
        foreach (var goal in _goals)
        {
            var distance = Vector2.Distance(goal.transform.position, transform.position);

            if (distance <= maxGoalSoundRange)
            {
                var normalizedDistance = 1 - Mathf.Clamp01(distance / maxGoalSoundRange);
                goal.SetIntensity(normalizedDistance);
            }
            else
            {
                goal.Stop();
            }
        }
    }


    private void UpdatePowerUpIntensitySound()
    {
        var closestPowerUp = powerUps
            .OrderBy(powerUp => Vector2.Distance(powerUp.transform.position, transform.position)).FirstOrDefault();
        if (closestPowerUp == null) return;

        float distance = Vector2.Distance(transform.position, closestPowerUp.transform.position);

        if (closestPowerUp.CompareTag("PositivePowerUp"))
        {
            _positivePowerUpIntensityController.SetIntensity(distance);
            _negativePowerUpIntensityController.Stop();
        }
        else if (closestPowerUp.CompareTag("NegativePowerUp"))
        {
            _negativePowerUpIntensityController.SetIntensity(distance);
            _positivePowerUpIntensityController.Stop();
        }
    }


    [SerializeField] private float dropperSoundVelocityFactor = 10f;
    [SerializeField] private float dropperSoundMaxVolume = 0.5f;

    private void UpdateDropperSound()
    {
        if (rb == null || dropperSound == null) return;

        if (!dropperSound.isPlaying) dropperSound.Play();
        float speed = -rb.velocity.y;
        dropperSound.volume = Mathf.Clamp(Mathf.Clamp01(speed / dropperSoundVelocityFactor), 0, dropperSoundMaxVolume);
        dropperSound.pitch = Mathf.Clamp(speed / dropperSoundVelocityFactor, 0.8f, 1.2f);
    }
}