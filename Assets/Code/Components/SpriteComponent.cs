using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteComponent : DR_Component
{
    public Sprite Sprite;

    public SpriteComponent(Sprite spr){
        Sprite = spr;
    }
}
