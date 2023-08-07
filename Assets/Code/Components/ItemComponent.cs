using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : DR_Component
{
    public DR_Item ownerItem;

    //TODO: refactor so this isn't needed
    public ItemComponent(DR_Item ownerAsItem){
        ownerItem = ownerAsItem;
    }
}
