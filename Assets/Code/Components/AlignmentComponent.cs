using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alignment {
    PLAYER,
    ENEMY
}

public class AlignmentComponent : DR_Component
{
    [Copy]
    public Alignment alignment;

    public AlignmentComponent(){}

    public AlignmentComponent(Alignment alignment){
        this.alignment = alignment;
    }

    public bool IsFriendly(AlignmentComponent other){
        return other.alignment == alignment;
    }
}
