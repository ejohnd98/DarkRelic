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

    public override string GetDetailsDescription()
    {
        switch (altarType){
            case AltarType.HEALTH:
                return "Restores health at the cost of blood.";
            
            case AltarType.ITEM:
                return "Grants a " + itemAltarContent.name + " in exchange for blood.";
            default:
                return "unknown altar type!";
        }
    }
}
