using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DR_Action {
    public bool loggable = false;
    public DR_Entity owner;

    public bool requiresFurtherInput = false;
    public bool hasReceivedFurtherInput = false;

    public bool hasStarted = false;
    public bool hasFinished = false;
    public bool wasSuccess = true;

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
        // use events for anims to call some TriggerNextSubaction method as thye finish?
        // for attacks, this event could be partway through (ie when sprite has just bumped into target)
        // this will sync up removing sprites with attack anim
    }

    public virtual void EndAction(DR_GameManager gm){
        if (wasSuccess){
            LogSystem.instance.AddLog(this);
        }
        hasFinished = true;
    }

    public virtual string GetLogText(){
        return "";
    }

    public virtual bool GiveAdditionalInput(DR_GameManager gm, Vector2Int pos){
        Debug.LogError("GiveAdditionalInput not implemented!");
        return false;
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
            moveAnim = owner.AddComponent<MoveAnimation>(new());
            moveAnim.SetAnim(pos);
            AnimationSystem.AddAnimation(moveAnim);

            moveAnim.AnimFinished += (DR_Animation moveAnim) => {
                EndAction(gm);
            };

            gm.CurrentMap.MoveActor(owner, pos, false);
            //TODO: later have animation system return a bool if the action can be completed without the animation
            // this will let multiple enemies animate at once
        }else{
            gm.CurrentMap.MoveActor(owner, pos, false);
            EndAction(gm);
        }
    }

    public override void ActionStep(DR_GameManager gm, float deltaTime)
    {
        if (!hasStarted || hasFinished){
            return;
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

        attackAnim = owner.AddComponent<AttackAnimation>(new());
        attackAnim.SetAnim(target.Entity.Position);
        AnimationSystem.AddAnimation(attackAnim);

        attackAnim.AnimHalfway += (DR_Animation moveAnim) => {
            int baseDamage = owner.GetComponent<LevelComponent>().stats.strength;

            DamageSystem.HandleAttack(gm, owner, target, baseDamage);

            //TODO: check if this is still needed here
            if (!target.IsAlive()){
                gm.CurrentMap.RemoveActor(target.Entity);
                target.Entity.DestroyEntity();
            }
        };

        attackAnim.AnimFinished += (DR_Animation moveAnim) => {
            EndAction(gm);
        };
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
        gm.MoveLevels(gm.CurrentMap, dest, stairs.goesDeeper);
        //return true;
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
        gm.OnGameWon();
        //return true;
    }

    public override string GetLogText(){
        return owner.Name + " has claimed victory!";
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
            requiresFurtherInput = itemComponent.requireFurtherInputOnUse;
        }

        loggable = true;
    }

    public override void StartAction(DR_GameManager gm){
        base.StartAction(gm);

        MagicConsumableComponent magic = item.GetComponent<MagicConsumableComponent>();
        if (magic != null){
            Vector2Int targetPos = magic.GetTargetPosition(gm, owner, null);
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

    public override bool GiveAdditionalInput(DR_GameManager gm, Vector2Int pos){
        DR_Entity newTarget = gm.CurrentMap.GetActorAtPosition(pos);
        if (newTarget != null){
            target = newTarget;
            hasReceivedFurtherInput = true;
            return true;
        }
        return false;
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

