using UnityEditor;

[CustomEditor(typeof(OverWorldController))]
    public class OverworldTransitionerEditor :  Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // OverworldController overworldController = (OverworldController) target;
        }
    }