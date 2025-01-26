using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Overworld
{
    public class OverWorldGameManager : MonoBehaviour
    {
        [SerializeField] public List<OverWorldMapData> levels;
        private static readonly Dictionary<CatBoss, OverWorldMapData> _levelDictionary = new();
        private static CatBoss _currentBoss;
        private static OverWorldGameManager _instance;
        private static OverWorldInnerLevel _currentLevel;
        public static OverWorldInnerLevel CurrentLevel => _currentLevel;

        public static void ChangeOverWorldLevel(OverWorldInnerLevel level)
        {
            if (level != CurrentLevel)
            {
                TravelToNeighbor(level);
            }
        }

        public Transform player;

        private static IEnumerator WalkPlayerToPath(List<OverWorldInnerLevel> levels, Transform player)
        {
            var lerpDuration = 1.5f;

            // todo, temp fix. We should not include the first in the collection
            levels.RemoveAt(0);

            foreach (OverWorldInnerLevel level in levels)
            {
                Vector3 startPos = player.transform.position;
                Vector3 endPos = level.transform.position;
                float elapsedTime = 0;

                while (elapsedTime < lerpDuration)
                {
                    player.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / lerpDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                player.transform.position = endPos;
            }
        }

        public AudioSource walkingAudioSource;
        public AudioSource invalidMoveSound;

        private static void TravelToNeighbor(OverWorldInnerLevel neighbor)
        {
            var path = OverWorldPathfinder.FindPath(_currentLevel, neighbor);
            if (path.Count > 0)
            {
                _instance.StopCoroutine("WalkPlayerToPath");
                _instance.StartCoroutine(WalkPlayerToPath(path, _instance.player));
                _currentLevel = neighbor;
                ValidMoveFeedback();
                _levelDictionary[_currentBoss].lastPlayerLevel = neighbor;
            }
            else
            {
                InvalidMoveFeedback();
            }
        }

        #region move to feedback system

        // todo, make a feedback system

        /// <summary>
        /// Valid Move Feedbacks
        /// </summary>
        private static void ValidMoveFeedback()
        {
            if (_instance.walkingAudioSource) _instance.walkingAudioSource.Play();
        }

        /// <summary>
        /// Invalid Move Feedbacks
        /// </summary>
        private static void InvalidMoveFeedback()
        {
            _instance.player.DOShakePosition(0.25f);
            if (_instance.invalidMoveSound) _instance.invalidMoveSound.Play();
        }

        #endregion

        public static void SetTravelling()
        {
            _currentLevel = null;
        }

        public static void UnlockLevel(OverWorldInnerLevel level)
        {
            level.Unlock();
        }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            foreach (var level in levels) _levelDictionary.Add(level.boss, level);
            _currentLevel = _levelDictionary[_currentBoss].lastPlayerLevel;
        }

        public static CatBoss GetNextBoss()
        {
            var index = _levelDictionary[_currentBoss].boss;
            if ((int) index + 1 < _instance.levels.Count) return _levelDictionary[(CatBoss) ((int) index + 1)].boss;

            return CatBoss.JesterClaw;
        }

        public static CatBoss GetPreviousBoss()
        {
            var index = _levelDictionary[_currentBoss].boss;
            if ((int) index - 1 >= 0) return _levelDictionary[(CatBoss) ((int) index - 1)].boss;
            return CatBoss.JesterClaw;
        }

        public static (Vector3, float) GetInitialCameraPosition(CatBoss boss) =>
            (_levelDictionary[boss].initialCameraPosition.position,
                _levelDictionary[boss].initialCameraOrthoSize);

        public static (Vector3, float) GetLastCameraPosition(CatBoss boss) =>
            (_levelDictionary[boss].lastPlayerLevel.transform.position,
                _levelDictionary[boss].initialCameraOrthoSize);

        public static void SetLastPlayerLevel(OverWorldInnerLevel level) =>
            _levelDictionary[_currentBoss].lastPlayerLevel = level;

        public static int GetLastPlayerLevelIndex() =>
            _levelDictionary[_currentBoss].lastPlayerLevel.data.index;

        public static int GetLastPlayerLevelIndex(CatBoss boss) =>
            _levelDictionary[boss].lastPlayerLevel.data.index;

        public static Rect GetBossMapBounds(CatBoss boss)
        {
            var target = _levelDictionary[boss].backgroundSprite;
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

        public static void SetCurrentBoss(CatBoss boss)
        {
            if (_levelDictionary.ContainsKey(boss) && _levelDictionary[boss].unlocked)
            {
                _currentBoss = boss;
                _currentLevel = _levelDictionary[boss].lastPlayerLevel;
            }
        }

        public static CatBoss GetCurrentBoss()
        {
            return _currentBoss;
        }

        public static List<OverWorldInnerLevel> GetInnerLevels(CatBoss targetCatBoss)
        {
            return _levelDictionary[targetCatBoss].backgroundSprite.parent.GetComponent<OverWorldMap>().levels;
        }

        public static void SetPlayerToProperPosition()
        {
            _instance.player.position = _levelDictionary[_currentBoss].lastPlayerLevel.transform.position;
        }
    }
}