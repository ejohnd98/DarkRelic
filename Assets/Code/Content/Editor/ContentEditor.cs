using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(Content))]
public class ContentEditor : Editor
{
    private Type[] derivedTypes;
    private int selectedTypeIndex = 0;

    private void OnEnable()
    {
        // Find all classes derived from DR_Component
        derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(DR_Component)))
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Content contentObject = (Content)target;

        GUILayout.Space(10);

        // Display the dropdown for selecting derived classes
        selectedTypeIndex = EditorGUILayout.Popup("Component Type: ", selectedTypeIndex, GetTypeNameArray());

        // Add a button to call the function with the selected type
        if (GUILayout.Button("Add Component"))
        {
            if (selectedTypeIndex >= 0 && selectedTypeIndex < derivedTypes.Length)
            {
                Type selectedType = derivedTypes[selectedTypeIndex];
                DR_Component selectedComponent = (DR_Component)Activator.CreateInstance(selectedType);
                contentObject.AddComponent(selectedComponent);
            }
        }
    }

    private string[] GetTypeNameArray()
    {
        // Convert array of Type objects to array of type names
        return derivedTypes.Select(type => type.Name).ToArray();
    }
}
