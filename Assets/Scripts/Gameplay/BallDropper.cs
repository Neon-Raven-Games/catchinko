using System.Collections;
using Gameplay;
using UnityEngine;

public enum GoalType
{
    Miss,
    Die,
    Hit
}

public class BallDropper : MonoBehaviour
{
    [SerializeField] private GameObject catBallPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private float ballSpawnDelay = 1f;
    [SerializeField] private float initialDropForce = 2f;

    [SerializeField] private float pathWidth = 5f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private GoalShuffler goalShuffler;
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CatchinkoBall _catBall;
    private Rigidbody2D _currentCatBallRb2d;
    private bool _isDropping = false;

    private Vector2 _leftEndpoint;
    private Vector2 _rightEndpoint;

    private float _movementTime = 0f;

    private void Start()
    {
        _leftEndpoint = new Vector2(ballSpawnPoint.position.x - pathWidth / 2f, ballSpawnPoint.position.y);
        _rightEndpoint = new Vector2(ballSpawnPoint.position.x + pathWidth / 2f, ballSpawnPoint.position.y);

        SpawnBall();
    }

    private void Update()
    {
        if (_isDropping) return;

        _movementTime += Time.deltaTime * movementSpeed;
        var t = Mathf.PingPong(_movementTime, 1f);
        var easedT = easingCurve.Evaluate(t);
        var targetX = Mathf.Lerp(_leftEndpoint.x, _rightEndpoint.x, easedT);

        if (_catBall != null)
        {
            var targetPosition = new Vector2(targetX, ballSpawnPoint.position.y);
            _currentCatBallRb2d.MovePosition(targetPosition);
        }

        if (Input.GetMouseButtonDown(0)) DropBall();
    }

    private void DropBall()
    {
        if (_isDropping || _currentCatBallRb2d == null) return;

        GameResultsManager.ReduceBallCount();
        _isDropping = true;
        _currentCatBallRb2d.constraints = RigidbodyConstraints2D.None;
        _currentCatBallRb2d.velocity = new Vector2(0, 0);
        _currentCatBallRb2d.AddForce(new Vector2(0, -initialDropForce), ForceMode2D.Impulse);
    }

    private void SpawnBall()
    {
        if (GameResultsManager.IsGameOver) return;

        _catBall = Instantiate(catBallPrefab, ballSpawnPoint.position, Quaternion.identity)
            .GetComponent<CatchinkoBall>();
        _currentCatBallRb2d = _catBall.GetComponent<Rigidbody2D>();
        _currentCatBallRb2d.constraints =
            RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        _isDropping = false;
        _movementTime = 0f;
    }

    [SerializeField] private CatCharacter playerCharacter;

    public void ScoreSpawn(GoalType goal)
    {
        StartCoroutine(DelayBallSpawn(goal));
    }

    private IEnumerator DelayBallSpawn(GoalType goal)
    {
        if (spawning) yield break;
        spawning = true;
        if (goal == GoalType.Hit) playerCharacter.Attack();
        else if (goal == GoalType.Die) GameResultsManager.ReduceBallCount();

        if (_catBall != null) PlayDespawnEffectAndDestroy(goal);
        yield return new WaitForSeconds(ballSpawnDelay);

        goalShuffler.SpawnGoalsWeighted();

        yield return null;
        SpawnBall();
        spawning = false;
    }

    private bool spawning;

    private void PlayDespawnEffectAndDestroy(GoalType goal)
    {
        _catBall.Despawn(goal);
    }

    private void OnDrawGizmos()
    {
        if (ballSpawnPoint == null) return;

        var leftEndpoint = new Vector2(ballSpawnPoint.position.x - pathWidth / 2f, ballSpawnPoint.position.y);
        var rightEndpoint = new Vector2(ballSpawnPoint.position.x + pathWidth / 2f, ballSpawnPoint.position.y);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftEndpoint, rightEndpoint); // Draw the path line

        Gizmos.DrawSphere(leftEndpoint, 0.1f);
        Gizmos.DrawSphere(rightEndpoint, 0.1f);
    }
}