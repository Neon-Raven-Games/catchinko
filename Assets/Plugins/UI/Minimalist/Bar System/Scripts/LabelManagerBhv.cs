using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Minimalist.Utility;

namespace Minimalist.Bar
{
    public class LabelManagerBhv : UIElementBhv
    {
        // Public properties
        public string Text
        {
            set
            {
                foreach (TextMeshProBhv label in Labels)
                {
                    label.Text = value;
                }
            }
        }
        public TMP_FontAsset FontAsset
        {
            set
            {
                foreach (TextMeshProBhv label in Labels)
                {
                    label.FontAsset = value;
                }
            }
        }
        public FontStyles FontStyle
        {
            set
            {
                foreach (TextMeshProBhv label in Labels)
                {
                    label.FontStyle = value;
                }
            }
        }
        public float FontSize
        {
            set
            {
                foreach (TextMeshProBhv label in Labels)
                {
                    label.FontSize = value;
                }
            }
        }
        public Color Color
        {
            set
            {
                foreach (TextMeshProBhv label in Labels)
                {
                    label.FontColor = value;
                }
            }
        }
        public TextAlignmentOptions Alignment
        {
            set
            {
                foreach (TextMeshProBhv label in Labels)
                {
                    label.Alignment = value;
                }
            }
        }
        public Vector4 Margins
        {
            set
            {
                foreach (TextMeshProBhv label in Labels)
                {
                    label.Margins = value;
                }
            }
        }
        public MainLabelBhv Main => _mainLabel == null ? GetComponentInChildren<MainLabelBhv>() : _mainLabel;
        public IncrementLabelBhv Increment => _incrementLabel == null ? GetComponentInChildren<IncrementLabelBhv>() : _incrementLabel;
        public DecrementLabelBhv Decrement => _decrementLabel == null ? GetComponentInChildren<DecrementLabelBhv>() : _decrementLabel;
        public BackgroundLabelBhv Background => _backgroundLabel == null ? GetComponentInChildren<BackgroundLabelBhv>() : _backgroundLabel;

        // Private properties
        private TextMeshProBhv[] Labels => _labels == null || _labels.Length == 0 ? this.GetComponentsInChildren<TextMeshProBhv>() : _labels;

        // Private fields
        private TextMeshProBhv[] _labels;
        private MainLabelBhv _mainLabel;
        private IncrementLabelBhv _incrementLabel;
        private DecrementLabelBhv _decrementLabel;
        private BackgroundLabelBhv _backgroundLabel;

        private void Awake()
        {
            _labels = Labels;

            _mainLabel = Main;

            _incrementLabel = Increment;

            _decrementLabel = Decrement;

            _backgroundLabel = Background;
        }

        public void UpdateLabel(float amount, float minimum, float maximum)
        {
            int maxDigitCount = Mathf.Max(maximum.ToString("f0").Length, minimum.ToString("f0").Length);

            int leadingSpaceCount = Mathf.Max(0, maxDigitCount - amount.ToString("f0").Length);

            string leadingSpaces = new string(' ', leadingSpaceCount);

            Text = leadingSpaces + amount.ToString("f0") + "/" + maximum.ToString("f0");
        }
    }
}
