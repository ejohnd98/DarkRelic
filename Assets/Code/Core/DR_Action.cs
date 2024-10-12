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

    // TODO: create a list of animations to be rendered alongside this action.
    // will need to pass this action along then to abilities and such
    // OR could maybe have a static "current action" thing going on, but that seems potentially fragile
    // Might be a worthwhile shortcut, but could instead can go through the "owner" entity's turn component and get the current action that way?
    // That way there's not need to pass the action around in events. No entity should ever need to be executing two actions at the exact same time.

    // Things to consider though: ability triggered on player when an enemy attacks. I guess that should be added to the enemy's action then?
    // Also, timing. May become more obvious once I look at the existing animations used by the renderer (ie maybe can define timing in normalized 0-1 values?)
    public List<ActionAnimation> animations = new();


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

    public void PerformAction(DR_GameManager gm){
        if (owner == null){
            Debug.LogAssertion("owner is null on " + GetType().ToString() + "!");
        }
        ActionEvent actionEvent= new ActionEvent();
        actionEvent.owner = owner;
        actionEvent.action = this;
        owner.GetComponent<TurnComponent>().OnActionStart?.Invoke(actionEvent);

        Perform(gm);

        if (wasSuccess){
            GameRenderer.instance.AddAction(this);
        }

        owner.GetComponent<TurnComponent>().OnActionEnd?.Invoke(actionEvent);
    }

    protected virtual void Perform(DR_GameManager gm){
    }

    public virtual string GetLogText(){
        return "";
    }

    public virtual List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner};
    }
}

public class AnimAction : DR_Action {
    // Possibly temp. Use this to pass animations to renderer without needing a pre-existing action
    public bool canOverlapOtherAnims = true;
    public List<DR_Entity> relatedEntities = new();

    public override List<DR_Entity> GetRelatedEntities(){
        return relatedEntities;
    }
}

public class MoveAction : DR_Action {
    public Vector2Int pos = Vector2Int.zero;

    public MoveAction (DR_Entity entity, int x, int y){
        this.owner = entity;
        pos = new Vector2Int(x,y);
    }

    public MoveAction (DR_Entity entity, Vector2Int pos){
        this.owner = entity;
        this.pos = pos;
    }

    protected override void Perform(DR_GameManager gm){
        if(!gm.CurrentMap.CanMoveActor(owner, pos)
            || owner.GetComponent<HealthComponent>().HasStatusEffect(typeof(WebStatusEffect))){
            wasSuccess = false;
            return;
        }
        
        MoveEvent moveEvent = new MoveEvent {
            owner = owner,
            startPos = owner.Position,
            endPos = pos
        };
        animations.Add(new MoveAnimation(owner, owner.Position, pos));
            
        gm.CurrentMap.MoveActor(owner, pos);

        DR_Cell cell = gm.CurrentMap.GetCell(pos);

        if (owner.GetComponent<AbilityComponent>() is AbilityComponent abilityComponent
            && abilityComponent.GetAbility<BloodPickupRangeAbility>() is BloodPickupRangeAbility ability){
            ability.CollectBlood(gm, pos);
        }else{
            cell.CollectBlood(owner);
        }
        

        owner.OnPickUpBlood?.Invoke(moveEvent);

        owner.OnMove?.Invoke(moveEvent);
    }
}

public class AttackAction : DR_Action {
    public DR_Entity target;

    public bool killed = false;
    public DamageEvent damageEvent;

    public AttackAction (DR_Entity target, DR_Entity attacker = null){
        this.target = target;
        this.owner = attacker;
        loggable = false; //handle this separately for attacks
    }

    public override List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner, target};
    }

    public override string GetLogText(){
        return owner.Name + " attacked " + target.Name + "!";
    }

    protected override void Perform(DR_GameManager gm){
        //int baseDamage = owner.GetComponent<LevelComponent>().stats.strength;
        DamageSystem.CreateAttackTransaction(owner, new(){target});

        animations.Add(new AttackAnimation(owner, target, owner.Position, target.Position));

        //TODO: check if this is still needed here
        if (!target.GetComponent<HealthComponent>().IsAlive()){
            killed = true;
            gm.CurrentMap.RemoveActor(target);
        }
    }
}

