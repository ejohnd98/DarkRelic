using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteComponent : DR_Component
{
    public Sprite Sprite;

    public bool hasAnimation = false;
    public Sprite[] animFrames;
    public float animLength = 2.0f;

    public SpriteComponent(Sprite spr){
        Sprite = spr;
    }

    public Sprite GetCurrentSprite(){
        //TODO: handle the animation in this component so it can actually get the current sprite
        return hasAnimation? animFrames[0] : Sprite;
    }
}
