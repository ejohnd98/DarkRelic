using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityToolbarUI : MonoBehaviour
{
    // Change to inventory component?
    // would allow player to have multiple inventories (like WOW)
    DR_Entity entity;
    public GameObject AbilityUIParent;
    public Transform AbilityButtonsParent;
    public GameObject AbilityButtonPrefab;
    List<GameObject> AbilityButtons;

    //very temp:
    public Sprite placeholderRelicSprite;

    private void Awake() {
        AbilityButtons = new List<GameObject>();
    }

    public void SetEntity(DR_Entity newEntity){
        entity = newEntity;
        UpdateUI();
    }

    public void UpdateUI(){
        if (entity == null){
            AbilityUIParent.SetActive(false);
            return;
        }

        foreach (GameObject obj in AbilityButtons){
            Destroy(obj);
        }
        AbilityButtons.Clear();

        //TODO: show "abilities" once that system exists
        // have a function to register to which is called when the button is clicked

        // this should have a reference to some ability componment to pull state of each ability (and sprites, dscriptions, etc?)

        // InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
        // if (inventory != null){

        //     foreach (var pair in inventory.RelicInventory){
        //         GameObject itemButtonObj = Instantiate(AbilityButtonPrefab, Vector3.zero, Quaternion.identity, AbilityButtonsParent);
        //         UIItemButton itemButton = itemButtonObj.GetComponent<UIItemButton>();
        //         itemButton.SetEntity(pair.Value.relicEntity);

        //         itemButton.OnMouseEnterEvents.AddListener(() => {UISystem.instance.detailsUI.SetRelic(pair.Value);});
        //         itemButton.OnMouseExitEvents.AddListener(() => {UISystem.instance.detailsUI.ClearItem();});

        //         AbilityButtons.Add(itemButtonObj);
        //     }
        // }
        
        AbilityUIParent.SetActive(true);
    }

    //TODO: implement similar method again
    public void OnItemClicked(DR_Entity item, DR_Entity user, DR_Entity target){
        // This was removed
        InventoryComponent inventory = user.GetComponent<InventoryComponent>();
        UISystem.instance.SetUIAction(null);
    }
}
