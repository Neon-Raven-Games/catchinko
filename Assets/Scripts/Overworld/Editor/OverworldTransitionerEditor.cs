using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OverworldController))]
    public class OverworldTransitionerEditor :  Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OverworldController overworldController = (OverworldController) target;
            if (GUILayout.Button("Test Dynamic Transitions"))
            {
                overworldController.TestDynamicTransitions();
            }
            
        }
    }