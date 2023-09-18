using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Modifier
{
    //TODO: rework damage system so that it loops through all of an entities modifiers (gets )

    //TODO: have virtual functions for any potential actions which modifiers can override
    // such as on attack (or on hit), on kill (or on killed)

    //TODO: these should receive some sort of damage event which it can modify.

    //public virtual void ComputeStats(){  
    //}
}
