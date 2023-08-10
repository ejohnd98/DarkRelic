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
            for (int i = 0; i < inventory.items.Count; i++){
                GameObject itemButtonObj = Instantiate(ItemButtonPrefab, Vector3.zero, Quaternion.identity, ItemButtonsParent);
                UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();
                DR_Item item = inventory.items[i];
                itemButton.SetEntity(item);

                itemButton.OnMouseDownEvents.AddListener(() => {OnItemClicked(item, entity, entity);});
                itemButton.OnMouseEnterEvents.AddListener(() => {UISystem.instance.detailsUI.SetEntity(item);});
                itemButton.OnMouseExitEvents.AddListener(() => {UISystem.instance.detailsUI.HideUI();});

                ItemButtons.Add(itemButtonObj);
            }

            for (int i = inventory.items.Count; i < inventory.capacity; i++){
                GameObject itemButtonObj = Instantiate(ItemButtonPrefab, Vector3.zero, Quaternion.identity, ItemButtonsParent);
                UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();
                itemButton.ItemImage.gameObject.SetActive(false);
                ItemButtons.Add(itemButtonObj);
            }
        }
        
        InventoryUIParent.SetActive(true);
    }

    public void OnItemClicked(DR_Item item, DR_Entity user, DR_Entity target){
        ItemAction itemAction = new ItemAction(item, user, target);
        UISystem.instance.SetUIAction(itemAction);
    }
}
