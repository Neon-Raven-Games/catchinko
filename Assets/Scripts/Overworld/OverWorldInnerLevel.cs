using System;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{

    public class OverWorldInnerLevel : MonoBehaviour
    {
        public OverWorldInnerLevelData data;
        private SpriteRenderer _spriteRenderer;

        public void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Unlock()
        {
            data.unlocked = true;
            SetVisualsUnlocked();
        }

        private void SetVisualsUnlocked()
        {
            _spriteRenderer.color = Color.blue;
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
            _spriteRenderer.color = Color.green;
        }
    }
}