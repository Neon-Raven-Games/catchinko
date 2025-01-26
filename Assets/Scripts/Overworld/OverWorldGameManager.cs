using System.Collections.Generic;
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
            if (level != CurrentLevel && CurrentLevel.CanTravelTo(level))
            {
                TravelToNeighbor(level);
                Debug.Log("Level changed to: " + level.name);
                _currentLevel = level;
            }
        }

        private static void TravelToNeighbor(OverWorldInnerLevel neighbor)
        {
            // make the sounds on the player object and in the travel to method.
            if (neighbor.data.unlocked)
            {
                // travel the player to neighbor

                // set travelling(true)
                // player.TravelTo(neighbor.transform.position);
                // when player finishes, we set travelling(false) from there
                //
                // then on player, play a sound
                // audioSource.PlayOneShot(validMoveSound);
            }
            else
            {
                // nudge the player
                // player.Nudge();
                // call sound from player
                // audioSource.PlayOneShot(invalidMoveSound);
            }
        }

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

        // gets the last player camera pos
        // we need to update the last player level when moving, and scroll this way
        // if we get off the map bounds, we can make a galaxy bg or something
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
    }
}