using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(AbilityContent))]
public class AbilityContentEditor : Editor
{
    private Type[] derivedTypes;
    private String[] typeNames;
    private int selectedTypeIndex = 0;

    private void OnEnable()
    {
        // Find all classes derived from DR_Ability
        derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(DR_Ability)))
            .ToArray();
        
        typeNames = GetTypeNameArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);

        AbilityContent contentObject = (AbilityContent)target;
        selectedTypeIndex = Mathf.Max(Array.IndexOf(typeNames, contentObject.typeName), 0);

        // Display dropdown for selecting derived classes
        selectedTypeIndex = EditorGUILayout.Popup("Ability Type: ", selectedTypeIndex, typeNames);

        string selectedTypeName = typeNames[selectedTypeIndex];
        if (selectedTypeName != contentObject.typeName)
        {
            contentObject.typeName = selectedTypeName;

            EditorUtility.SetDirty(contentObject);
            AssetDatabase.SaveAssets();
        }
    }

    private string[] GetTypeNameArray()
    {
        // Convert array of Type objects to array of type names
        return derivedTypes.Select(type => type.Name).ToArray();
    }
}
