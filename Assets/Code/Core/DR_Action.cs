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

    public bool hasStarted = false;
    public bool hasFinished = false;
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

    public virtual void StartAction(DR_GameManager gm){
        hasStarted = true;
    }

    public virtual void ActionStep(DR_GameManager gm, float deltaTime){
        if (!hasStarted || hasFinished){
            return;
        }
        EndAction(gm);

        //TODO: refactor action to have a list of subactions (or some other name)
        // These would be the action itself, plus any animations. they would be executed sequentially
        // use events for anims to call some TriggerNextSubaction method as they finish?
        // for attacks, this event could be partway through (ie when sprite has just bumped into target)
        // this will sync up removing sprites with attack anim
    }

    public virtual void EndAction(DR_GameManager gm){
        if (wasSuccess){
            //LogSystem.instance.AddLog(this);
        }
        hasFinished = true;
    }

    public virtual string GetLogText(){
        return "";
    }

    //TODO: DR_Animation: move existing animation components into this new class
    // these animations will be stored in a list in a new AnimationSystem class.
    // the renderer will iterate through them, but when possible perform multiple at once (can do that later though)

    //TODO: make these work:
    public virtual DR_Animation GetStartAnimation(){
        return null;
    }

    public virtual DR_Animation GetEndAnimation(){
        return null;
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

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);

        Vector2Int posA = owner.Position;
        if(!gm.CurrentMap.CanMoveActor(owner, pos)){
            wasSuccess = false;
            EndAction(gm);
        }

        if (gm.CurrentMap.IsVisible[owner.Position.y, owner.Position.x]){
            //moveAnim = owner.AddComponent<MoveAnimation>(new());
            //moveAnim.SetAnim(pos);
            //AnimationSystem.AddAnimation(moveAnim, owner);
            
            //SoundSystem.instance.PlaySound("move2");

            // moveAnim.AnimFinished += (DR_Animation moveAnim) => {
            //     EndAction(gm);
            // };

            gm.CurrentMap.MoveActor(owner, pos);
            //TODO: later have animation system return a bool if the action can be completed without the animation
            // this will let multiple enemies animate at once
            EndAction(gm);
        }else{
            gm.CurrentMap.MoveActor(owner, pos);
            EndAction(gm);
        }
    }

    public override void ActionStep(DR_GameManager gm, float deltaTime)
    {
        if (!hasStarted || hasFinished){
            return;
        }
    }

    public override void EndAction(DR_GameManager gm)
    {
        base.EndAction(gm);
        if (!wasSuccess){
            return;
        }

        DR_Cell cell = gm.CurrentMap.GetCell(pos);

        if (cell.blood > 0){
            if (owner.GetComponent<InventoryComponent>() is InventoryComponent inventory && inventory.canCollectBlood){
                //TODO: later have a handler for this as relics will affect stuff here when getting blood

                inventory.AddBlood(cell.blood);
                UISystem.instance.RefreshInventoryUI();
                cell.blood = 0;
                //DR_Renderer.instance.SetCellBloodState(pos, cell);
            }
        }


        
    }
}

public class AttackAction : DR_Action {
    public HealthComponent target;
    public AttackAnimation attackAnim;

    public AttackAction (HealthComponent target, DR_Entity attacker = null){
        this.target = target;
        this.owner = attacker;
        loggable = false; //handle this separately for attacks
    }

    public override string GetLogText(){
        return owner.Name + " attacked " + target.Entity.Name + "!";
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);

        //attackAnim = owner.AddComponent<AttackAnimation>(new());
        //attackAnim.SetAnim(target.Entity.Position);
        //AnimationSystem.AddAnimation(attackAnim, owner);

        //attackAnim.AnimHalfway += (DR_Animation moveAnim) => {
            // int baseDamage = owner.GetComponent<LevelComponent>().stats.strength;

            // DamageSystem.HandleAttack(gm, owner, target, baseDamage);

            // //TODO: check if this is still needed here
            // if (!target.IsAlive()){
            //     gm.CurrentMap.RemoveActor(target.Entity);
            //     target.Entity.DestroyEntity();
            // }
        //};

        int baseDamage = owner.GetComponent<LevelComponent>().stats.strength;

        DamageSystem.HandleAttack(gm, owner, target, baseDamage);

        //TODO: check if this is still needed here
        if (!target.IsAlive()){
            gm.CurrentMap.RemoveActor(target.Entity);
            target.Entity.DestroyEntity();
        }

        EndAction(gm);

        // attackAnim.AnimFinished += (DR_Animation moveAnim) => {
        //     EndAction(gm);
        // };
    }

    public override void ActionStep(DR_GameManager gm, float deltaTime)
    {
        if (!hasStarted || hasFinished){
            return;
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

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
        DR_Map dest = gm.CurrentDungeon.GetNextMap(stairs.goesDeeper);
        gm.MoveLevels(gm.CurrentMap, dest, stairs.goesDeeper, true);
        //SoundSystem.instance.PlaySound(stairs.goesDeeper ? "descend" : "ascend");
        //return true;
    }

    public override void ActionStep(DR_GameManager gm, float deltaTime) {
        if (!hasStarted || hasFinished){
            return;
        }

        if (!DR_GameManager.instance.isFadeActive) {
            EndAction(gm);
        }
    }
}

