using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
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
        Debug.Log("Handling turn for " + turnTaker.Name);
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
        HandleTurnAction(gm, turnTaker, playerAction);
    }

    void HandleTurnAction(DR_GameManager gm, DR_Entity turnTaker, DR_Action action){
        bool isPlayer = turnTaker.HasComponent<PlayerComponent>();

        if (action == null){
            Debug.LogAssertion("TurnSystem.HandleTurnAction | action is null!");
            return;
        }


        ActionEvent actionEvent = new ActionEvent(action);
        if (actionEvent.action.requiresFurtherInput){
            //TODO: split with a coroutine function here
        }

        if (ActionSystem.HandleAction(gm, actionEvent)){
            turnTaker.GetComponent<TurnComponent>().SpendTurn();
            if (isPlayer){
                SightSystem.CalculateVisibleCells(turnTaker, gm.CurrentMap);
                DR_Renderer.instance.UpdateTiles();
            }
            
        }else{
            Debug.LogError("ActionSystem could not handle action for " + turnTaker);
        }

        //TODO: move animation component into DR_Action, then check if that exists here.
        // If so, run coroutuine to wait for it, otherwise call TurnEnd directly (assuming animation should run before performing the action)

        //alternatively, have the actions themselves run a coroutine to wait for their animations to finish.
        // this may be much more flexible. if doing this, will need a coroutine here to wait until the action is done.
        // or create an action event and subscribe to it which could be easier/cleaner.

        StartCoroutine(CheckForTurnEnd(gm, turnTaker));
    }

    IEnumerator CheckForTurnEnd(DR_GameManager gm, DR_Entity turnTaker){
        yield return new WaitUntil(() => !AnimationSystem.IsAnimating());
        TurnEnd(gm, turnTaker);
    }

    void TurnEnd(DR_GameManager gm, DR_Entity turnTaker){
        LevelComponent levelComp = turnTaker.GetComponent<LevelComponent>();
        if (levelComp.RequiresLevelUp()){
            LogSystem.instance.AddTextLog(turnTaker.Name + " leveled up!");
            levelComp.AdvanceLevel();
        }
            
        UISystem.instance.RefreshDetailsUI();
        Debug.Log("Turn handled for " + turnTaker.Name);
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
