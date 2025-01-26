using UnityEngine;

namespace Overworld
{
    public class OverWorldGameManager : MonoBehaviour
    {
        private static OverWorldGameManager _instance;
        
        private static OverworldLevel _currentLevel;
        public static OverworldLevel CurrentLevel => _currentLevel;
        
        public static void ChangeOverWorldLevel(OverworldLevel level)
        {
            if (level != CurrentLevel && CurrentLevel.CanTravelTo(level))
            {
                TravelToNeighbor(level);
                Debug.Log("Level changed to: " + level.name);
                _currentLevel = level;
                
            }
        }
        private static void TravelToNeighbor(OverworldLevel neighbor)
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
        
        public static void SetTravelling(bool isTravelling)
        {
            _currentLevel = null;
        }
        
        public static void UnlockLevel(OverworldLevel level)
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
        }


        public static void SetCurrentLevel(OverworldLevel level)
        {
            _currentLevel = level;
            Debug.Log("Setting current level: " + level.data.index);
        }
    }
}