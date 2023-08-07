using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// When given an entity, this will extract any useful information and display it to the player
// Can start off as just a string
public class InventoryUI : MonoBehaviour
{
    // Change to inventory component?
    // would allow player to have multiple inventories (like WOW)
    DR_Entity entity;
    public GameObject InventoryUIParent;
    public TextMeshProUGUI InventoryText; 

    public void SetEntity(DR_Entity newEntity){
        entity = newEntity;
        UpdateUI();
    }

    public void UpdateUI(){
        if (entity == null){
            InventoryUIParent.SetActive(false);
            return;
        }
        string inventoryText = "-Inventory-";
        InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
        if (inventory != null){
            for (int i = 0; i < inventory.items.Count; i++){
                inventoryText += "\n" + (i+1) + ": " + inventory.items[i].Name;
            }
        }

        InventoryText.text = inventoryText;
        InventoryUIParent.SetActive(true);
    }
}
