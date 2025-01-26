#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIf))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var showIf = (ShowIf) attribute;
        var conditionField = property.serializedObject.FindProperty(showIf.ConditionalFieldName);

        if (conditionField is {boolValue: true})
            EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var showIf = (ShowIf)attribute;
        var conditionField = property.serializedObject.FindProperty(showIf.ConditionalFieldName);

        if (conditionField is {boolValue: false}) return 0f;  

        return EditorGUI.GetPropertyHeight(property, label);
    }
}
#endif