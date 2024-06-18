using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityComponent : DR_Component
{
    public List<DR_Ability> abilities = new();

    public AbilityComponent(){
        var testAbility = new TestAbility();
        abilities.Add(testAbility);
    }

    public void TriggerAbilityFromUI(DR_Ability triggeredAbility){
        TestEvent uiEvent = new();
        foreach(var ability in abilities){
            if (ability == triggeredAbility){

                if (!ability.CanBePerformed()){
                    continue;
                }

                ability.OnTrigger(uiEvent);
                return;
            }
        }
    }
}
