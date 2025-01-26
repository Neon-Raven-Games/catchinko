using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Overworld
{
    [Serializable]
    public class OverWorldLevelData
    {
        public int index;

        public OverworldLevel eastNeighbor;
        public OverworldLevel westNeighbor;
        public OverworldLevel northNeighbor;
        public OverworldLevel southNeighbor;

        public bool unlocked;
        public bool completed;
    }

    public class OverworldLevel : MonoBehaviour, IPointerUpHandler
    {
        public OverWorldLevelData data;

        public void Unlock()
        {
            data.unlocked = true;
            SetVisualsUnlocked();
        }

        private void SetVisualsUnlocked()
        {
        }

        public void Complete()
        {
            if (data.northNeighbor != null) data.northNeighbor.Unlock();
            if (data.southNeighbor != null) data.southNeighbor.Unlock();
            if (data.eastNeighbor != null) data.eastNeighbor.Unlock();
            if (data.westNeighbor != null) data.westNeighbor.Unlock();
            data.completed = true;
            SetVisualsCompleted();
        }

        private void SetVisualsCompleted()
        {
        }

        public bool CanTravelTo(OverworldLevel level) => level.data.unlocked &&
                                                         (level == data.eastNeighbor || level == data.westNeighbor ||
                                                          level == data.northNeighbor || level == data.southNeighbor);

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Clicked on level from event handler: " + data.index);
        }
    }
}