using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropComponent : DR_Component
{
    [Copy]
    public bool blocksSight = true;
    [Copy]
    public bool blocksMovement = true;
}
