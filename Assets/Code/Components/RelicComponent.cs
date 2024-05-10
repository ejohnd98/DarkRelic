using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RelicType
{
    DAMAGE_RELIC,
    SPEED_RELIC,
    HEALTH_RELIC,
    NONE
}

// This class is only used for pickups
public class RelicComponent : DR_Component
{
    [Copy]
    public RelicType relicType;

    public override string GetDetailsDescription()
    {
        switch (relicType){
            case RelicType.DAMAGE_RELIC:
                return "Increases damage dealt by 5% (stacks).";
            case RelicType.SPEED_RELIC:
                return "Reduces turn cost by 5% (stacks).";
            case RelicType.HEALTH_RELIC:
                return "Increases total health by 5% (stacks).";
            default:
                return "unknown relic type!";
        }
    }
}