public class AbilityAction : DR_Action {
    public DR_Ability ability;

    public AbilityAction (DR_Ability ability, DR_Entity owner){
        this.ability = ability;
        this.owner = owner;
        loggable = true;
        
        // Tell action if we need any further input
        ability.ResetInputs();
        actionInputs = ability.actionInputs;
    }

    public override List<DR_Entity> GetRelatedEntities(){
        return ability.GetRelatedEntities();
    }

    public override string GetLogText(){
        return owner.Name + " activated " + ability.abilityName;
    }

    protected override void Perform(DR_GameManager gm){
        if (ability.CanBePerformed()){
            DR_Event abilityEvent = new();
            abilityEvent.owner = owner;
            ability.Trigger(abilityEvent);

            // Very temp. animations should come from abilities themselves
            if (ability is BloodBoltAbility bloodBoltAbility){
                animations.Add(new ProjectileAnimation(owner, bloodBoltAbility.target, owner.Position, bloodBoltAbility.target.Position, 0.2f, bloodBoltAbility.projectileSprite));
            }else{
                animations.Add(new AbilityAnimation(owner));
            }

            wasSuccess = true;
        }else{
            wasSuccess = false;
        }
        
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

    protected override void Perform(DR_GameManager gm){
        if (!stairs.goesDeeper && gm.CurrentDungeon.mapIndex == 0){
            wasSuccess = false;
            
            return;
        }

        //TODO: might be a bug here where you can quickly go up before the renderer has moved down, causing it to go up from what was originally on screen
        DR_Map dest = gm.CurrentDungeon.GetNextMap(stairs.goesDeeper);
        gm.MoveLevels(gm.CurrentMap, dest, stairs.goesDeeper, true);
        animations.Add(new StairAnimation(owner));
        
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

    protected override void Perform(DR_GameManager gm){
        if (!target.canBeManuallyOpened){
            wasSuccess = false;
            return;
        }
        target.ToggleOpen(owner);
        SoundSystem.instance.PlaySound("door");
        
    }
}

public class GoalAction : DR_Action {
    public GoalComponent target;

    public GoalAction (GoalComponent target, DR_Entity opener = null){
        this.target = target;
        this.owner = opener;
        loggable = true;
    }

    protected override void Perform(DR_GameManager gm){
        SoundSystem.instance.PlaySound("altar");
        gm.OnGameWon();
        
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
        //loggable = true; //TODO: figure out better way to get info from altar for log
    }

    public override List<DR_Entity> GetRelatedEntities(){
        return new List<DR_Entity>(){owner, altar.Entity};
    }

    public override string GetLogText(){
        switch (altar.altarType){
            case AltarType.HEALTH_ALTAR:
                return owner.Name + " spent " + bloodCost + " blood at the altar to heal " + healthRestored + ".";
            
            case AltarType.ITEM_ALTAR:
                return owner.Name + " spent " + bloodCost + " blood at the altar to acquire " + altar.altarAbilityContent.name + ".";

            case AltarType.CURSED_ALTAR:
                return owner.Name + " acquired " + altar.altarAbilityContent.name + " from a cursed altar.";
            
            case AltarType.CHEST:
                return owner.Name + " acquired " + altar.altarAbilityContent.name + " from a chest";

            default:
                return "unknown altar type!";
        }
    }

    protected override void Perform(DR_GameManager gm){
        wasSuccess = altar.Interact(owner);
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

   protected override void Perform(DR_GameManager gm){
        InventoryComponent inventory = owner.GetComponent<InventoryComponent>();
        if (inventory != null){
            bool addedItem = inventory.AddItem(item);
            if (addedItem){
                gm.CurrentMap.RemoveItem(item);
            }
            
            return;
        }else{
            Debug.LogError("Inventory is invalid!");
        }
        
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

   protected override void Perform(DR_GameManager gm){
        wasSuccess = true;
        
    }

    public override string GetLogText(){
        return owner.Name + " waited around...";
    }
}

