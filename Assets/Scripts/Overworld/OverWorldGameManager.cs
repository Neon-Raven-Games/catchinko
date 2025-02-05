using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Overworld
{
    public class OverWorldGameManager : MonoBehaviour
    {
        public static OverWorldInnerLevel CurrentLevel => _currentLevel;
        
        // todo, this is public because the editor script needed it.
        [SerializeField] public List<OverWorldMapData> levels;
        [SerializeField] private AudioSource walkingAudioSource;
        [SerializeField] private AudioSource invalidMoveSound;
        
        [SerializeField] private GameObject uIObject;
        [SerializeField] private GameObject overWorldObject;
        public OverWorldPlayer player;
        

        private static readonly Dictionary<CatBoss, OverWorldMapData> _levelDictionary = new();
        private static CatBoss _currentBoss;
        private static OverWorldInnerLevel _currentLevel;
        private static OverWorldGameManager _instance;
        private static bool _pathing = false;

        
        public static void ChangeOverWorldLevel(OverWorldInnerLevel level)
        {
            if (level != CurrentLevel) TravelToNeighbor(level);
            else _instance.player.ShowPanel();
        }

        private static IEnumerator WalkPlayerToPath(List<OverWorldInnerLevel> levels, OverWorldPlayer player)
        {
            // advance two frames to ensure all coroutines are stopped, stop coroutine is trash
            if (_pathing)
            {
                _pathing = false;
                yield return null;
                yield return null;
            }
            _pathing = true;
            
            var lerpDuration = 1.5f;

            // todo, temp fix. We should not include the first in the collection
            levels.RemoveAt(0);

            foreach (var level in levels)
            {
                var startPos = player.transform.position;
                var endPos = level.transform.position;
                float elapsedTime = 0;

                while (elapsedTime < lerpDuration)
                {
                    if (_pathing == false) break;
                    
                    player.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / lerpDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                if (_pathing == false) break;
                player.transform.position = endPos;
            }

            if (_pathing) player.ShowPanel();
        }

        private static void TravelToNeighbor(OverWorldInnerLevel neighbor)
        {
            var path = OverWorldPathfinder.FindPath(_currentLevel, neighbor);
            
            if (path.Count > 1)
            {
                _instance.player.HidePanel();
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
            _instance.player.transform.DOShakePosition(0.05f, 0.1f, 1);
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

        // can I repopulate an object whenever the build index of the scene is 0 and is loaded?
        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (var level in levels) _levelDictionary.Add(level.boss, level);
            _currentLevel = _levelDictionary[_currentBoss].lastPlayerLevel;
        }


        private void OnSceneLoaded(Scene curScene, LoadSceneMode loadMode)
        {
            if (curScene.buildIndex == 0)
            {
                if (GameResultsManager.lastGameWon) _currentLevel.Complete();
                player.gameObject.SetActive(true);
                uIObject.SetActive(true);
                overWorldObject.SetActive(true);
            }
            else
            {
                player.gameObject.SetActive(false);
                uIObject.SetActive(false);
                overWorldObject.SetActive(false);
            }
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

        public static (Vector3, float) GetLastCameraPosition(CatBoss boss)
        {
            _pathing = false;
            _instance.player.HidePanel();
            
            return (_levelDictionary[boss].lastPlayerLevel.transform.position,
                _levelDictionary[boss].initialCameraOrthoSize);
        }

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
            _pathing = false;
            _instance.StopCoroutine("WalkPlayerToPath");
            
            _instance.player.ShowPanel();
            _instance.player.transform.position = _levelDictionary[_currentBoss].lastPlayerLevel.transform.position;
        }
    }
}