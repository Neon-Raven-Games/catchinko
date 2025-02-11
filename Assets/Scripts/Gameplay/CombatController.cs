using System.Collections;
using Gameplay;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    private static CombatController _instance;

    [Header("Basic Enemy Attack")] [SerializeField]
    private Vector2 enemyAttackSpeedRange = new Vector2(1.5f, 3);

    [SerializeField] private CatCharacter enemyCharacter;
    private bool _playing;

    private IEnumerator EnemyAttackRoutine()
    {
        while (_playing)
        {
            var time = Random.Range(enemyAttackSpeedRange.x, enemyAttackSpeedRange.y);
            yield return new WaitForSeconds(time);
            enemyCharacter.Attack();
        }
    }

    private void Start()
    {
        if (!_instance) _instance = this;
        else Destroy(gameObject);

        _playing = true;
        StartCoroutine(EnemyAttackRoutine());
        SetCam();
    }

    [SerializeField] private SpriteRenderer background;

    private void SetCam()
    {
        var size = background.size;
        Vector2 position = background.transform.position;
        var xMin = position.x - size.x / 2;
        var yMin = position.y - size.y / 2;

        var mapBounds = new Rect(xMin, yMin, size.x, size.y);
        
        var targetOrthoSize = Mathf.Min(mapBounds.height / 2f, mapBounds.width / (2f * Camera.main.aspect));
        var cameraHeight = targetOrthoSize * 2f;
        var cameraWidth = cameraHeight * Camera.main.aspect;
        var halfCameraWidth = cameraWidth / 2f;
        var halfCameraHeight = targetOrthoSize;

        var clampedPosition = (Vector3) position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, mapBounds.xMin + halfCameraWidth,
            mapBounds.xMax - halfCameraWidth);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, mapBounds.yMin + halfCameraHeight,
            mapBounds.yMax - halfCameraHeight);
        clampedPosition.z = -8f;
        
        Camera.main.transform.position = clampedPosition;
    }

    public static void EndGame()
    {
        _instance._playing = false;
        _instance.StopAllCoroutines();

        GameResultsManager.GameOver(!_instance.enemyCharacter.IsAlive);
    }
}