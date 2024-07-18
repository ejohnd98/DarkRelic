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
        //TODO: list granted abilities
        return "[To be implemented]";
    }
}
