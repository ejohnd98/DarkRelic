using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public Sprite passiveAbilityFrame;

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

        AbilityComponent abilityComponent = entity.GetComponent<AbilityComponent>();
        if (!abilityComponent.dirtyFlag){
            return; //Really temp
        }
        abilityComponent.dirtyFlag = false;

        foreach (GameObject obj in AbilityButtons){
            Destroy(obj);
        }
        AbilityButtons.Clear();

        if (abilityComponent == null){
            AbilityUIParent.SetActive(false);
            return;
        }

            foreach (var ability in abilityComponent.abilities){
                GameObject abilityButtonObj = Instantiate(AbilityButtonPrefab, Vector3.zero, Quaternion.identity, AbilityButtonsParent);
                UIItemButton abilityButton = abilityButtonObj.GetComponent<UIItemButton>();
                abilityButton.SetAbility(ability);

                abilityButton.OnMouseEnterEvents.AddListener(() => {UISystem.instance.detailsUI.SetAbility(ability);});
                abilityButton.OnMouseExitEvents.AddListener(() => {UISystem.instance.detailsUI.ClearItem();});
                AbilityButtons.Add(abilityButtonObj);
                if (!ability.triggeredByPlayer){
                    abilityButton.GetComponent<Image>().sprite = passiveAbilityFrame;
                    continue;
                }
                abilityButton.OnMouseDownEvents.AddListener(() => {
                    OnAbilityClicked(entity, ability);
                });
            }
        
        AbilityUIParent.SetActive(true);
    }

    public void OnAbilityClicked(DR_Entity owner, DR_Ability ability){
        TurnComponent turnComponent = owner.GetComponent<TurnComponent>();
        if (!turnComponent.waitingForAction || !ability.CanBePerformed()){
            return;
        }

        UISystem.instance.SetUIAction(new AbilityAction(ability, owner));
    }
}
