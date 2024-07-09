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

    public AbilityComponent(){}

    public bool dirtyFlag = true; //Temp for UI

    public override void OnComponentAdded()
    {
        if (startingAbilities == null){
            return;
        }

        foreach(var startingAbility in startingAbilities){
            AddAbilityFromContent(startingAbility);
        }
    }

    public void AddAbilityFromContent(AbilityContent content){
        Type abilityType = Type.GetType(content.typeName);
        DR_Ability ability = System.Activator.CreateInstance(abilityType) as DR_Ability;
        ability.owner = Entity;
        ability.sprite = content.abilitySprite;
        ability.abilityName = content.contentName;
        ability.abilityDescription = content.abilityDescription;
        abilities.Add(ability);
        ability.OnAdded();
        dirtyFlag = true;
    }
}
