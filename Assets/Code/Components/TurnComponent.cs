using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnComponent : DR_Component
{
    public Action<DR_Event> OnActionStart;
    public Action<DR_Event> OnActionEnd;

    float CurrentDebt = 0.0f;

    [DoNotSerialize][HideInInspector]
    public bool waitingForAction = false;

    private LevelComponent levelComponent;

    public bool CanTakeTurn(){
        return CurrentDebt >= 0;
    }

    public float GetTurnLength(){
        if (levelComponent == null){
            levelComponent = Entity.GetComponent<LevelComponent>();
        }
        return levelComponent.stats.turnLength;
    }

    public void SpendTurn()
    {
        CurrentDebt -= GetTurnLength();
    }

    public void RecoverDebt(int amount){
        float newDebt = Mathf.Min(CurrentDebt + amount, 0);
        
        // only tick ability cooldown every entity turn ()
        if (newDebt == 0 && CurrentDebt != 0 && Entity.GetComponent<AbilityComponent>() is AbilityComponent abilityComponent){
            abilityComponent.TickCooldowns();
        }

        if (Entity.GetComponent<HealthComponent>() is HealthComponent health){
            health.TickStatusEffects(amount);
        }

        CurrentDebt = newDebt;
    }

    public override void OnComponentRemoved()
    {
        base.OnComponentRemoved();

        //todo: figure out a better way to do this
        DR_GameManager.instance.turnSystem.RemoveEntityTurnComponent(this);
    }

    public override string GetDetailsDescription()
    {
        return "Turn Length: " + GetTurnLength().ToString("F1");
    }
}
