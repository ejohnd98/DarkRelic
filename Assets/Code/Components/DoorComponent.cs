using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComponent : DR_Component
{
    [Copy]
    public Sprite openSprite, closeSprite;

    [Copy]
    bool isOpen = false;

    [Copy]
    public bool canBeManuallyOpened = true;

    public Action<DR_Event> OnDoorStateChanged;

    public DoorComponent(){}

    public DoorComponent(Sprite openSpr, Sprite closeSpr, bool open = false){
        openSprite = openSpr;
        closeSprite = closeSpr;
        isOpen = open;
    }

    public void ToggleOpen(DR_Entity instigator){
        SetOpen(!isOpen, instigator);
    }

    public void SetOpen(bool open, DR_Entity instigator = null){
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
        if (instigator != null){
            DoorEvent doorEvent = new()
            {
                owner = instigator,
                door = this
            };
            OnDoorStateChanged?.Invoke(doorEvent);
        }
    }

    public bool IsOpen(){
        return isOpen;
    }

    public override string GetDetailsDescription()
    {
        return "The door is " + (IsOpen() ? "open." : "closed.");
    }
}
