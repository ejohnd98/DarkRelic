using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComponent : DR_Component
{
    public Sprite openSprite, closeSprite;

    bool isOpen = false;

    public DoorComponent(Sprite openSpr, Sprite closeSpr, bool open = false){
        openSprite = openSpr;
        closeSprite = closeSpr;
        isOpen = open;
    }

    public void ToggleOpen(){
        SetOpen(!isOpen);
    }

    public void SetOpen(bool open){
        isOpen = open;
        PropComponent propComp = Entity.GetComponent<PropComponent>();
        if(propComp != null){
            propComp.blocksMovement = !open;
            propComp.blocksSight = !open;
        }

        SpriteComponent spriteComp = Entity.GetComponent<SpriteComponent>();
        if(spriteComp != null){
            spriteComp.Sprite = open ? openSprite : closeSprite;
        }
    }

    public bool IsOpen(){
        return isOpen;
    }
}
