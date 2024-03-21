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

    public GameObject EquipmentUIParent;
    public TextMeshProUGUI EquipmentText; 
    public Transform EquipmentButtonsParent;
    public GameObject EquipmentButtonPrefab;
    List<GameObject> EquipmentButtons;
    
    public TextMeshProUGUI tempRelicText;

    private void Awake() {
        ItemButtons = new List<GameObject>();
        EquipmentButtons = new List<GameObject>();
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
        foreach (GameObject obj in EquipmentButtons){
            Destroy(obj);
        }
        EquipmentButtons.Clear();

        string inventoryText = "--- Inventory ---";
        InventoryText.text = inventoryText;

        string equipmentText = "--- Equipment ---";
        EquipmentText.text = equipmentText;

        InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
        if (inventory != null){
            
            //temp relic text:
            tempRelicText.text = "";
            foreach (KeyValuePair<RelicType, int> pair in inventory.RelicInventory)
            {
                if (tempRelicText.text != "")
                {
                    tempRelicText.text += '\n';
                }
                if (pair.Value > 0)
                {
                    tempRelicText.text += pair.Key.ToString() + ": " + pair.Value;
                }
            }
            
            for (int i = 0; i < inventory.items.Count; i++){
                DR_Entity item = inventory.items[i];
                EquippableComponent equippable = item.GetComponent<EquippableComponent>();

                GameObject itemButtonObj;

                if (equippable != null && equippable.isEquipped){
                    itemButtonObj = Instantiate(EquipmentButtonPrefab, Vector3.zero, Quaternion.identity, EquipmentButtonsParent);
                    EquipmentButtons.Add(itemButtonObj);
                }else{
                    itemButtonObj = Instantiate(ItemButtonPrefab, Vector3.zero, Quaternion.identity, ItemButtonsParent);
                    ItemButtons.Add(itemButtonObj);
                }

                UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();

                itemButton.SetEntity(item);
                itemButton.OnMouseDownEvents.AddListener(() => {OnItemClicked(item, entity, entity);});
                itemButton.OnMouseEnterEvents.AddListener(() => {UISystem.instance.detailsUI.SetEntity(item);});
                itemButton.OnMouseExitEvents.AddListener(() => {UISystem.instance.detailsUI.HideUI();});
            }

            for (int i = inventory.items.Count - inventory.equippedItems; i < inventory.capacity; i++){
                GameObject itemButtonObj = Instantiate(ItemButtonPrefab, Vector3.zero, Quaternion.identity, ItemButtonsParent);
                UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();
                itemButton.ItemImage.gameObject.SetActive(false);
                ItemButtons.Add(itemButtonObj);
            }

            for (int i = inventory.equippedItems; i < inventory.maxEquips; i++){
                GameObject itemButtonObj = Instantiate(ItemButtonPrefab, Vector3.zero, Quaternion.identity, EquipmentButtonsParent);
                UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();
                itemButton.ItemImage.gameObject.SetActive(false);
                EquipmentButtons.Add(itemButtonObj);
            }
        }
        
        InventoryUIParent.SetActive(true);
        EquipmentUIParent.SetActive(true);
    }

    public void OnItemClicked(DR_Entity item, DR_Entity user, DR_Entity target){
        //messy drop/add action:
        DR_Action action;

        EquippableComponent equippable = item.GetComponent<EquippableComponent>();
        InventoryComponent inventory = user.GetComponent<InventoryComponent>();
        if (equippable != null){
            action = new ChangeEquipmentAction(item, user, !equippable.isEquipped);
            UISystem.instance.SetUIAction(action);
            return;
        }

        if (DR_InputHandler.GetKeyHeld(KeyCode.LeftControl)){
            action = new DropAction(item, user);
        }else{
            action = new ItemAction(item, user, target);
        }
        UISystem.instance.SetUIAction(action);
    }
}
