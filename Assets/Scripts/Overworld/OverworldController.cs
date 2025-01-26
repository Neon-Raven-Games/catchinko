using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Overworld;
using UnityEngine.Serialization;

public class OverworldController : MonoBehaviour
{
    private int LevelCount => overWorldLevels.Count;

    [SerializeField] private List<GameObject> overWorldLevels;
    [SerializeField] private List<OverworldLevel> innerOverWorldLevels;
    [FormerlySerializedAs("overworldCamera")] 
    [SerializeField] private Camera overWorldCamera;
    [SerializeField] private Transform background;
    
    [SerializeField] private Rect overWorldBounds;
    
    
    [SerializeField] private float zoomMin = 5f;
    [SerializeField] private float zoomMax = 20f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private LayerMask levelLayerMask; // Layer mask for levels

    [SerializeField] private AnimatedMenu animatedMenu;

    private Vector3 dragOrigin; // Starting point of the drag
    
    private Vector3 _startPosition;
    private float _startOrthographicSize;
    
    private Tween _movementTween;

    private int _currentIndex = 0;

    private void Start()
    {
        if (overWorldCamera != null) _startPosition = overWorldCamera.transform.position;
        overWorldBounds = GetRectFromTransform(background);
        _startOrthographicSize = overWorldCamera.orthographicSize;
        OverWorldGameManager.SetCurrentLevel(innerOverWorldLevels[0]);
    }
    private Rect GetRectFromTransform(Transform target)
    {
        // Check if the target has a SpriteRenderer
        var spriteRenderer = target.GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            var size = spriteRenderer.size;
            size.x *= target.transform.localScale.x;
            size.y *= target.transform.localScale.y;
            
            Vector2 position = target.position;
            var xMin = position.x - size.x / 2;
            var yMin = position.y - size.y / 2;

            return new Rect(xMin, yMin, size.x, size.y);
        }

