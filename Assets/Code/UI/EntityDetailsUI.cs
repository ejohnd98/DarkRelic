using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// When given an entity, this will extract any useful information and display it to the player
// Can start off as just a string
public class EntityDetailsUI : MonoBehaviour
{
    DR_Cell selectedCell;
    public GameObject DetailsUIParent;
    public TextMeshProUGUI TitleText; 
    public TextMeshProUGUI DetailsText; 
    public Image DetailsImage;

    public void SetCell(DR_Cell newCell){
        selectedCell = newCell ?? DR_GameManager.instance.CurrentMap.GetCell(DR_GameManager.instance.GetPlayer().Position);
        UpdateUI();
    }

    public void HideUI(){
        //entity = null;
        //SetCell(null);
        //DetailsUIParent.SetActive(false);
        //SetEntity(DR_GameManager.instance.GetPlayer());
    }

    private void UpdateUI(){
        if (selectedCell == null){
            DetailsUIParent.SetActive(false);
            return;
        }

        string titleText = "";
        string detailsText = "";
        bool firstLine = true;

        //TODO: show details for each of these instead of first
        DR_Entity focusedEntity = selectedCell.Actor ?? selectedCell.Item ?? selectedCell.Prop;

        // TODO: have a generic GetDetails function on any entity, which loops through components to build up details
        if (focusedEntity != null){
            titleText = focusedEntity.Name;
            detailsText = "";


            LevelComponent level = focusedEntity.GetComponent<LevelComponent>();
            if (level != null){
                if (!firstLine){
                    detailsText += "\n";
                }
                detailsText += "Level " + level.level + "\nStr: " + level.stats.strength;
                firstLine = false;
            }

            HealthComponent health = focusedEntity.GetComponent<HealthComponent>();
            if (health != null){
                if (!firstLine){
                    detailsText += "\n";
                }
                detailsText += "HP: " + health.currentHealth + " / " + health.maxHealth;
                firstLine = false;
            }
            SpriteComponent spriteComp = focusedEntity.GetComponent<SpriteComponent>();
            if (spriteComp != null){
                DetailsImage.sprite = spriteComp.GetCurrentSprite();
                DetailsImage.gameObject.SetActive(true);
            }else{
                DetailsImage.gameObject.SetActive(false);
            }
            EquippableComponent equippable = focusedEntity.GetComponent<EquippableComponent>();
            if (equippable != null){
                foreach(DR_Modifier modifier in equippable.modifiers){
                    if (!firstLine){
                        detailsText += "\n";
                    }
                    detailsText += modifier.GetDescription();
                    firstLine = false;
                }
            }

            HealingConsumableComponent healingConsumable = focusedEntity.GetComponent<HealingConsumableComponent>();
            if (healingConsumable != null){
                if (!firstLine){
                    detailsText += "\n";
                }
                detailsText += healingConsumable.GetDescription();
                firstLine = false;
            }

            MagicConsumableComponent magicConsumable = focusedEntity.GetComponent<MagicConsumableComponent>();
            if (magicConsumable != null){
                if (!firstLine){
                    detailsText += "\n";
                }
                detailsText += magicConsumable.GetDescription();
                firstLine = false;
            }

            DoorComponent doorComponent = focusedEntity.GetComponent<DoorComponent>();
            if (doorComponent != null){
                if (!firstLine){
                    detailsText += "\n";
                }
                detailsText += "The " + focusedEntity.Name + " is " + (doorComponent.IsOpen() ? "open." : "closed.");
                firstLine = false;
            }

        }else{
            TitleText.text = selectedCell.bBlocksMovement ? "Wall" : "Floor";
            DetailsImage.gameObject.SetActive(false);
        }

        if (selectedCell.bloodStained){
            if (!firstLine){
                detailsText += "\n\n";
            }
            detailsText += "Bloodstained" + (selectedCell.blood > 0 ? (" with " + selectedCell.blood + " blood.") : ".");
        }

        TitleText.text = titleText;
        DetailsText.text = detailsText;
        DetailsUIParent.SetActive(true);
    }
}
