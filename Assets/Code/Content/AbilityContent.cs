using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class KeyValuePairGeneric<T>
{
    public string Key;
    public T Value;

    public KeyValuePairGeneric(string key, T value)
    {
        this.Key = key;
        this.Value = value;
    }
}

[CreateAssetMenu(fileName = "NewAbilityContent", menuName = "DR Ability Content")]
[System.Serializable]
public class AbilityContent : ContentBase
{
    public DR_Ability.AbilityType abilityType;

    public Sprite abilitySprite;

    [HideInInspector]
    public string typeName;
    
    [HideInInspector]
    public Dictionary<string, object> propertyValues;

    public void CopyPropertiesToAbility(DR_Ability target){
        Type abilityType = target.GetType();

        foreach (FieldInfo targetField in abilityType.GetFields())
        {
            // Check if this should be copied
            if (!Attribute.IsDefined(targetField, typeof(AbilityPropertyAttribute))){
                continue;
            }

            if (targetField != null && propertyValues != null){
                if (propertyValues.TryGetValue(targetField.Name, out object value))
                {
                    targetField.SetValue(target, value);
                }
            }
        }
    }

    private void CreateLists(){
        intProperties = new();
        floatProperties = new();
        boolProperties = new();
        spriteProperties = new();
        colorProperties = new();
        stringProperties = new();
    }

    public void RecreateDictionary(Type abilityType){
        propertyValues = new();

        // Fill defaults first
        foreach (FieldInfo field in abilityType.GetFields())
        {
            if (Attribute.IsDefined(field, typeof(AbilityPropertyAttribute)))
            {
                propertyValues[field.Name] = field.GetValue(Activator.CreateInstance(abilityType));
            }
        }

        if (intProperties == null){
            CreateLists();
        }
        foreach(var pair in intProperties){
            if (propertyValues.ContainsKey(pair.Key))
                propertyValues[pair.Key] = pair.Value;
        }
        foreach(var pair in boolProperties){
            if (propertyValues.ContainsKey(pair.Key))
                propertyValues[pair.Key] = pair.Value;
        }
        foreach(var pair in floatProperties){
            if (propertyValues.ContainsKey(pair.Key))
                propertyValues[pair.Key] = pair.Value;
        }
        foreach(var pair in spriteProperties){
            if (propertyValues.ContainsKey(pair.Key))
                propertyValues[pair.Key] = pair.Value;
        }
        foreach(var pair in colorProperties){
            if (propertyValues.ContainsKey(pair.Key))
                propertyValues[pair.Key] = pair.Value;
        }
        foreach(var pair in stringProperties){
            if (propertyValues.ContainsKey(pair.Key))
                propertyValues[pair.Key] = pair.Value;
        }
    }

    public void UpdateLists(){
        if (intProperties == null){
            CreateLists();
        }
        intProperties.Clear();
        boolProperties.Clear();
        floatProperties.Clear();
        spriteProperties.Clear();
        colorProperties.Clear();
        stringProperties.Clear();

        foreach (KeyValuePair<string, object> pair in propertyValues){
            if (pair.Value is int)
                intProperties.Add(new (pair.Key, (int)pair.Value));
            if (pair.Value is bool)
                boolProperties.Add(new (pair.Key, (bool)pair.Value));
            if (pair.Value is float)
                floatProperties.Add(new (pair.Key, (float)pair.Value));
            if (pair.Value is Sprite)
                spriteProperties.Add(new (pair.Key, (Sprite)pair.Value));
            if (pair.Value is Color)
                colorProperties.Add(new(pair.Key, (Color)pair.Value));
            if (pair.Value is string)
                stringProperties.Add(new (pair.Key, (string)pair.Value));
        }
    }

    // This is pretty bad, but it works. Workaround for dictionary not being serializable
    [Serializable] public class StringIntPair : KeyValuePairGeneric<int> { public StringIntPair(string key, int value) : base(key, value) { } }
    [Serializable] public class StringBoolPair : KeyValuePairGeneric<bool> { public StringBoolPair(string key, bool value) : base(key, value) { } }
    [Serializable] public class StringFloatPair : KeyValuePairGeneric<float> { public StringFloatPair(string key, float value) : base(key, value) { } }
    [Serializable] public class StringSpritePair : KeyValuePairGeneric<Sprite> { public StringSpritePair(string key, Sprite value) : base(key, value) { } }
    [Serializable] public class StringColorPair : KeyValuePairGeneric<Color> { public StringColorPair(string key, Color value) : base(key, value) { } }
    [Serializable] public class StringStringPair : KeyValuePairGeneric<string> { public StringStringPair(string key, string value) : base(key, value) { } }

    [HideInInspector, SerializeField] private List<StringIntPair> intProperties;
    [HideInInspector, SerializeField] private List<StringFloatPair> floatProperties;
    [HideInInspector, SerializeField] private List<StringBoolPair> boolProperties;
    [HideInInspector, SerializeField] private List<StringSpritePair> spriteProperties;
    [HideInInspector, SerializeField] private List<StringColorPair> colorProperties;
    [HideInInspector, SerializeField] private List<StringStringPair> stringProperties;

}