        Debug.LogWarning("Transform does not have a SpriteRenderer. Using default bounds.");
        return new Rect(-50, -50, 100, 100);
    }
    
    /// <summary>
    /// Scrolls to the specified level index using DoTween.
    /// </summary>
    /// <param name="index">The target level index to scroll to.</param>
    public void TravelToLevel(int index)
    {
        // hardlocks the camera transitioning to menu state being open.
        if (animatedMenu.State != MenuState.Open) return;
        
        if (index < 0 || index >= LevelCount)
        {
            Debug.LogError("Invalid level index.");
            return;
        }

        _movementTween?.Kill();
        _zoomTween?.Kill();
        var targetPosition = _startPosition + new Vector3(0, index * 10f, 0);

        _movementTween = overWorldCamera.transform.DOMove(targetPosition, 1f)
            .SetEase(Ease.InOutQuad)
            .OnStart(() =>
            {
                // blur effects maybe?
            }).OnComplete(() => { _currentIndex = index; });
        
        _zoomTween = overWorldCamera.DOOrthoSize(_startOrthographicSize, 1f)
            .SetEase(Ease.InOutQuad);
    }
    
    private Tween _zoomTween;

    private void GoUpLevel()
    {
        if (LevelCount > _currentIndex + 1) TravelToLevel(_currentIndex + 1);
    }

    private void GoDownLevel()
    {
        if (_currentIndex - 1 >= 0) TravelToLevel(_currentIndex - 1);
    }

    private void Update()
    {
        // hard lock controls to menu state being closed.
        if (animatedMenu.State != MenuState.Closed) return;
        HandlePan();
        HandleZoom();
        HandleClickLevel();
        HandleButtonLevel();
    }

    private void HandleButtonLevel()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            if (innerOverWorldLevels[_currentIndex].data.northNeighbor != null)
            {
                Debug.Log(innerOverWorldLevels[_currentIndex].data.northNeighbor.data.index);
            }
            else
            {
                Debug.Log("No north neighbor found");
            }
        }
        
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (innerOverWorldLevels[_currentIndex].data.southNeighbor != null)
            {
                Debug.Log(innerOverWorldLevels[_currentIndex].data.southNeighbor.data.index);
            }
            else
            {
                Debug.Log("No south neighbor found");
            }
        }
        
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (innerOverWorldLevels[_currentIndex].data.westNeighbor != null)
            {
                Debug.Log(innerOverWorldLevels[_currentIndex].data.westNeighbor.data.index);
            }
            else
            {
                Debug.Log("No west neighbor found");
            }
        }
        
        if (Input.GetKeyUp(KeyCode.D))
        {
            
            if (innerOverWorldLevels[_currentIndex].data.eastNeighbor == null)
            {
                Debug.Log("No east neighbor found");
                return;
            }
            Debug.Log(innerOverWorldLevels[_currentIndex].data.eastNeighbor.data.index);
        }
        
    }
    
    private void HandleClickLevel()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Convert the mouse position to world space
            Vector3 mouseWorldPosition = overWorldCamera.ScreenToWorldPoint(Input.mousePosition);

            // Perform the raycast
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, levelLayerMask);

            // Check if the raycast hit something
            if (hit.collider != null)
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                // Attempt to get the OverworldLevel component
                var clickedLevel = hit.collider.GetComponent<OverworldLevel>();
                if (clickedLevel != null)
                {
                    Debug.Log("Clicked on level: " + clickedLevel.name);

                    // Trigger the game manager's level change logic
                    if (OverWorldGameManager.CurrentLevel != clickedLevel)
                    {
                        OverWorldGameManager.ChangeOverWorldLevel(clickedLevel);
                    }
                }
            }
        }
    }

    private void HandleZoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            var newSize = overWorldCamera.orthographicSize - scroll * zoomSpeed;
            newSize = Mathf.Clamp(newSize, zoomMin, zoomMax);

            var cameraHeight = newSize * 2f; // Total height of the camera's view
            var cameraWidth = cameraHeight * overWorldCamera.aspect; // Total width of the camera's view

            var halfCameraWidth = cameraWidth / 2f;
            var halfCameraHeight = newSize;

            var cameraPosition = overWorldCamera.transform.position;

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, overWorldBounds.xMin + halfCameraWidth, overWorldBounds.xMax - halfCameraWidth);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, overWorldBounds.yMin + halfCameraHeight, overWorldBounds.yMax - halfCameraHeight);

            overWorldCamera.orthographicSize = newSize;
            overWorldCamera.transform.position = cameraPosition;
        }
    }

    private void HandlePan()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = overWorldCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (!Input.GetMouseButton(1)) return;
        
        var difference = dragOrigin - overWorldCamera.ScreenToWorldPoint(Input.mousePosition);
        var newPosition = overWorldCamera.transform.position + difference;

        var cameraHeight = overWorldCamera.orthographicSize * 2f;
        var cameraWidth = cameraHeight * overWorldCamera.aspect;

        var halfCameraWidth = cameraWidth / 2f;
        var halfCameraHeight = overWorldCamera.orthographicSize;

        newPosition.x = Mathf.Clamp(newPosition.x, overWorldBounds.xMin + halfCameraWidth, overWorldBounds.xMax - halfCameraWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, overWorldBounds.yMin + halfCameraHeight, overWorldBounds.yMax - halfCameraHeight);

        overWorldCamera.transform.position = newPosition;
    }

    /// <summary>
    /// Example method to dynamically test transitions.
    /// </summary>
    public void TestDynamicTransitions()
    {
        StartCoroutine(TestTravelSequence());
    }

    /// <summary>
    /// Whenever we get more levels, lets stress test hard here.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TestTravelSequence()
    {
        TravelToLevel(1);
        yield return new WaitForSeconds(0.5f); // Test interrupt after 0.5 seconds
        TravelToLevel(2);
        yield return new WaitForSeconds(1f); // Allow transition to finish
        TravelToLevel(0);
        yield return new WaitForSeconds(0.2f);
        TravelToLevel(2);
        yield return new WaitForSeconds(0.5f);
        TravelToLevel(0);
    }
}