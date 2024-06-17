using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInput {
    public string playerPrompt;
    public bool hasInput = false;
    public bool hasPrompted = false;
    public bool hasExitInput = false;

    //TODO: make this generic, and instead of just a validation check, set the value as well?
    public Vector2Int inputValue = Vector2Int.zero;
    private Func<Vector2Int, bool> validationCheck;

    public ActionInput(Func<Vector2Int, bool> valCheck, string prompt = "Please enter a position"){
        validationCheck = valCheck ?? DefaultValidationCheck;
        playerPrompt = prompt;
    }

    public bool GiveInput(Vector2Int inputPos){
        if (!validationCheck(inputPos)){
            return false;
        }
        inputValue = inputPos;
        hasInput = true;
        return true;
    }

    private static bool DefaultValidationCheck(Vector2Int value){
        return true;
    }
}

public abstract class DR_Action {
    public bool loggable = false;
    public DR_Entity owner;

    // Right now this is only used for additional inputs not given when
    // the action is created (ie. requires further input from player)
    public List<ActionInput> actionInputs = new List<ActionInput>();
    public bool wasSuccess = true;

    public bool RequiresInput(){
        foreach (ActionInput actionInput in actionInputs){
            if (actionInput.hasExitInput){
                return false;
            }
            if (!actionInput.hasInput){
                return true;
            }
        }
        return false;
    }

    public bool ShouldExitAction(){
        foreach (ActionInput actionInput in actionInputs){
            if (actionInput.hasExitInput){
                return true;
            }
        }
        return false;
    }

    public ActionInput GetNextNeededInput(){
        for (int i = 0; i < actionInputs.Count; i++){
            if (!actionInputs[i].hasInput){
                return actionInputs[i];
            }
        }
        return null;
    }

    public virtual void Perform(DR_GameManager gm){
        if (wasSuccess){
            GameRenderer.instance.AddAction(this);
        }
    }

    public virtual string GetLogText(){
        return "";
    }

    public virtual List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner};
    }
}

public class MoveAction : DR_Action {
    public Vector2Int pos = Vector2Int.zero;
    MoveAnimation moveAnim;

    public MoveAction (DR_Entity entity, int x, int y){
        this.owner = entity;
        pos = new Vector2Int(x,y);
    }

    public MoveAction (DR_Entity entity, Vector2Int pos){
        this.owner = entity;
        this.pos = pos;
    }

    public override void Perform(DR_GameManager gm){
        Vector2Int posA = owner.Position;
        if(!gm.CurrentMap.CanMoveActor(owner, pos)){
            wasSuccess = false;
            return;
        }
        
        gm.CurrentMap.MoveActor(owner, pos);

        DR_Cell cell = gm.CurrentMap.GetCell(pos);

        if (cell.blood > 0){
            if (owner.GetComponent<InventoryComponent>() is InventoryComponent inventory && inventory.canCollectBlood){
                //TODO: later have a handler for this as relics will affect stuff here when getting blood

                inventory.AddBlood(cell.blood);
                UISystem.instance.RefreshInventoryUI();
                cell.blood = 0;
            }
        }

        base.Perform(gm);
    }
}

public class AttackAction : DR_Action {
    public HealthComponent target;
    public AttackAnimation attackAnim;

    public bool killed = false;
    public DamageEvent damageEvent;

    public AttackAction (HealthComponent target, DR_Entity attacker = null){
        this.target = target;
        this.owner = attacker;
        loggable = false; //handle this separately for attacks
    }

    public override List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner, target.Entity};
    }

    public override string GetLogText(){
        return owner.Name + " attacked " + target.Entity.Name + "!";
    }

    public override void Perform(DR_GameManager gm){
        int baseDamage = owner.GetComponent<LevelComponent>().stats.strength;
        damageEvent = DamageSystem.HandleAttack(gm, owner, target, baseDamage);

        //TODO: check if this is still needed here
        if (!target.IsAlive()){
            killed = true;
            gm.CurrentMap.RemoveActor(target.Entity);
        }

        var test = new TestEvent
        {
            owner = owner
        };
        DR_EventSystem.TestEvent(test);

        base.Perform(gm);
    }
}

public class StairAction : DR_Action {
    public StairComponent stairs;

    public StairAction (DR_Entity owner, StairComponent stairs){
        this.owner = owner;
        this.stairs = stairs;
        loggable = true;
    }

