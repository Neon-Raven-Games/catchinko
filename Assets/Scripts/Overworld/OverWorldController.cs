using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Overworld;

public class OverWorldController : MonoBehaviour
{
    [SerializeField] private List<OverWorldInnerLevel> innerOverWorldLevels;
    [SerializeField] private Camera overWorldCamera;
    [SerializeField] private Rect overWorldBounds;
    [SerializeField] private float zoomMin = 5f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private LayerMask levelLayerMask;
    [SerializeField] private AnimatedMenu animatedMenu;

    private int _currentIndex;
    private Vector3 _dragOrigin;

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

        // Kill any existing tweens to prevent interruptions
        _movementTween?.Kill();
        _zoomTween?.Kill();

        // Get target position and bounds
        var targetPosition = OverWorldGameManager.GetLastCameraPosition(targetCatBoss);
        targetPosition.Item1.z = -8; // Set appropriate Z position

        // Get the map bounds for the boss level
        Rect mapBounds = OverWorldGameManager.GetBossMapBounds(targetCatBoss);

        // Calculate clamped ortho size (zoom) and final position
        float targetOrthoSize = Mathf.Clamp(
            targetPosition.Item2,
            zoomMin,
            Mathf.Min(mapBounds.height / 2f, mapBounds.width / (2f * overWorldCamera.aspect))
        );

        // Calculate the visible area of the camera at the target zoom level
        float cameraHeight = targetOrthoSize * 2f;
        float cameraWidth = cameraHeight * overWorldCamera.aspect;
        float halfCameraWidth = cameraWidth / 2f;
        float halfCameraHeight = targetOrthoSize;

        // Clamp the target position to ensure it stays within the bounds
        Vector3 clampedPosition = targetPosition.Item1;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, mapBounds.xMin + halfCameraWidth,
            mapBounds.xMax - halfCameraWidth);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, mapBounds.yMin + halfCameraHeight,
            mapBounds.yMax - halfCameraHeight);

        // Start the movement tween using the clamped position and size
        _movementTween = overWorldCamera.transform.DOMove(clampedPosition, 1f)
            .SetEase(Ease.InOutQuad)
            .OnStart(OverWorldGameManager.SetTravelling)
            .OnComplete(() =>
            {
                OverWorldGameManager.SetCurrentBoss(targetCatBoss);
                overWorldBounds = OverWorldGameManager.GetBossMapBounds(targetCatBoss);
                OverWorldGameManager.SetPlayerToProperPosition();
            });

        // Tween the zoom level to the clamped ortho size
        _zoomTween = overWorldCamera.DOOrthoSize(targetOrthoSize, 1f)
            .SetEase(Ease.InOutQuad);
    }


    private void Update()
    {
        if (!overWorldCamera) overWorldCamera = Camera.main;
        if (animatedMenu.State != MenuState.Closed) return;

        HandlePan();
        HandleZoom();
        HandleClickLevel();
        HandleButtonLevel();
    }

    private static void HandleButtonLevel()
    {
        if (OverWorldGameManager.CurrentLevel == null) return;
        var curLevelData = OverWorldGameManager.CurrentLevel.data;

        if (Input.GetKeyUp(KeyCode.W) && curLevelData.northNeighbor != null)
            OverWorldGameManager.ChangeOverWorldLevel(curLevelData.northNeighbor);

        if (Input.GetKeyUp(KeyCode.S) && curLevelData.southNeighbor != null)
            OverWorldGameManager.ChangeOverWorldLevel(curLevelData.southNeighbor);

        if (Input.GetKeyUp(KeyCode.A) && curLevelData.westNeighbor != null)
            OverWorldGameManager.ChangeOverWorldLevel(curLevelData.westNeighbor);

        if (!Input.GetKeyUp(KeyCode.D) || !curLevelData.eastNeighbor) return;
        OverWorldGameManager.ChangeOverWorldLevel(curLevelData.eastNeighbor);
    }

    private void HandleClickLevel()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouseWorldPosition = overWorldCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, levelLayerMask);

            if (!hit.collider) return;

            var clickedLevel = hit.collider.GetComponent<OverWorldInnerLevel>();
            if (clickedLevel != null) OverWorldGameManager.ChangeOverWorldLevel(clickedLevel);
        }
    }

    private void OnEnable()
    {
        overWorldCamera = Camera.main;
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
            _dragOrigin = overWorldCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (!Input.GetMouseButton(1)) return;

        var difference = _dragOrigin - overWorldCamera.ScreenToWorldPoint(Input.mousePosition);
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