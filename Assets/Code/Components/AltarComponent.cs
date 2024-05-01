using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AltarType
{
    HEALTH,
    ITEM
}

public class AltarComponent : DR_Component
{
    [Copy]
    public AltarType altarType;

    [DoNotSerialize]
    public Content itemAltarContent;
}
