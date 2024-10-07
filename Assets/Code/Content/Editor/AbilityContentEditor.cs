using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

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

        Type abilityType = derivedTypes[selectedTypeIndex];

        EditorGUILayout.LabelField("Ability Parameters", EditorStyles.boldLabel);
        contentObject.RecreateDictionary(abilityType);
        foreach (FieldInfo field in abilityType.GetFields())
        {
            if (Attribute.IsDefined(field, typeof(CopyAttribute)))
            {
                if (contentObject.propertyValues.TryGetValue(field.Name, out object value))
                {
                    DrawField(contentObject, field, value);
                }
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(contentObject);
            AssetDatabase.SaveAssets();
        }
    }


    private void DrawField(AbilityContent contentObject, FieldInfo field, object value)
    {
        EditorGUI.BeginChangeCheck();

        // This is not good
        if (field.FieldType == typeof(float))
        {
            float newValue = EditorGUILayout.FloatField(ObjectNames.NicifyVariableName(field.Name), (float)value);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else if (field.FieldType == typeof(int))
        {
            int newValue = EditorGUILayout.IntField(ObjectNames.NicifyVariableName(field.Name), (int)value);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else if (field.FieldType == typeof(bool))
        {
            bool newValue = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(field.Name), (bool)value);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else if (field.FieldType == typeof(string))
        {
            string newValue = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(field.Name), (string)value);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else if (field.FieldType == typeof(Vector2))
        {
            Vector2 newValue = EditorGUILayout.Vector2Field(ObjectNames.NicifyVariableName(field.Name), (Vector2)value);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else if (field.FieldType == typeof(Vector3))
        {
            Vector3 newValue = EditorGUILayout.Vector3Field(ObjectNames.NicifyVariableName(field.Name), (Vector3)value);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else if (field.FieldType == typeof(Color))
        {
            Color newValue = EditorGUILayout.ColorField(ObjectNames.NicifyVariableName(field.Name), (Color)value);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else if (field.FieldType == typeof(Sprite))
        {
            Sprite newValue = (Sprite)EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(field.Name), (Sprite)value, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                contentObject.propertyValues[field.Name] = newValue;
                contentObject.UpdateLists();
            }
        }
        else
        {
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(field.Name), "Unsupported Type");
        }
    }


    private string[] GetTypeNameArray()
    {
        // Convert array of Type objects to array of type names
        return derivedTypes.Select(type => type.Name).ToArray();
    }
}
