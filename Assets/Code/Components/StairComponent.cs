using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairComponent : DR_Component
{
    [Copy]
    public bool goesDeeper = false;

    public StairComponent(){}

    public StairComponent(bool deeper = true){
        goesDeeper = deeper;
    }
}
