using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityComponent : DR_Component
{
    [Copy]
    public AbilityContent[] startingAbilities;

    public List<DR_Ability> abilities = new();

    public AbilityComponent(){

    }

    public bool dirtyFlag = true; //Temp for UI

    public override void OnComponentAdded()
    {
        if (startingAbilities == null){
            return;
        }

        foreach(var startingAbility in startingAbilities){
            Type abilityType = Type.GetType(startingAbility.typeName);
            DR_Ability ability = System.Activator.CreateInstance(abilityType) as DR_Ability;
            ability.sprite = startingAbility.abilitySprite;
            ability.abilityName = startingAbility.contentName;
            abilities.Add(ability);
            dirtyFlag = true;
        }
    }
}
