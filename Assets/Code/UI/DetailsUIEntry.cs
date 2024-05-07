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

        spriteImage.sprite = cell.bBlocksMovement ? DR_Renderer.instance.WallTexture : DR_Renderer.instance.FloorTexture;

        nameText.text = entityName;
        detailsText.text = details;
    }

    public void Init(RelicType relic){
        string entityName = relic.ToString();
        int count = DR_GameManager.instance.GetPlayer().GetComponent<InventoryComponent>().RelicInventory[relic];
        string details = "You possess " + count + " of this.";

        nameText.text = entityName;
        detailsText.text = details;
    }
}
