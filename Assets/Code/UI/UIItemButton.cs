using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIItemButton : UIButton {
    public Image ItemImage;
    public TextMeshProUGUI amountText;

    public void SetEntity(DR_Entity entity){
        SpriteComponent spriteComp = entity.GetComponent<SpriteComponent>();
        ItemImage.sprite = spriteComp.GetCurrentSprite();
    }

    public void SetSprite(Sprite spr){
        ItemImage.sprite = spr;
    }

    public void SetAbility(DR_Ability ability){
        ItemImage.sprite = ability.sprite;
    }

    public void SetAmount(int amount){
        amountText.text = (amount > 1) ? amount.ToString() : "";
    }
}