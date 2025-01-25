using UnityEngine;

public class PachinkoCannon : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float maxLaunchForce = 10f;
    [SerializeField] private float minLaunchForce = 2f;
    [SerializeField] private float timeToMaxLaunchForce = 2f;
    [SerializeField] private Vector2 angleClamp = new(-45, 45);

    private float _launchForce;
    
    private void Update()
    {
        PointAtMouse();

        // todo, make game fully support controllers
        // ui to match controller input and mouse.
        // keep generic like this:
        
        if (Input.GetMouseButton(0)) UpdateLaunchForce();
        if (!Input.GetMouseButtonUp(0)) return;

        LaunchBall();
    }

    private void LaunchBall()
    {
        var ball = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);
        var rb = ball.GetComponent<Rigidbody2D>();
        rb.AddForce(launchPoint.up * _launchForce, ForceMode2D.Impulse);
        _launchForce = minLaunchForce;
    }

    private void PointAtMouse()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = mousePos - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(angle + 90, angleClamp.x, angleClamp.y));
    }

    private void UpdateLaunchForce() =>
        _launchForce = Mathf.Lerp(_launchForce, maxLaunchForce, Time.deltaTime / timeToMaxLaunchForce);
}