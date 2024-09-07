using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootHandler : MonoBehaviour
{
    public static LootHandler instance;

    public List<AbilityContent> abilityContentObjects;

    void Awake(){
        if (instance != null && instance != this){
            Debug.LogError("The following singleton already exists!: " + typeof(LootHandler).Name);
        }else{
            instance = this;
        }
    }

    public AbilityContent GetRandomAbility(DR_Ability.AbilityType chestType){

        List<AbilityContent> possibleAbilities = new();
        foreach (var ability in abilityContentObjects){
            if (ability.abilityType == chestType){
                possibleAbilities.Add(ability);
            }
        }
        int randomIndex = UnityEngine.Random.Range(0, possibleAbilities.Count);
        return possibleAbilities[randomIndex];
    }

    // By default prevents duplicates. Can change if desired.
    public List<AbilityContent> GetRandomAbilities(DR_Ability.AbilityType chestType, int count)
    {
        List<AbilityContent> possibleAbilities = new();
        foreach (var ability in abilityContentObjects){
            if (ability.abilityType == chestType){
                possibleAbilities.Add(ability);
            }
        }

        if (possibleAbilities.Count < count){
            Debug.LogAssertion("Tried to get more items then exist. Implement something to work around that now!");
        }

        while (possibleAbilities.Count > count){
            int randomIndex = UnityEngine.Random.Range(0, possibleAbilities.Count);
            possibleAbilities.RemoveAt(randomIndex);
        }

        return possibleAbilities;
    }
}
