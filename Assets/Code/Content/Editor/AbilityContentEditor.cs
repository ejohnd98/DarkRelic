using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(AbilityContent))]
public class AbilityContentEditor : Editor
{
    private Type[] derivedTypes;
    private int selectedTypeIndex = 0;

    private void OnEnable()
    {
        // Find all classes derived from DR_Ability
        derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(DR_Ability)))
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AbilityContent contentObject = (AbilityContent)target;

        GUILayout.Space(10);

        // Display the dropdown for selecting derived classes
        selectedTypeIndex = EditorGUILayout.Popup("Ability Type: ", selectedTypeIndex, GetTypeNameArray());
        
        // Add a button to call the function with the selected type
        if (GUILayout.Button("Assign Ability Type"))
        {
            if (selectedTypeIndex >= 0 && selectedTypeIndex < derivedTypes.Length)
            {
                Type selectedType = derivedTypes[selectedTypeIndex];
                contentObject.abilityType = selectedType;
                contentObject.typeName = selectedType.Name;
            }
        }
    }

    private string[] GetTypeNameArray()
    {
        // Convert array of Type objects to array of type names
        return derivedTypes.Select(type => type.Name).ToArray();
    }
}
