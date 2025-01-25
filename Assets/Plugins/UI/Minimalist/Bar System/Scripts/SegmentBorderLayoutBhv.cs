using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minimalist.Utility;

namespace Minimalist.Bar
{
    public class SegmentBorderLayoutBhv : UIElementBhv
    {
        // Public properties
        public Color Color
        {
            set
            {
                foreach (BorderBhv border in Borders)
                {
                    border.Color = value;
                }
            }
        }

        // Private properties
        private BorderBhv[] Borders => _borders == null || _borders.Length == 0 ? GetComponentsInChildren<BorderBhv>(true) : _borders;

        // Private fields
        private BorderBhv[] _borders;

        private void Awake()
        {
            _borders = Borders;
        }

        public void UpdateSegmentDelimeters(float capacity, bool isSegmented, float segmentAmount, float borderWidth)
        {
            float effectiveWidth = Width + borderWidth;

            for (int i = 0; i < Borders.Length; i++)
            {
                float currentAmount = (i + 1) * segmentAmount;

                Borders[i].SizeDelta = new Vector2(borderWidth, 0);

                if (currentAmount < capacity && isSegmented)
                {
                    float x = currentAmount / capacity * effectiveWidth - effectiveWidth / 2f;

                    Borders[i].LocalPosition = new Vector3(x, 0, 0);

                    Borders[i].SizeDelta = new Vector2(borderWidth, 0);

                    Borders[i].IsActive = true;
                }

                else
                {
                    Borders[i].LocalPosition = Vector3.zero;

                    Borders[i].IsActive = false;
                }
            }
        }
    }
}