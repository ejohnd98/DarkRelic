using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbilityContent", menuName = "DR Ability Content")]
[System.Serializable]
public class AbilityContent : ScriptableObject
{
    public string contentName;
    public string abilityDescription; //Temp
    public Sprite abilitySprite;

    [HideInInspector]
    public string typeName;
}