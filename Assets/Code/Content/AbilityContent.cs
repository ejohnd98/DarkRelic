using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbilityContent", menuName = "DR Ability Content")]
[System.Serializable]
public class AbilityContent : ContentBase
{
    public string abilityDescription; //Temp
    public Sprite abilitySprite;

    [HideInInspector]
    public string typeName;
}