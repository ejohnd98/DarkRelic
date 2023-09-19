using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// When given an entity, this will extract any useful information and display it to the player
// Can start off as just a string
public class EntityDetailsUI : MonoBehaviour
{
    DR_Entity entity;
    public GameObject DetailsUIParent;
    public TextMeshProUGUI TitleText; 
    public TextMeshProUGUI DetailsText; 
    public Image DetailsImage;

    public void SetEntity(DR_Entity newEntity){
        entity = newEntity;
        UpdateUI();
    }

    public void HideUI(){
        entity = null;
        DetailsUIParent.SetActive(false);
    }

    private void UpdateUI(){
        if (entity == null){
            DetailsUIParent.SetActive(false);
            return;
        }
        string titleText = entity.Name;
        string detailsText = "";
        bool firstLine = true;

        LevelComponent level = entity.GetComponent<LevelComponent>();
        if (level != null){
            if (!firstLine){
                detailsText += "\n";
            }
            detailsText += "Level " + level.level;
            firstLine = false;
        }

        HealthComponent health = entity.GetComponent<HealthComponent>();
        if (health != null){
            if (!firstLine){
                detailsText += "\n";
            }
            detailsText += "HP: " + health.currentHealth + " / " + health.maxHealth;
            firstLine = false;
        }
        SpriteComponent spriteComp = entity.GetComponent<SpriteComponent>();
        if (spriteComp != null){
            DetailsImage.sprite = spriteComp.Sprite;
            DetailsImage.gameObject.SetActive(true);
        }else{
            DetailsImage.gameObject.SetActive(false);
        }
        EquippableComponent equippable = entity.GetComponent<EquippableComponent>();
        if (equippable != null){
            foreach(DR_Modifier modifier in equippable.modifiers){
                if (!firstLine){
                    detailsText += "\n";
                }
                detailsText += modifier.GetDescription();
                firstLine = false;
            }
        }

        HealingConsumableComponent healingConsumable = entity.GetComponent<HealingConsumableComponent>();
        if (healingConsumable != null){
            if (!firstLine){
                detailsText += "\n";
            }
            detailsText += healingConsumable.GetDescription();
            firstLine = false;
        }

        MagicConsumableComponent magicConsumable = entity.GetComponent<MagicConsumableComponent>();
        if (magicConsumable != null){
            if (!firstLine){
                detailsText += "\n";
            }
            detailsText += magicConsumable.GetDescription();
            firstLine = false;
        }

        TitleText.text = titleText;
        DetailsText.text = detailsText;
        DetailsUIParent.SetActive(true);
    }
}
