using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;
    public Transform HealthBarPivot; //TODO make healthbar wrapper class (so enemies can have health bars too)
    public EntityDetailsUI detailsUI;
    public InventoryUI inventoryUI;

    Vector2Int LastMousePos = Vector2Int.zero;
    bool ShouldUpdateDetailsUI = true;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void RefreshDetailsUI(){
        ShouldUpdateDetailsUI = true;
    }

    public void UpdateInventoryUI(DR_Entity entity){
        inventoryUI.SetEntity(entity);
    }

    public void RefreshUI(){
        UpdateHealthBar();

        inventoryUI.UpdateUI();

        Vector2Int MousePos = DR_InputHandler.instance.GetMouseCellPosition();
        if (MousePos != LastMousePos || ShouldUpdateDetailsUI){
            LastMousePos = MousePos;
            ShouldUpdateDetailsUI = false;
            DR_Entity MousedOverEntity = DR_GameManager.instance.CurrentMap.GetActorAtPosition(MousePos);
            if (MousedOverEntity == null){
                MousedOverEntity =  DR_GameManager.instance.CurrentMap.GetItemAtPosition(MousePos);
            }
            detailsUI.SetEntity(MousedOverEntity);
        }
    }

    void UpdateHealthBar(){
        HealthComponent PlayerHealth = DR_GameManager.instance.GetPlayer().GetComponent<HealthComponent>();
        float HealthFraction = Mathf.Clamp01(PlayerHealth.currentHealth / (float) PlayerHealth.maxHealth);

        HealthBarPivot.localScale = new Vector3(HealthFraction, 1.0f, 1.0f);
    }

    void Update()
    {
        //TODO Call this from other parts of game when needed instead of every tick
        RefreshUI();
    }
}
