using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailsUIEntry : MonoBehaviour
{
    public TextMeshProUGUI nameText; 
    public TextMeshProUGUI detailsText; 
    public Image spriteImage;
    public Image bloodstainedOverlay;
    public Image bloodOverlay;

    public void Init(DR_Entity entity){

        string entityName = entity.Name;
        string details = "";
        bool firstLine = true;

        if (entity.GetComponent<LevelComponent>() is LevelComponent levelComponent){
            if (!firstLine) { details += '\n'; }
            details += levelComponent.GetDetailsDescription();
            firstLine = false;
        }

        if (entity.GetComponent<TurnComponent>() is TurnComponent turnComponent){
            if (!firstLine) { details += '\n'; }
            details += turnComponent.GetDetailsDescription();
            firstLine = false;
        }

        if (entity.GetComponent<HealthComponent>() is HealthComponent healthComponent){
            if (!firstLine) { details += '\n'; }
            details += healthComponent.GetDetailsDescription();
            firstLine = false;
        }

        if (entity.GetComponent<AltarComponent>() is AltarComponent altarComponent){
            if (!firstLine) { details += '\n'; }
            details += altarComponent.GetDetailsDescription();
            firstLine = false;
        }

        if (entity.GetComponent<DoorComponent>() is DoorComponent doorComponent){
            if (!firstLine) { details += '\n'; }
            details += doorComponent.GetDetailsDescription();
            firstLine = false;
        }

        if (entity.GetComponent<RelicComponent>() is RelicComponent relicComponent){
            if (!firstLine) { details += '\n'; }
            details += relicComponent.GetDetailsDescription();
            firstLine = false;
        }

        if (entity.GetComponent<SpriteComponent>() is SpriteComponent spriteComponent){
            spriteImage.sprite = spriteComponent.GetCurrentSprite();
            spriteImage.gameObject.SetActive(true);
        }else{
            spriteImage.gameObject.SetActive(false);
        }

        nameText.text = entityName;
        detailsText.text = details;
    }

    // Note: for displaying floor/wall, not its containing entities
    public void Init(DR_Cell cell){
        string entityName = cell.bBlocksMovement ? "Wall" : "Floor";
        string details = "";
        if (cell.blood > 0){
            details =  "Covered in " + cell.blood + " unit" + (cell.blood > 1 ? "s" : "") +" of blood.";
            bloodOverlay.gameObject.SetActive(true);
        }else if (cell.bloodStained){
            details =  "Bloodstained.";
            bloodstainedOverlay.gameObject.SetActive(true);
        }

        if (cell.associatedRoom != null){
            details += "\nLabel: " + cell.associatedRoom.roomLabel;
        }

        spriteImage.sprite = cell.bBlocksMovement ? GameRenderer.instance.WallTexture : GameRenderer.instance.FloorTexture;

        nameText.text = entityName;
        detailsText.text = details;
    }

    public void Init(HeldRelic relic){
        string entityName = relic.relicEntity.Name;
        string details = relic.relicEntity.GetComponent<RelicComponent>().GetDetailsDescription();
        details += "\nHeld: " + relic.count + ".";

        spriteImage.sprite = relic.relicEntity.GetComponent<SpriteComponent>().GetCurrentSprite();
        nameText.text = entityName;
        detailsText.text = details;
    }

    public void Init(DR_Ability ability){
        string entityName = ability.abilityName + ((ability.count > 1) ? (" (" + ability.count + ")") : "");
        string details = ability.GetFormattedDescription();

        if (ability.triggeredByPlayer){
            details += "\n\nBlood Cost: " + ability.GetBloodCost();
        }else{
            details += "\n\nPassive";
        }

        if (ability.cooldownLength > 0){
            details += "\nCooldown: " + ability.cooldownLength;
        }

        spriteImage.sprite = ability.sprite;
        nameText.text = entityName;
        detailsText.text = details;
    }
}
