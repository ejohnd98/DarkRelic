using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    List<TurnComponent> EligibleEntities;
    List<TurnComponent> CanAct;

    DR_GameManager gm = null;

    public DR_Action currentAction = null;

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

                // TODO: originally gave player priority but it doesn't seem needed?
                if (false && turnComp.Entity.HasComponent<PlayerComponent>()){
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

    public void HandleTurn(DR_GameManager gm, DR_Entity turnTaker){
        //Debug.Log("Handling turn for " + turnTaker.Name);
        bool isPlayer = turnTaker.HasComponent<PlayerComponent>();
        gm.SetGameState(DR_GameManager.GameState.HANDLING_TURN);
        TurnComponent turnComponent = turnTaker.GetComponent<TurnComponent>();
        turnComponent.waitingForAction = true;

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

        TurnComponent turnComponent = turnTaker.GetComponent<TurnComponent>();
        turnComponent.waitingForAction = false;

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

        if(!isPlayer){
            // Only gating this because this is already handled for player in WaitForPlayerInput
            TurnComponent turnComponent = turnTaker.GetComponent<TurnComponent>();
            turnComponent.waitingForAction = false;
        }

        if (action == null){
            Debug.LogAssertion("TurnSystem.HandleTurnAction | action is null!");
            return;
        }

        currentAction = action;
        action.PerformAction(gm);
        TurnEnd(gm, turnTaker, action.wasSuccess);
        currentAction = null;
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
            }
        }
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
            if (DR_InputHandler.GetKeyPressed(DR_GameManager.KeyDirections[i])
                || DR_InputHandler.GetKeyHeld(DR_GameManager.KeyDirections[i], 0.3f))
            {
                Vector2Int interactPos = playerActor.Position + gm.Directions[i];

                DR_Cell targetCell = gm.CurrentMap.Cells[interactPos.y, interactPos.x];

                if (targetCell.Actor != null){
                    HealthComponent target = targetCell.Actor.GetComponent<HealthComponent>();
                    if (target != null){
                        actionList.Add(new AttackAction(targetCell.Actor, playerActor));
                    }
                }

                //TEMP
                if (targetCell.Prop != null && Input.GetKey(KeyCode.LeftAlt)) {
                    DoorComponent door = targetCell.Prop.GetComponent<DoorComponent>();
                    if (door != null){
                        actionList.Add(new DoorAction(door, playerActor));
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
