using UnityEngine;

namespace Overworld
{

    public class OverWorldInnerLevel : MonoBehaviour
    {
        public OverWorldInnerLevelData data;

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
    }
}