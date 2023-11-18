using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIItemButton : UIButton {
    public Image ItemImage;

    public void SetEntity(DR_Entity entity){
        SpriteComponent spriteComp = entity.GetComponent<SpriteComponent>();
        ItemImage.sprite = spriteComp.GetCurrentSprite();
    }
}