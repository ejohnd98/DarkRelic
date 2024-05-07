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
                RelicType relicType = pair.Key;
                GameObject itemButtonObj = Instantiate(ItemButtonPrefab, Vector3.zero, Quaternion.identity, ItemButtonsParent);
                UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();
                itemButton.SetSprite(placeholderRelicSprite);

                itemButton.OnMouseEnterEvents.AddListener(() => {UISystem.instance.detailsUI.SetItem(relicType);});
                itemButton.OnMouseExitEvents.AddListener(() => {UISystem.instance.detailsUI.ClearItem();});

                ItemButtons.Add(itemButtonObj);
            }
        }
        
        InventoryUIParent.SetActive(true);
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
