using UnityEditor;

[CustomEditor(typeof(OverworldController))]
    public class OverworldTransitionerEditor :  Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // OverworldController overworldController = (OverworldController) target;
        }
    }