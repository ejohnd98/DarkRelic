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
    public Transform ItemButtonsParent;
    public GameObject ItemButtonPrefab;
    List<GameObject> ItemButtons;

    public TextMeshProUGUI bloodText;
    public TextMeshProUGUI bloodShadowText;

    //very temp:
    public Sprite placeholderRelicSprite;

    private void Awake() {
        ItemButtons = new List<GameObject>();
    }

    public void SetEntity(DR_Entity newEntity){
        entity = newEntity;
        UpdateUI();
    }

    public void UpdateUI(){
        if (entity == null){
            InventoryUIParent.SetActive(false);
            return;
        }

        foreach (GameObject obj in ItemButtons){
            Destroy(obj);
        }
        ItemButtons.Clear();

        string inventoryText = "--- Inventory ---";
        InventoryText.text = inventoryText;

        InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
        if (inventory != null){

            bloodText.text = inventory.blood.ToString();
            bloodShadowText.text = inventory.blood.ToString();
            

            foreach (var pair in inventory.RelicInventory){
                GameObject itemButtonObj = Instantiate(ItemButtonPrefab, Vector3.zero, Quaternion.identity, ItemButtonsParent);
                UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();
                itemButton.SetEntity(pair.Value.relicEntity);

                itemButton.OnMouseEnterEvents.AddListener(() => {UISystem.instance.detailsUI.SetRelic(pair.Value);});
                itemButton.OnMouseExitEvents.AddListener(() => {UISystem.instance.detailsUI.ClearItem();});

                ItemButtons.Add(itemButtonObj);
            }
        }
        
        InventoryUIParent.SetActive(true);
    }

    public void OnItemClicked(DR_Entity item, DR_Entity user, DR_Entity target){
        // This was removed
        InventoryComponent inventory = user.GetComponent<InventoryComponent>();
        UISystem.instance.SetUIAction(null);
    }
}
