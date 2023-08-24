using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UISystem : MonoBehaviour
{
    enum UIState {
        NORMAL, //come up with better name?
        SELECTING_TARGET,

        INVALID
    }

    UIState currentState = UIState.NORMAL;

    public static UISystem instance;
    public Transform HealthBarPivot; //TODO make healthbar wrapper class (so enemies can have health bars too)
    public EntityDetailsUI detailsUI;
    public InventoryUI inventoryUI;

    DR_Action UIAction;

    Vector2Int LastMousePos = Vector2Int.zero;
    bool ShouldUpdateDetailsUI = true;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        detailsUI.SetEntity(null);
    }

    public void RefreshDetailsUI(){
        ShouldUpdateDetailsUI = true;
    }

    public void UpdateInventoryUI(DR_Entity entity){
        inventoryUI.SetEntity(entity);
    }

    public void RefreshInventoryUI(){
        inventoryUI.UpdateUI();
    }

    public void RefreshUI(){
        UpdateHealthBar();

        if (!DR_InputHandler.instance.mouseIsInWorld){
            DR_Renderer.instance.ResetSelectedCell();
            return;
        }

        Vector2Int MousePos = DR_InputHandler.instance.mouseWorldPosition;
        if (MousePos != LastMousePos || ShouldUpdateDetailsUI){

            //TODO: should just have a single gameobject for the cursor?
            // may want to keep this when highlighting multiple cells though (spell target selection?)
            DR_Renderer.instance.SetSelectedCell(MousePos);

            LastMousePos = MousePos;
            ShouldUpdateDetailsUI = false;
            DR_Entity MousedOverEntity = DR_GameManager.instance.CurrentMap.GetActorAtPosition(MousePos);
            if (MousedOverEntity == null){
                MousedOverEntity = DR_GameManager.instance.CurrentMap.GetItemAtPosition(MousePos);
            }
            detailsUI.SetEntity(MousedOverEntity);
        }
    }

    void UpdateHealthBar(){
        HealthComponent PlayerHealth = DR_GameManager.instance.GetPlayer().GetComponent<HealthComponent>();
        float HealthFraction = Mathf.Clamp01(PlayerHealth.currentHealth / (float) PlayerHealth.maxHealth);

        HealthBarPivot.localScale = new Vector3(HealthFraction, 1.0f, 1.0f);
    }

    public void SetUIAction(DR_Action action){
        UIAction = action;
    }

    public DR_Action GetUIAction(bool clearAction = true){
        DR_Action returnedAction = UIAction;

        if (clearAction){
            UIAction = null;
        }
        
        return returnedAction;
    }

    //TODO: pass in int for # of targets? or an array to be filled?
    public void BeginTargetSelection(){
        currentState = UIState.SELECTING_TARGET;
    }

    void Update()
    {
        switch(currentState)
        {
            case UIState.NORMAL:
                break;
            case UIState.SELECTING_TARGET:
                //TODO: do this through DR_InputHandler
                if (Input.GetMouseButtonDown(0) && DR_InputHandler.instance.mouseIsInWorld){
                    Vector2Int MousePos = DR_InputHandler.instance.mouseWorldPosition;
                    if (DR_GameManager.instance.ProvideAdditionalInput(MousePos)){
                        currentState = UIState.NORMAL;
                    }

                    //TODO: allow selecting other items in inventory?
                }
                break;
            default:
                break;
        }

        //TODO: Call this from other parts of game when needed instead of every tick
        RefreshUI();
    }
}
