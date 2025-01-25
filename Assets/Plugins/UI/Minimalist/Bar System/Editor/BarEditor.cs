using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Minimalist.Utility.Editor;

namespace Minimalist.Bar.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BarBhv))]
    public class BarEditor : UnityEditor.Editor
    {
        private SerializedProperty _quantity;
        private SerializedProperty _barColors;
        private SerializedProperty _displaySegments;
        private SerializedProperty _segmentAmount;
        private SerializedProperty _mainBorderWidth;
        private SerializedProperty _segmentBorderWidth;
        private SerializedProperty _glowProportion;
        private SerializedProperty _shadowProportion;
        private SerializedProperty _displayLabel;
        private SerializedProperty _labelFontAsset;
        private SerializedProperty _labelFontStyle;
        private SerializedProperty _labelFontSize;
        private SerializedProperty _labelMargins;
        private SerializedProperty _labelAligment;
        private SerializedProperty _fillFastestTime;
        private SerializedProperty _fillIncrementCatchUpTime;
        private SerializedProperty _fillDecrementCatchUpTime;
        private SerializedProperty _flashFrequency;
        private SerializedProperty _flashRepetitions;

        private bool _barColorsFoldout;

        private void OnEnable()
        {
            _quantity = serializedObject.FindProperty("_quantity");

            _barColors = serializedObject.FindProperty("_barColors");

            _displaySegments = serializedObject.FindProperty("_displaySegments");

            _segmentAmount = serializedObject.FindProperty("_segmentAmount");

            _mainBorderWidth = serializedObject.FindProperty("_mainBorderWidth");

            _segmentBorderWidth = serializedObject.FindProperty("_segmentBorderWidth");

            _glowProportion = serializedObject.FindProperty("_glowProportion");

            _shadowProportion = serializedObject.FindProperty("_shadowProportion");

            _displayLabel = serializedObject.FindProperty("_displayLabel");

            _labelFontAsset = serializedObject.FindProperty("_labelFontAsset");

            _labelFontStyle = serializedObject.FindProperty("_labelFontStyle");

            _labelFontSize = serializedObject.FindProperty("_labelFontSize");

            _labelMargins = serializedObject.FindProperty("_labelMargins");

            _labelAligment = serializedObject.FindProperty("_labelAligment");

            _fillFastestTime = serializedObject.FindProperty("_fillFastestTime");

            _fillIncrementCatchUpTime = serializedObject.FindProperty("_fillIncrementCatchUpTime");

            _fillDecrementCatchUpTime = serializedObject.FindProperty("_fillDecrementCatchUpTime");

            _flashFrequency = serializedObject.FindProperty("_flashFrequency");

            _flashRepetitions = serializedObject.FindProperty("_flashRepetitions");
        }

        public override void OnInspectorGUI()
        {
            BarBhv bar = target as BarBhv;

            serializedObject.Update();

            EditorExtensions.ScriptHolder(target);

            SubscriptionSection();

            BarColorsSection();

            SegmentationSection(bar);

            BarBordersSection();

            PseudoLightingSection();

            LabelsSection();

            FillAnimationSection();

            FlashAnimationSection();

            AnimationTestButtons(bar);

            serializedObject.ApplyModifiedProperties();
        }

        private void SubscriptionSection()
        {
            EditorExtensions.PropertyField("Quantity", _quantity);
        }

        private void BarColorsSection()
        {
            EditorExtensions.PropertyField("Colors", _barColors);

            //EditorGUILayout.Separator();

            //_barColorsFoldout = EditorGUILayout.Foldout(_barColorsFoldout, "Colors:", EditorStyles.foldout);

            //if (_barColorsFoldout)
            //{
            //    EditorGUI.BeginChangeCheck();

            //    EditorExtensions.PropertyField("", _barColors);

            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        serializedObject.ApplyModifiedProperties();

            //        bar.UpdateColors();
            //    }
            //}
        }

        private void SegmentationSection(BarBhv bar)
        {
            EditorExtensions.PropertyField("Display Segments", _displaySegments);

            EditorExtensions.PropertyField("Amount Per Segment", _segmentAmount, _displaySegments.boolValue);

            if (bar.Quantity != null)
            {
                if (_segmentAmount.floatValue < BarBhv.MIN_SEGMENT_PROPORTION * bar.Quantity.Capacity)
                {
                    _segmentAmount.floatValue = BarBhv.MIN_SEGMENT_PROPORTION * bar.Quantity.Capacity;
                }
                else if (_segmentAmount.floatValue > bar.Quantity.Capacity)
                {
                    _segmentAmount.floatValue = bar.Quantity.Capacity;
                }
            }
        }

        private void BarBordersSection()
        {
            EditorExtensions.PropertyField("Main Border Width", _mainBorderWidth);

            EditorExtensions.PropertyField("Segment Border Width", _segmentBorderWidth, _displaySegments.boolValue);
        }

        private void PseudoLightingSection()
        {
            EditorExtensions.PropertyField("Glow Proportion", _glowProportion);

            EditorExtensions.PropertyField("Shadow Proportion", _shadowProportion);
        }

        private void LabelsSection()
        {
            EditorExtensions.PropertyField("Display Label", _displayLabel);

            EditorExtensions.PropertyField("Label Font Asset", _labelFontAsset, _displayLabel.boolValue);

            EditorExtensions.PropertyField("Label Font Style", _labelFontStyle, _displayLabel.boolValue);

            EditorExtensions.PropertyField("Label Font Size", _labelFontSize, _displayLabel.boolValue);

            EditorExtensions.PropertyField("Label Margins", _labelMargins, _displayLabel.boolValue);

            EditorExtensions.PropertyField("Label Alignment", _labelAligment, _displayLabel.boolValue);
        }

        private void FillAnimationSection()
        {
            EditorExtensions.PropertyField("Fill Fastest Time", _fillFastestTime);

            EditorExtensions.PropertyField("Fill Increment Time", _fillIncrementCatchUpTime);

            EditorExtensions.PropertyField("Fill Decrement Time", _fillDecrementCatchUpTime);
        }

        private void FlashAnimationSection()
        {
            EditorExtensions.PropertyField("Flash Frequency", _flashFrequency);

            EditorExtensions.PropertyField("Flash Repetitions", _flashRepetitions);
        }

        private void AnimationTestButtons(BarBhv bar)
        {
            GUILayout.Space(8);

            EditorGUILayout.LabelField((Application.isPlaying ? "Runtime" : "Edit Mode") + " Animation Tests:", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                GUI.enabled = false;

                GUILayout.TextArea("Note that animations may play a bit faster & not as smoothly in 'Edit Mode' when compared to 'Runtime'. " +
                    "For that reason, and even though coarse 'Edit Mode' animation tweaking is highly encouraged, " +
                    "I would suggest fine-tuning animation parameters at 'Runtime'.", GUI.skin.textArea);

                GUI.enabled = true;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Decrement By 25%"))
            {
                bar.Quantity.Amount -= bar.Quantity.Capacity * .25f;
            }

            if (GUILayout.Button("Flash"))
            {
                bar.FlashBar();
            }

            if (GUILayout.Button("Increment By 25%"))
            {
                bar.Quantity.Amount += bar.Quantity.Capacity * .25f;
            }

            GUILayout.EndHorizontal();
        }
    }
}