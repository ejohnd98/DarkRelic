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
}