public class DoorAction : DR_Action {
    public DoorComponent target;

    public DoorAction (DoorComponent target, DR_Entity opener = null){
        this.target = target;
        this.owner = opener;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
        target.ToggleOpen();
        //SoundSystem.instance.PlaySound("door");
        EndAction(gm);
    }
}

public class GoalAction : DR_Action {
    public GoalComponent target;

    public GoalAction (GoalComponent target, DR_Entity opener = null){
        this.target = target;
        this.owner = opener;
        loggable = true;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
        //SoundSystem.instance.PlaySound("altar");
        gm.OnGameWon();
        //return true;
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

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
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
            //SoundSystem.instance.PlaySound("altar");
        }
    }
}

public class ItemAction : DR_Action {
    public DR_Entity target;
    public DR_Entity item;

    public MoveAnimation moveAnim;

    public ItemAction (DR_Entity item, DR_Entity user, DR_Entity target){
        this.item = item;
        this.owner = user;
        this.target = target;

        ItemComponent itemComponent = item.GetComponent<ItemComponent>();
        if (itemComponent != null){
            if (itemComponent.requireFurtherInputOnUse){

                actionInputs.Add(new(
                    //Input validation:
                    pos => {
                        DR_Entity newTarget = DR_GameManager.instance.CurrentMap.GetActorAtPosition(pos);
                        return newTarget != null;
                    },
                    "Please select a target, or press ESC to cancel."
                ));
            }
        }

        loggable = true;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);

        if (actionInputs.Count > 0){
            target = DR_GameManager.instance.CurrentMap.GetActorAtPosition(actionInputs[0].inputValue);
        }

        MagicConsumableComponent magic = item.GetComponent<MagicConsumableComponent>();
        if (magic != null){
            Vector2Int targetPos;
            if (magic.targetClosest){
                targetPos = magic.GetTargetPosition(gm, owner, null);
            }else{
                targetPos = target.Position;
            }
            moveAnim = EntityFactory.CreateProjectileEntityAtPosition(
                magic.projectileSprite, "Projectile", owner.Position,targetPos, magic.color).GetComponent<MoveAnimation>();
        }
    }

    public override void ActionStep(DR_GameManager gm, float deltaTime)
    {
        if (!hasStarted || hasFinished){
            return;
        }
        if (moveAnim == null || !moveAnim.isAnimating){
            EndAction(gm);
        }
    }

    public override void EndAction(DR_GameManager gm)
    {
        ItemComponent itemComponent = item.GetComponent<ItemComponent>();
        if (itemComponent != null){
            itemComponent.UseItem(gm, owner, target);
        }
        base.EndAction(gm);
    }

    public override string GetLogText(){
        //todo: get this from the item itself
        return owner.Name + " used " + item.Name + ((owner == target)? "" : " on " + target.Name);
    }
}

public class ChangeEquipmentAction : DR_Action {
    public DR_Entity item;
    public bool equip;

    public ChangeEquipmentAction (DR_Entity item, DR_Entity user, bool equip){
        this.item = item;
        this.owner = user;
        this.equip = equip;

        loggable = true;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
        InventoryComponent inventory = owner.GetComponent<InventoryComponent>();
        if (inventory != null){
            bool success = equip? inventory.EquipItem(item) : inventory.UnequipItem(item);
        }
    }

    public override string GetLogText(){
        return owner.Name + (equip ? " equipped " : " unequipped ") + item.Name;
    }
}

public class PickupAction : DR_Action {
    public DR_Entity item;

    public PickupAction (DR_Entity item, DR_Entity user){
        this.item = item;
        this.owner = user;
        loggable = true;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
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
        return;
    }

    public override string GetLogText(){
        return owner.Name + " picked up " + item.Name;
    }
}

public class DropAction : DR_Action {
    public DR_Entity item;

    public DropAction (DR_Entity item, DR_Entity user){
        this.item = item;
        this.owner = user;
        loggable = true;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
        InventoryComponent inventory = owner.GetComponent<InventoryComponent>();
        if (inventory != null){
            if (gm.CurrentMap.GetItemAtPosition(owner.Position) == null){
                inventory.RemoveItem(item);
                gm.CurrentMap.AddItem(item, owner.Position);
            }
            
        }else{
            Debug.LogError("Inventory is invalid!");
        }
        return;
    }

    public override string GetLogText(){
        return owner.Name + " dropped " + item.Name;
    }
}


public class WaitAction : DR_Action {

    public WaitAction(DR_Entity owner, bool logAction = false){
        this.owner = owner;
        loggable = logAction;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);
        return;
    }

    public override string GetLogText(){
        return owner.Name + " waited around...";
    }
}

