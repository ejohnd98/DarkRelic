using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    // keep a list of all actors
    // have a queue of any actors that are going to act in the current turn
    // each turn loop through and add to a 

    List<TurnComponent> EligibleEntities;
    List<TurnComponent> CanAct;

    public TurnSystem(){
        EligibleEntities = new List<TurnComponent>();
        CanAct = new List<TurnComponent>();
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
        // messy handling of entites which have been removed
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
        //TODO: maybe this shouldn't be called on updatewhen it is the players turn (ie. only call this when there IS input?)

        DR_Action turnAction = null;

        //get turncomponent

        //determine if this is the player or an AI:
        bool isPlayer = turnTaker.HasComponent<PlayerComponent>();
        if(isPlayer){
            turnAction = GetPlayerActionFromInput(gm, turnTaker);
        }else{
            //TODO: move AI code here
        }

        if (turnAction != null){
            gm.SetGameState(DR_GameManager.GameState.HANDLING_TURN);

            ActionEvent actionEvent = new ActionEvent(turnAction);
            if (actionEvent.action.requiresFurtherInput){
                // CurrentState = GameState.FURTHER_INPUT_REQUIRED;
                // LogSystem.instance.AddTextLog("Please select a target...");
                // currentActionEvent = actionEvent;
                // UISystem.instance.BeginTargetSelection();
                // break;

                //TODO: move into a coroutine here?
                //TODO: possibly move into one regardless of whether further input is required
                //TODO: ie. yield return until this bool is true
                //TODO: (this reduces weird branching behaviour between actions with/without needed input)
            }
            if (ActionSystem.HandleAction(gm, actionEvent)){
                turnTaker.GetComponent<TurnComponent>().SpendTurn();
                PopNextEntity();
                if (isPlayer){
                    SightSystem.CalculateVisibleCells(turnTaker, gm.CurrentMap);
                    DR_Renderer.instance.UpdateTiles();
                }
                
            }

            StartCoroutine(CheckForTurnEnd(gm, turnTaker));
        }
    }

    IEnumerator CheckForTurnEnd(DR_GameManager gm, DR_Entity turnTaker){
        Debug.Log("CheckForTurnEnd started");
        yield return new WaitUntil(() => DR_Renderer.animsActive <= 0);
        Debug.Log("CheckForTurnEnd finishing up");

        //End of turn stuff:
        LevelComponent levelComp = turnTaker.GetComponent<LevelComponent>();
        if (levelComp.RequiresLevelUp()){
            LogSystem.instance.AddTextLog(turnTaker.Name + " leveled up!");
            levelComp.AdvanceLevel();

            //TODO: create new game state for this where player can choose skills. etc
        }
            
        UISystem.instance.RefreshDetailsUI();

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
