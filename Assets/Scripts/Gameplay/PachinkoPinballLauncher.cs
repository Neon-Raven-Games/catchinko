using System.Collections;
using UnityEngine;

public class PachinkoPinballLauncher : MonoBehaviour
{
    [Header("Launcher Settings")]
    [SerializeField] private Transform launchPosition;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float minPower = 5f;
    [SerializeField] private float maxPower = 25f;
    [SerializeField] private float chargeRate = 10f;
    [SerializeField] private float spawnDelay = 0.2f;

    [Header("Audio & Visuals")]
    [SerializeField] private AudioSource chargeSound;
    [SerializeField] private AudioSource launchSound;
    [SerializeField] private AnimationCurve chargePitchCurve;

    private bool _isCharging;
    private float _currentPower;
    private GameObject _currentBall;
    private Rigidbody2D _ballRb;
    
    private Coroutine _chargeRoutine;
    private bool _canLaunch = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            StartCharging();
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            ReleaseBall();
        }
    }

    private void StartCharging()
    {
        if (!_canLaunch) return;
        

        _isCharging = true;
        _currentPower = minPower;
        _chargeRoutine = StartCoroutine(ChargePower());
    }

    private IEnumerator ChargePower()
    {
        if (chargeSound) chargeSound.Play(); 

        while (_isCharging)
        {
            _currentPower += chargeRate * Time.deltaTime;
            _currentPower = Mathf.Clamp(_currentPower, minPower, maxPower);

            if (chargeSound != null)
            {
                chargeSound.pitch = chargePitchCurve.Evaluate(_currentPower / maxPower);
            }

            yield return null;
        }
    }

    private void ReleaseBall()
    {
        if (!_isCharging) return;
        _isCharging = false;
        _canLaunch = false; // Prevent spamming launches
        
        GameResultsManager.ReduceBallCount();
        // if (goal == GoalType.Hit) playerCharacter.Attack();
        // else if (goal == GoalType.Die) GameResultsManager.ReduceBallCount(); 
        
        if (_chargeRoutine != null)
        {
            StopCoroutine(_chargeRoutine);
        }

        if (chargeSound) chargeSound.Stop();
        if (launchSound) launchSound.Play();
        
        SpawnBall();
        
        _ballRb.AddForce(Vector2.up * _currentPower, ForceMode2D.Impulse);
        StartCoroutine(ResetLaunchCooldown());
        _currentBall = null;
        _ballRb = null;
    }

    private void SpawnBall()
    {
        _currentBall = Instantiate(ballPrefab, launchPosition.position, Quaternion.identity);
        _ballRb = _currentBall.GetComponent<Rigidbody2D>();
    }

    private IEnumerator ResetLaunchCooldown()
    {
        yield return new WaitForSeconds(spawnDelay);
        _canLaunch = true;
    }
}
