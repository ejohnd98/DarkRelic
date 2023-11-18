using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippableComponent : DR_Component
{
    public List<DR_Modifier> modifiers = new List<DR_Modifier>();

    public bool isEquipped = false;
}
