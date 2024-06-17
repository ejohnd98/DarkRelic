using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityComponent : DR_Component
{
    private List<DR_Ability> abilities = new();

    public AbilityComponent(){
        var testAbility = new TestAbility();
        abilities.Add(testAbility);
    }
}
