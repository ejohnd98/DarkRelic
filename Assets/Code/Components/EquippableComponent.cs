using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippableComponent : DR_Component
{
    public List<DR_Modifier> modifiers;

    public bool isEquipped = false;

    public EquippableComponent(){
        modifiers = new List<DR_Modifier>();
    }
}
