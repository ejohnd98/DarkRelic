using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class UISystem : MonoBehaviour
{
    public enum UIState {
        NORMAL, //come up with better name?
        SELECTING_TARGET,
        MAIN_MENU,
        FADING,

        INVALID,
    }

    public UIState currentState = UIState.MAIN_MENU;

    public Texture2D cursorTexture, targetingCursorTexture;

    public static UISystem instance;
    public Transform HealthBarPivot; //TODO make healthbar wrapper class (so enemies can have health bars too)
    public Transform ExpBarPivot;
    public EntityDetailsUI detailsUI;
    public InventoryUI inventoryUI;
    public DepthGaugeUI depthUI;
    public GameObject gameOverUI, victoryUI;

    DR_Action UIAction;
    ActionInput currentActionInput;

    Vector2Int LastMousePos = Vector2Int.zero;
    bool ShouldUpdateDetailsUI = true;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
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

    public void UpdateDepthUI(){
        depthUI.SetDungeon(DR_GameManager.instance.CurrentDungeon);
    }

    public void RefreshUI(){
        if (currentState == UIState.MAIN_MENU){
            return;
        }
        UpdateHealthBar();
        UpdateExpBar();

        if (!DR_InputHandler.instance.mouseIsInWorld){
            GameRenderer.instance.ResetSelectedCell();
            return;
        }

        Vector2Int MousePos = DR_InputHandler.instance.mouseWorldPosition;
        if (MousePos != LastMousePos || ShouldUpdateDetailsUI){

            //TODO: should just have a single gameobject for the cursor?
            // may want to keep this when highlighting multiple cells though (spell target selection?)
            GameRenderer.instance.SetSelectedCell(MousePos);


            LastMousePos = MousePos;
            ShouldUpdateDetailsUI = false;
            
            if (DR_GameManager.instance.CurrentMap.IsPosVisible(MousePos)){
                detailsUI.SetCell(DR_GameManager.instance.CurrentMap.GetCell(MousePos));
            }else{
                detailsUI.SetCell(null);
            }
            
        }
    }

    void UpdateHealthBar(){
        HealthComponent PlayerHealth = DR_GameManager.instance.GetPlayer().GetComponent<HealthComponent>();
        float HealthFraction = Mathf.Clamp01(PlayerHealth.currentHealth / (float) PlayerHealth.maxHealth);

        HealthBarPivot.localScale = new Vector3(HealthFraction, 1.0f, 1.0f);
    }
    
    void UpdateExpBar(){
        LevelComponent levelComponent = DR_GameManager.instance.GetPlayer().GetComponent<LevelComponent>();
        float ExpFraction = Mathf.Clamp01(levelComponent.currentExp / (float) LevelComponent.GetRequiredExpForLevelUp(levelComponent.level));

        ExpBarPivot.localScale = new Vector3(ExpFraction, 1.0f, 1.0f);
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

    public void BeginTargetSelection(ActionInput actionInput){
        currentActionInput = actionInput;
        currentState = UIState.SELECTING_TARGET;

        Cursor.SetCursor(targetingCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void BackOutOfTargetSelection(){
        currentActionInput.hasExitInput = true;
        currentActionInput = null;
        currentState = UIState.NORMAL;

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }


    void Update()
    {
        if (currentState == UIState.MAIN_MENU){
            return;
        }
        switch(currentState)
        {
            case UIState.NORMAL:
                break;
            case UIState.SELECTING_TARGET:
                if (Input.GetMouseButtonDown(0) && DR_InputHandler.instance.mouseIsInWorld){
                    Vector2Int MousePos = DR_InputHandler.instance.mouseWorldPosition;
                    if (currentActionInput.GiveInput(MousePos)){
                        currentState = UIState.NORMAL;
                        currentActionInput = null;
                        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    }
                    //TODO: allow selecting other items in inventory?
                }
                if (Input.GetKeyDown(KeyCode.Escape)){
                    BackOutOfTargetSelection();
                }
                break;
            case UIState.MAIN_MENU:
                break;
            default:
                break;
        }

        //TODO: Call this from other parts of game when needed instead of every tick
        RefreshUI();
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void ShowVictory()
    {
        victoryUI.SetActive(true);
    }

    public static void ExitGame(){
        Application.Quit();
    }

    public static void NewGame(){
        SceneManager.LoadScene(1);
    }

    public static void ReturnToMenu(){
        SceneManager.LoadScene(0);
    }
}
