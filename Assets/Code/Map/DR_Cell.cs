using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Cell
{
    public bool bBlocksMovement = false;
    public DR_Actor Actor;

    public bool IsTraversable(){
        return !bBlocksMovement && Actor == null;
    }
}
