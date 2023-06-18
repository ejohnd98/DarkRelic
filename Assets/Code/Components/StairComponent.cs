using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairComponent : DR_Component
{
    public bool goesDeeper = false;

    public StairComponent(bool deeper = true){
        goesDeeper = deeper;
    }
}
