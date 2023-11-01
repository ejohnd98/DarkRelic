using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    List<TurnComponent> EligibleEntities;
    List<TurnComponent> CanAct;

    DR_Action currentAction = null;
    DR_GameManager gm = null;

    public TurnSystem(){
        EligibleEntities = new List<TurnComponent>();
        CanAct = new List<TurnComponent>();
        gm = DR_GameManager.instance;
    }

    public void RemoveEntityTurnComponent(TurnComponent turnComp){
        if (turnComp == null){
            return;
        }
        EligibleEntities.Remove(turnComp);
        CanAct.Remove(turnComp);
    }

    public void UpdateEntityLists(DR_Map map){
        EligibleEntities.Clear();
        CanAct.Clear();

        foreach (DR_Entity entity in map.Entities){
            TurnComponent turnComp = entity.GetComponent<TurnComponent>();
            if (turnComp == null){
                continue;
            }

            EligibleEntities.Add(turnComp);
            if(turnComp.CanTakeTurn()){

                // TODO implement initiative or something. For now player always takes priority
                if (turnComp.Entity.HasComponent<PlayerComponent>()){
                    CanAct.Insert(0, turnComp);
                }else{
                    CanAct.Add(turnComp);
                }
            }
        }
    }

    public void RecoverDebts(int amount){
        foreach(TurnComponent turnComp in EligibleEntities){
            turnComp.RecoverDebt(amount);
        }
    }

    public bool CanEntityAct(){
        return CanAct.Count != 0;
    }

    public TurnComponent GetNextEntity(){
        if (!CanEntityAct()){
            return null;
        }

        return CanAct[0];
    }

    public TurnComponent PopNextEntity(){
        if (!CanEntityAct()){
            return null;
        }
        TurnComponent turnComp = CanAct[0];
        CanAct.RemoveAt(0);
        return turnComp;
    }

    public bool IsPlayerTurn(){
        if (!CanEntityAct()){
            return false;
        }

        // TODO: have entities properly remove themselves
        // Messy handling of entites which have been removed:
        TurnComponent NextEntityTurn = GetNextEntity();
        DR_Entity NextEntity = NextEntityTurn.Entity;

        if (NextEntity == null){
            CanAct.Remove(NextEntityTurn);
            EligibleEntities.Remove(NextEntityTurn);
            return IsPlayerTurn(); //recursively call to remove all null entities
        }

        return NextEntity.HasComponent<PlayerComponent>();
    }

    public void HandleTurn(DR_GameManager gm, DR_Entity turnTaker){
        //Debug.Log("Handling turn for " + turnTaker.Name);
        gm.SetGameState(DR_GameManager.GameState.HANDLING_TURN);

        bool isPlayer = turnTaker.HasComponent<PlayerComponent>();

        if(isPlayer){
            StartCoroutine(WaitForPlayerInput(gm, turnTaker));
        }else{
            HandleTurnAction(gm, turnTaker, AISystem.DetermineAIAction(gm, turnTaker));
        }
    }

    IEnumerator WaitForPlayerInput(DR_GameManager gm, DR_Entity turnTaker){
        DR_Action playerAction = null;
        while(playerAction == null){
            playerAction = GetPlayerActionFromInput(gm, turnTaker);
            yield return null;
        }
        if (playerAction.requiresFurtherInput){
            //TODO: if playerAction requires input, wait as well
        }

        HandleTurnAction(gm, turnTaker, playerAction);
    }

    void HandleTurnAction(DR_GameManager gm, DR_Entity turnTaker, DR_Action action){
        bool isPlayer = turnTaker.HasComponent<PlayerComponent>();

        if (action == null){
            Debug.LogAssertion("TurnSystem.HandleTurnAction | action is null!");
            return;
        }
        if (currentAction != null){
            Debug.LogAssertion("TurnSystem.HandleTurnAction | currentAction is NOT null!");
            return;
        }
        currentAction = action;
        currentAction.StartAction(gm);
        StartCoroutine(CheckIfActionFinished(gm, turnTaker));
    }

    void Update(){
        if (currentAction != null){ //TODO: change to flag/state
            currentAction.ActionStep(gm, Time.deltaTime);
        }
    }

    IEnumerator CheckIfActionFinished(DR_GameManager gm, DR_Entity turnTaker){
        yield return new WaitUntil(() => currentAction.hasFinished);
        Debug.Log("action "+ currentAction.GetType() + " for " + turnTaker.Name + " succeeded: " + currentAction.wasSuccess);
        TurnEnd(gm, turnTaker, currentAction.wasSuccess);
    }

    void TurnEnd(DR_GameManager gm, DR_Entity turnTaker, bool actionSucceeded){
        if (actionSucceeded){

            LevelComponent levelComp = turnTaker.GetComponent<LevelComponent>();
            if (levelComp.RequiresLevelUp()){
                LogSystem.instance.AddTextLog(turnTaker.Name + " leveled up!");
                levelComp.AdvanceLevel();
            }
                
            UISystem.instance.RefreshDetailsUI();

            turnTaker.GetComponent<TurnComponent>().SpendTurn();
            if (turnTaker.HasComponent<PlayerComponent>()){
                SightSystem.CalculateVisibleCells(turnTaker, gm.CurrentMap);
                DR_Renderer.instance.UpdateTiles();
            }
        }
        currentAction = null;
        gm.OnTurnHandled();
    }

    //TODO: do this somewhere else (player component?)
    //TODO: create struct that can be returned to break some of this up
    private DR_Action GetPlayerActionFromInput(DR_GameManager gm, DR_Entity playerActor){
        DR_Action selectedAction = null;

        KeyCode key = KeyCode.None;

        Vector2Int interactPos = Vector2Int.zero;
        for (int i = 0; i < DR_GameManager.KeyDirections.Length; i++)
        {
            if (DR_InputHandler.GetKeyPressed(DR_GameManager.KeyDirections[i]))
            {
                key = DR_GameManager.KeyDirections[i];
                interactPos = playerActor.Position + gm.Directions[i];
            }
        }

        for (int i = 0; i < DR_GameManager.NumberKeys.Length; i++)
        {
            if (DR_InputHandler.GetKeyPressed(DR_GameManager.NumberKeys[i]))
            {
                key = DR_GameManager.NumberKeys[i];
                interactPos = playerActor.Position;
            }
        }

        if (DR_InputHandler.GetKeyPressed(KeyCode.Space)){
            key = KeyCode.Space;
            interactPos = playerActor.Position;
        }

        if (DR_InputHandler.GetKeyPressed(KeyCode.G)){
            key = KeyCode.G;
            interactPos = playerActor.Position;
        }

        DR_Action UIAction = UISystem.instance.GetUIAction();

        if (key != KeyCode.None || UIAction != null)
        {
            List<DR_Action> possibleActions = InteractionSystem.GetPotentialActions(playerActor, gm.CurrentMap, interactPos, key);

            if (UIAction != null){
                selectedAction = UIAction;
            }else if (possibleActions.Count > 0){
                selectedAction = possibleActions[0] ;
            }
        }

        return selectedAction;
    }
}
