using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minimalist.Utility;

namespace Minimalist.Bar
{
    public class MainBorderBhv : UIElementBhv
    {
        // Public properties
        public float BorderWidth
        {
            set
            {
                foreach (BorderBhv border in Borders)
                {
                    border.SizeDelta = value * border.AnchorMin;
                }
            }
        }
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
    }
}