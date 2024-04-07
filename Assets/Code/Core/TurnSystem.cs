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

        while (playerAction.RequiresInput()){
            ActionInput actionInput = playerAction.GetNextNeededInput();
            if (!actionInput.hasPrompted){
                actionInput.hasPrompted = true;
                LogSystem.instance.AddTextLog(actionInput.playerPrompt);
                UISystem.instance.BeginTargetSelection(actionInput);
            }
            yield return null;
        }

        if (playerAction.ShouldExitAction()){
            TurnEnd(gm, turnTaker, false);
        }else{
            HandleTurnAction(gm, turnTaker, playerAction);
        }

        
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
        //Debug.Log("action "+ currentAction.GetType() + " for " + turnTaker.Name + " succeeded: " + currentAction.wasSuccess);
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

            //TODO: merge popping of entity and spending of turn
            DR_Entity poppedEntity = PopNextEntity().Entity;
            if (poppedEntity != turnTaker){
                //TODO: this will be hit if the player kills themselves
                Debug.LogAssertion("TurnSystem.TurnEnd: Popped entity does not match turn taker!");
            }

            turnTaker.GetComponent<TurnComponent>().SpendTurn();
            if (turnTaker.HasComponent<PlayerComponent>()){
                SightSystem.CalculateVisibleCells(turnTaker, gm.CurrentMap);
                DR_Renderer.instance.UpdateTiles();
            }
        }
        currentAction = null;
        gm.OnTurnHandled();
    }

    private DR_Action GetPlayerActionFromInput(DR_GameManager gm, DR_Entity playerActor){

        DR_Action UIAction = UISystem.instance.GetUIAction();
        if (UIAction != null){
            return UIAction;
        }

        List<DR_Action> actionList = new List<DR_Action>();

        for (int i = 0; i < DR_GameManager.KeyDirections.Length; i++)
        {
            if (DR_InputHandler.GetKeyPressed(DR_GameManager.KeyDirections[i]))
            {
                Vector2Int interactPos = playerActor.Position + gm.Directions[i];

                DR_Cell targetCell = gm.CurrentMap.Cells[interactPos.y, interactPos.x];

                if (targetCell.Actor != null){
                    HealthComponent target = targetCell.Actor.GetComponent<HealthComponent>();
                    if (target != null){
                        actionList.Add(new AttackAction(target, playerActor));
                    }
                }

                if (!targetCell.BlocksMovement()){
                    actionList.Add(new MoveAction(playerActor, interactPos));
                }

                if (targetCell.Prop != null) {
                    AltarComponent altar = targetCell.Prop.GetComponent<AltarComponent>();
                    if (altar != null) {
                        actionList.Add(new AltarAction(playerActor, altar));
                    }
                    
                    DoorComponent door = targetCell.Prop.GetComponent<DoorComponent>();
                    if (door != null){
                        actionList.Add(new DoorAction(door, playerActor));
                    }

                    StairComponent stairs = targetCell.Prop.GetComponent<StairComponent>();
                    if (stairs != null){
                        actionList.Add(new StairAction(playerActor, stairs));
                    }

                    GoalComponent goal = targetCell.Prop.GetComponent<GoalComponent>();
                    if (goal != null){
                        actionList.Add(new GoalAction(goal, playerActor));
                    }
                }
            }
        }

        if (DR_InputHandler.GetKeyPressed(KeyCode.Space)){
            actionList.Add(new WaitAction(playerActor, true));
        }

        if (DR_InputHandler.GetKeyPressed(KeyCode.G)){
            DR_Cell targetCell = gm.CurrentMap.Cells[playerActor.Position.y, playerActor.Position.x];
            if (targetCell.Item != null){
                actionList.Add(new PickupAction(targetCell.Item, playerActor));
            }
        }

        if (actionList.Count > 0){
            return actionList[0] ;
        }

        return null;
    }
}
