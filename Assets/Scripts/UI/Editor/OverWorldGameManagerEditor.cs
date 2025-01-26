using System;
using System.Collections.Generic;
using Overworld;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OverWorldGameManager))]
public class OverWorldGameManagerEditor : Editor
{
    [SerializeField] private List<GameObject> parentObjects = new List<GameObject>();

    public override void OnInspectorGUI()
    {
        // Draw default inspector
        base.OnInspectorGUI();

        OverWorldGameManager overWorldGameManager = (OverWorldGameManager)target;

        // Label for custom list
        EditorGUILayout.LabelField("Parent Objects", EditorStyles.boldLabel);

        // Display and edit the list of GameObjects
        if (parentObjects == null)
        {
            parentObjects = new List<GameObject>();
        }

        int listSize = Mathf.Max(0, EditorGUILayout.IntField("Size", parentObjects.Count));

        // Resize the list if necessary
        while (listSize > parentObjects.Count)
        {
            parentObjects.Add(null);
        }

        while (listSize < parentObjects.Count)
        {
            parentObjects.RemoveAt(parentObjects.Count - 1);
        }

        // Render each GameObject field in the list
        for (int i = 0; i < parentObjects.Count; i++)
        {
            parentObjects[i] = (GameObject)EditorGUILayout.ObjectField($"Element {i}", parentObjects[i], typeof(GameObject), true);
        }

        // Example: Assign the list to OverWorldGameManager (if required)
        if (GUILayout.Button("Initialize Data"))
        {
            ProcessParents(overWorldGameManager);
        }
    }

    private void ProcessParents(OverWorldGameManager overWorldGameManager)
    {
        overWorldGameManager.levels.Clear();
        
        foreach (var parentObject in parentObjects)
        {
            if (parentObject == null)
            {
                continue;
            }

            var newData = new OverWorldMapData
            {
                boss = (CatBoss) Enum.Parse(typeof(CatBoss), parentObject.name),
                percentComplete = 0,
                backgroundSprite = parentObject.transform.Find("Background"),
                initialCameraPosition = parentObject.transform.Find("LvlOne"),
                initialCameraOrthoSize = 5
            };
            newData.lastPlayerLevel = newData.initialCameraPosition.GetComponent<OverWorldInnerLevel>();
            newData.firstLevel = newData.lastPlayerLevel;
            overWorldGameManager.levels.Add(newData);
        }
    }
}
