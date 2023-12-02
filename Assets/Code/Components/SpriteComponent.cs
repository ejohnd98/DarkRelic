using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteComponent : DR_Component
{
    [Copy]
    public Sprite Sprite;
    [Copy]
    public bool hasAnimation = false;
    [Copy]
    public Sprite[] animFrames;
    [Copy]
    public float animLength = 2.0f;

    public SpriteComponent(){}

    public SpriteComponent(Sprite spr){
        Sprite = spr;
    }

    public Sprite GetCurrentSprite(){
        //TODO: handle the animation in this component so it can actually get the current sprite
        return hasAnimation? animFrames[0] : Sprite;
    }
}
