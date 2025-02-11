using System.Collections;
using UnityEngine;

public class PachinkoPinballLauncher : MonoBehaviour
{
    [Header("Launcher Settings")]
    [SerializeField] private Transform plunger;
    [SerializeField] private Transform launchPosition;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float minPower = 15f;
    [SerializeField] private float maxPower = 35f;
    [SerializeField] private float chargeRate = 25f; // Balanced speed
    [SerializeField] private float chargeAcceleration = 1.1f;
    [SerializeField] private float plungerTravelDistance = 0.3f;
    [SerializeField] private float plungerResetSpeed = 0.1f;
    [SerializeField] private AnimationCurve plungerCurve;

    [Header("Curve Assistance (For Hugging the Wall)")]
    [SerializeField] private float curveCorrectionForce = 8f; // Stronger to keep it hugging the wall
    [SerializeField] private float downwardForceOnCurve = 3f; // Prevents bouncing at the top

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
    private Vector3 _plungerStartPos;

    private void Start()
    {
        _plungerStartPos = plunger.position;
        SpawnBall();
    }

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

        float chargeSpeed = chargeRate;

        while (_isCharging)
        {
            chargeSpeed *= chargeAcceleration;
            _currentPower += chargeSpeed * Time.deltaTime;
            _currentPower = Mathf.Clamp(_currentPower, minPower, maxPower);

            if (chargeSound != null)
            {
                chargeSound.pitch = chargePitchCurve.Evaluate(_currentPower / maxPower);
            }

            // Smooth plunger pullback
            float chargePercent = (_currentPower - minPower) / (maxPower - minPower);
            plunger.position = _plungerStartPos - new Vector3(0, plungerTravelDistance * plungerCurve.Evaluate(chargePercent), 0);

            yield return null;
        }
    }

    private void ReleaseBall()
    {
        if (!_isCharging) return;
        _isCharging = false;
        _canLaunch = false;

        GameResultsManager.ReduceBallCount();

        if (_chargeRoutine != null)
        {
            StopCoroutine(_chargeRoutine);
        }

        if (chargeSound) chargeSound.Stop();
        if (launchSound) launchSound.Play();

        StartCoroutine(PlungerLaunch());
    }

    private IEnumerator PlungerLaunch()
    {
        Vector3 targetPos = _plungerStartPos;
        Vector3 launchPos = plunger.position;

        float elapsed = 0f;
        while (elapsed < plungerResetSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / plungerResetSpeed;
            plunger.position = Vector3.Lerp(launchPos, targetPos, t * t);
            yield return null;
        }
        plunger.position = targetPos;

        if (_currentBall != null)
        {
            _ballRb.velocity = Vector2.up * _currentPower;
            StartCoroutine(ApplyCurveCorrection(_currentPower));
        }

        StartCoroutine(WaitAndSpawnNewBall());
    }

    /// <summary>
    /// Applies forces to keep the ball on the wall and prevent bouncing at the top.
    /// </summary>
    private IEnumerator ApplyCurveCorrection(float launchPower)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (_ballRb != null)
            {
                _ballRb.AddForce(-Vector2.right * curveCorrectionForce, ForceMode2D.Force);
                if (_ballRb.velocity.y > 0)
                {
                    _ballRb.AddForce(Vector2.down * downwardForceOnCurve, ForceMode2D.Force);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator WaitAndSpawnNewBall()
    {
        yield return new WaitForSeconds(0.2f);

        while (GameResultsManager.ballCount > 0 && _currentBall != null && _currentBall.transform.position.y < launchPosition.position.y + 1f)
        {
            yield return null;
        }

        _canLaunch = true;
        SpawnBall();
    }

    private void SpawnBall()
    {

        if (GameResultsManager.ballCount <= 0)
        {
            _canLaunch = false;
            _currentBall = null;
            _ballRb = null;
            GameResultsManager.GameOver(false);
            return;
        }
        _currentBall = Instantiate(ballPrefab, launchPosition.position, Quaternion.identity);
        _ballRb = _currentBall.GetComponent<Rigidbody2D>();
    }
}
