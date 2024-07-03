using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Ability
{
    public bool triggeredByPlayer = true;
    public string abilityName = "";
    public Sprite sprite;
    public List<ActionInput> actionInputs;
    public List<DR_Entity> relatedEntities = new();

    public DR_Entity owner;

    public int bloodCost = 0;

    public virtual void OnAdded(){
        //owner is guaranteed to be set here
    }

    public virtual bool CanBePerformed(){
        if (bloodCost > 0){
            return owner.GetComponent<InventoryComponent>().blood >= bloodCost;
        }
        return true;
    }

    public void Trigger(DR_Event e){
        if (bloodCost > 0){
            owner.GetComponent<InventoryComponent>().SpendBlood(bloodCost);
        }
        OnTrigger(e);
    }

    protected virtual void OnTrigger(DR_Event e){
    }

    public void ResetInputs(){
        actionInputs = new();
        relatedEntities = new();
        SetupInputs();
    }

    public virtual List<DR_Entity> GetRelatedEntities(){
        return relatedEntities;
    }

    protected virtual void SetupInputs(){

    }

    //TODO: need to think about how this can be driven by scriptable objects as that's where the sprite, name, and description would be
    // Don't necessarily want to make some elaborate generic system though, but can maybe just specify those things, and then see
    // if a dropdown can be created with the ability child class
    // Then when creating the ability at runtime it will just assign the data from the scriptable object to the ability instance

    // Where do entity specific events live?
    // They shouldn't necessarily live on DR_Entity
    // Perhaps they are added to each individual component?
    // What about stuff not tied to components, such as door opening?
    
    // Possibly it does live on the DR_Entity, and then the components trigger them
    // Then things can subscribe to them and they can be triggered without worry.

    // CURRENT TODO:
    // Move ability to scriptable object (initially can be default type?)
    // Create an ability action for those which are player triggered. Can be pretty barebones, but will have the "waiting for input" stage if needed

    // Future:
    // Have relic grant ability
    // Add events for basic stuff on Entity and some way for abilities to subscribe (or just hardcode that)
}

public class TestAbility : DR_Ability
{
    public TestAbility(){
    }
}

public class TestAbility2 : DR_Ability
{
    public TestAbility2(){
    }

    protected override void OnTrigger(DR_Event e){
        base.OnTrigger(e);

        Debug.Log("Extra override ability code!");
    }
}

// TODO: generalize to projectile (or further to targeted?) ability
public class BloodBoltAbility : DR_Ability
{
    public bool killed = false;
    public DR_Entity target;
    public BloodBoltAbility(){
        bloodCost = 1;
    }

    protected override void SetupInputs(){
        //TODO: cap range? Eventually will want to precompute targets so they can shown through UI
        //TODO BUG: require visibility too!
        actionInputs.Add(new ActionInput((Vector2Int pos) => {return DR_GameManager.instance.CurrentMap.GetActorAtPosition(pos) != null;}));
    }

    protected override void OnTrigger(DR_Event e){
        Debug.Log("BloodBoltAbility Triggered with input: " + actionInputs[0].inputValue);

        DR_Entity owner = e.owner;
        target = DR_GameManager.instance.CurrentMap.GetActorAtPosition(actionInputs[0].inputValue);

        relatedEntities.Add(target);
        relatedEntities.Add(owner);

        int baseDamage = owner.GetComponent<LevelComponent>().stats.strength * 2;
        var damageEvent = DamageSystem.HandleAttack(DR_GameManager.instance, owner, target, baseDamage);

        killed = damageEvent.killed;
    }
}

public class BloodWalkAbility : DR_Ability
{
    public BloodWalkAbility(){
        bloodCost = 5;
    }

    protected override void SetupInputs(){
        //TODO BUG: require visibility too!
        actionInputs.Add(new ActionInput((Vector2Int pos) => {
            var cell = DR_GameManager.instance.CurrentMap.GetCell(pos);
            return cell.bloodStained && !cell.BlocksMovement();
            }));
    }

    protected override void OnTrigger(DR_Event e){
        relatedEntities.Add(owner);

        var pos = actionInputs[0].inputValue;
        var gm = DR_GameManager.instance;

        if(!gm.CurrentMap.CanMoveActor(owner, pos)){
            Debug.LogAssertion("BloodWalkAbility failed to execute!");
            return;
        }
        
        gm.CurrentMap.MoveActor(owner, pos);

        DR_Cell cell = gm.CurrentMap.GetCell(pos);
        cell.CollectBlood(owner);
    }
}

public class BludgeonAbility : DR_Ability
{
    public BludgeonAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.OnAttackOther += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e){

        var attackEvent = e as AttackEvent;

        var attackOrigin = attackEvent.owner.Position;
        var pos = attackEvent.target.Position;
        var gm = DR_GameManager.instance;

        // TODO: only bloody tiles if enough damage is done, and only partially on tiles if not enough to cover all
        int bloodAmountPerTile = Mathf.Max(Mathf.CeilToInt(attackEvent.damageDealt * 0.25f), 1);

        // Kind of messy way of getting the "splatter area"
        List<Vector2Int> positions = new()
        {
            pos,
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.right,
            pos + Vector2Int.left
        };

        float distToRemove = 10000.0f;
        Vector2Int posToRemove = Vector2Int.zero;
        foreach(var cellPos in positions){
            float dist = (cellPos - attackOrigin).magnitude;
            if (dist < distToRemove){
                distToRemove = dist;
                posToRemove = cellPos;
            }
        }

        positions.Remove(posToRemove);

        foreach(var cellPos in positions){
            if (gm.CurrentMap.GetCell(cellPos).BlocksMovement(true)){
                continue;
            }
            gm.CurrentMap.GetCell(cellPos).AddBlood(bloodAmountPerTile);
        }
    }
}