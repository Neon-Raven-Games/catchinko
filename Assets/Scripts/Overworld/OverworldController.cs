using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Overworld;
using UnityEngine.Serialization;

public class OverworldController : MonoBehaviour
{
    [SerializeField] private List<OverWorldInnerLevel> innerOverWorldLevels;
    private int _currentIndex;

    [FormerlySerializedAs("overworldCamera")] [SerializeField]
    private Camera overWorldCamera;

    [SerializeField] private Rect overWorldBounds;


    [SerializeField] private float zoomMin = 5f;
    [SerializeField] private float zoomMax = 20f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private LayerMask levelLayerMask; // Layer mask for levels

    [SerializeField] private AnimatedMenu animatedMenu;

    private Vector3 dragOrigin; // Starting point of the drag

    private Tween _movementTween;
    private Tween _zoomTween;

    private void Start()
    {
        // todo, once serialized, populate this before default to first
        SetBossAndGetBounds(CatBoss.JesterClaw);
    }

    private void SetBossAndGetBounds(CatBoss boss)
    {
        OverWorldGameManager.SetCurrentBoss(boss);
        overWorldBounds = OverWorldGameManager.GetBossMapBounds(boss);
    }

    /// <summary>
    /// Scrolls to the specified level index using DoTween.
    /// </summary>
    /// <param name="targetCatBoss">The target level index to scroll to.</param>
    public void TravelToLevel(CatBoss targetCatBoss)
    {
        if (animatedMenu.State != MenuState.Open) return;

        _movementTween?.Kill();
        _zoomTween?.Kill();
        var targetPosition = OverWorldGameManager.GetLastCameraPosition(targetCatBoss);
        targetPosition.Item1.z = -8;

        // populate the inner levels and last index
        innerOverWorldLevels = OverWorldGameManager.GetInnerLevels(targetCatBoss);
        _currentIndex = OverWorldGameManager.GetLastPlayerLevelIndex(targetCatBoss);
        
        var target = targetCatBoss;
        _movementTween = overWorldCamera.transform.DOMove(targetPosition.Item1, 1f)
            .SetEase(Ease.InOutQuad)
            .OnStart(OverWorldGameManager.SetTravelling).OnComplete(() =>
            {
                OverWorldGameManager.SetCurrentBoss(target);
                overWorldBounds = OverWorldGameManager.GetBossMapBounds(target);
                OverWorldGameManager.SetPlayerToProperPosition();
            });

        _zoomTween = overWorldCamera.DOOrthoSize(targetPosition.Item2, 1f)
            .SetEase(Ease.InOutQuad);
    }

    private void Update()
    {
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
                OverWorldGameManager.SetLastPlayerLevel(innerOverWorldLevels[_currentIndex].data.northNeighbor);
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
                OverWorldGameManager.SetLastPlayerLevel(innerOverWorldLevels[_currentIndex].data.southNeighbor);
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
                OverWorldGameManager.SetLastPlayerLevel(innerOverWorldLevels[_currentIndex].data.westNeighbor);
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

            OverWorldGameManager.SetLastPlayerLevel(innerOverWorldLevels[_currentIndex].data.eastNeighbor);
            Debug.Log(innerOverWorldLevels[_currentIndex].data.eastNeighbor.data.index);
        }
    }

    private void HandleClickLevel()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouseWorldPosition = overWorldCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, levelLayerMask);

            if (hit.collider != null)
            {
                var clickedLevel = hit.collider.GetComponent<OverWorldInnerLevel>();
                if (clickedLevel != null)
                {
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

            var maxZoomOutSize = Mathf.Min(
                (overWorldBounds.height / 2f),
                (overWorldBounds.width / (2f * overWorldCamera.aspect))
            );

            // can we account for the bounds here? if it's too small for zoom out, we should not be able to scroll out
            newSize = Mathf.Clamp(newSize, zoomMin, maxZoomOutSize);

            var cameraHeight = newSize * 2f; // Total height of the camera's view
            var cameraWidth = cameraHeight * overWorldCamera.aspect; // Total width of the camera's view


            var halfCameraWidth = cameraWidth / 2f;
            var halfCameraHeight = newSize;

            var cameraPosition = overWorldCamera.transform.position;

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, overWorldBounds.xMin + halfCameraWidth,
                overWorldBounds.xMax - halfCameraWidth);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, overWorldBounds.yMin + halfCameraHeight,
                overWorldBounds.yMax - halfCameraHeight);

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

        newPosition.x = Mathf.Clamp(newPosition.x, overWorldBounds.xMin + halfCameraWidth,
            overWorldBounds.xMax - halfCameraWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, overWorldBounds.yMin + halfCameraHeight,
            overWorldBounds.yMax - halfCameraHeight);

        overWorldCamera.transform.position = newPosition;
    }
}