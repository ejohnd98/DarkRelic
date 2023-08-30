using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
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
}
