using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is only used for pickups
public class RelicComponent : DR_Component
{
    [Copy]
    public AbilityContent[] grantedAbilities;

    public override string GetDetailsDescription()
    {
        string description = "Grants:";

        for (int i = 0; i < grantedAbilities.Length; i++){
            description += '\n' + grantedAbilities[i].name;
        }

        return description;
    }
}