    public override string GetLogText(){
        return owner.Name + (stairs.goesDeeper ? " descended down a set of stairs." : " climbed up a set of stairs.");
    }

    public override void Perform(DR_GameManager gm){

        DR_Map dest = gm.CurrentDungeon.GetNextMap(stairs.goesDeeper);
        gm.MoveLevels(gm.CurrentMap, dest, stairs.goesDeeper, true);
        SoundSystem.instance.PlaySound(stairs.goesDeeper ? "descend" : "ascend");

        base.Perform(gm);
        //TODO: create animation and wait for DR_GameManager.instance.isFadeActive
    }
}

public class DoorAction : DR_Action {
    public DoorComponent target;

    public DoorAction (DoorComponent target, DR_Entity opener = null){
        this.target = target;
        this.owner = opener;
    }

    public override List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner, target.Entity};
    }

    public override void Perform(DR_GameManager gm){
        target.ToggleOpen();
        SoundSystem.instance.PlaySound("door");
        base.Perform(gm);
    }
}

public class GoalAction : DR_Action {
    public GoalComponent target;

    public GoalAction (GoalComponent target, DR_Entity opener = null){
        this.target = target;
        this.owner = opener;
        loggable = true;
    }

    public override void Perform(DR_GameManager gm){
        SoundSystem.instance.PlaySound("altar");
        gm.OnGameWon();
        base.Perform(gm);
    }

    public override string GetLogText(){
        return owner.Name + " has claimed victory!";
    }
}

public class AltarAction : DR_Action {
    public AltarComponent altar;
    private int healthRestored = 0;
    private int bloodCost = 0;

    public AltarAction (DR_Entity owner, AltarComponent altar){
        this.owner = owner;
        this.altar = altar;
        loggable = true;
    }

    public override List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner, altar.Entity};
    }

    public override string GetLogText(){
        switch (altar.altarType){
            case AltarType.HEALTH:
                return owner.Name + " spent " + bloodCost + " blood at the altar to heal " + healthRestored + ".";
            
            case AltarType.ITEM:
                return owner.Name + " spent " + bloodCost + " blood at the altar to acquire a " + altar.itemAltarContent.name + ".";
            default:
                return "unknown altar type!";
        }
    }

    public override void Perform(DR_GameManager gm){
        switch (altar.altarType){
            case AltarType.HEALTH:
            {
                HealthComponent healthComponent = owner.GetComponent<HealthComponent>();
                InventoryComponent inventoryComponent = owner.GetComponent<InventoryComponent>();

                bloodCost = Mathf.Min(healthComponent.maxHealth - healthComponent.currentHealth, inventoryComponent.blood);

                healthRestored = healthComponent.Heal(bloodCost);
                inventoryComponent.SpendBlood(healthRestored);
                wasSuccess = healthRestored > 0;
                break;
            }
            
            case AltarType.ITEM:
            {
                InventoryComponent inventoryComponent = owner.GetComponent<InventoryComponent>();
                bloodCost = 10; //TODO: determine this better
                if (inventoryComponent.blood < bloodCost){
                    wasSuccess = false;
                    break;
                }
                inventoryComponent.SpendBlood(bloodCost);
                inventoryComponent.AddItemFromContent(altar.itemAltarContent);

                break;
            }
            default:
            break;
        }
        
        if (wasSuccess) {
            SoundSystem.instance.PlaySound("altar");
        }
        base.Perform(gm);
    }
}

public class PickupAction : DR_Action {
    public DR_Entity item;

    public PickupAction (DR_Entity item, DR_Entity user){
        this.item = item;
        this.owner = user;
        loggable = true;
    }

    public override List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner, item};
    }

    public override void Perform(DR_GameManager gm){
        InventoryComponent inventory = owner.GetComponent<InventoryComponent>();
        if (inventory != null){
            bool addedItem = inventory.AddItem(item);
            if (addedItem){
                gm.CurrentMap.RemoveItem(item);
            }
            base.Perform(gm);
            return;
        }else{
            Debug.LogError("Inventory is invalid!");
        }
        base.Perform(gm);
    }

    public override string GetLogText(){
        return owner.Name + " picked up " + item.Name;
    }
}


public class WaitAction : DR_Action {

    public WaitAction(DR_Entity owner, bool logAction = false){
        this.owner = owner;
        loggable = logAction;
    }

    public override void Perform(DR_GameManager gm){
        wasSuccess = true;
        base.Perform(gm);
    }

    public override string GetLogText(){
        return owner.Name + " waited around...";
    }
}

