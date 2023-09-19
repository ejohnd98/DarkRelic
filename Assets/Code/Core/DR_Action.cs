using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Action {
    public bool loggable = false;
    public DR_Entity owner;

    //TODO: create more robust way of multi step actions (array of input structs/classes?)
    // could later autopopulate that array to streamline using items and targeting things
    public bool requiresFurtherInput = false;
    public bool hasReceivedFurtherInput = false;

    public virtual bool Perform(DR_GameManager gm){
        //TODO: only log if action was successful (and log a different message if not?)
        LogSystem.instance.AddLog(this);
        return false;
    }

    public virtual string GetLogText(){
        return "";
    }

    public virtual bool GiveAdditionalInput(DR_GameManager gm, Vector2Int pos){
        Debug.LogError("GiveAdditionalInput not implemented!");
        return false;
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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        return gm.CurrentMap.MoveActor(owner, pos, true);
    }
}

public class AttackAction : DR_Action {
    public HealthComponent target;

    public AttackAction (HealthComponent target, DR_Entity attacker = null){
        this.target = target;
        this.owner = attacker;
        loggable = false; //handle this separately for attacks
    }

    public override string GetLogText(){
        return owner.Name + " attacked " + target.Entity.Name + "!";
    }

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        //todo: get damage amount from some component (melee component?)
        // and/or assign damage to action upon creating it?
        DamageSystem.HandleAttack(gm, owner, target, 1);

        //TODO: check if this is still needed here
        if (!target.IsAlive()){
            gm.CurrentMap.RemoveActor(target.Entity);
            target.Entity.DestroyEntity();
        }

        AttackAnimComponent attackAnim = owner.AddComponent<AttackAnimComponent>(new AttackAnimComponent());
        attackAnim.SetAnim(target.Entity.Position);

        return true;
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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        DR_Map dest = gm.CurrentDungeon.GetNextMap(stairs.goesDeeper);
        gm.MoveLevels(gm.CurrentMap, dest, stairs.goesDeeper);
        return true;
    }
}

public class DoorAction : DR_Action {
    public DoorComponent target;

    public DoorAction (DoorComponent target, DR_Entity opener = null){
        this.target = target;
        this.owner = opener;
    }

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        target.ToggleOpen();
        return true;
    }
}

public class GoalAction : DR_Action {
    public GoalComponent target;

    public GoalAction (GoalComponent target, DR_Entity opener = null){
        this.target = target;
        this.owner = opener;
        loggable = true;
    }

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        gm.OnGameWon();
        return true;
    }

    public override string GetLogText(){
        return owner.Name + " has claimed victory!";
    }
}

public class ItemAction : DR_Action {
    public DR_Entity target;
    public DR_Entity item;

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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        ItemComponent itemComponent = item.GetComponent<ItemComponent>();
        if (itemComponent != null){
            return itemComponent.UseItem(gm, owner, target);
        }
        return false;
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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        InventoryComponent inventory = owner.GetComponent<InventoryComponent>();
        if (inventory != null){
            return equip? inventory.EquipItem(item) : inventory.UnequipItem(item);
        }
        return false;
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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        InventoryComponent inventory = owner.GetComponent<InventoryComponent>();
        if (inventory != null){
            bool addedItem = inventory.AddItem(item);
            if (addedItem){
                gm.CurrentMap.RemoveItem(item);
            }
            return addedItem;
        }else{
            Debug.LogError("Inventory is invalid!");
        }
        return false;
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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        InventoryComponent inventory = owner.GetComponent<InventoryComponent>();
        if (inventory != null){
            if (gm.CurrentMap.GetItemAtPosition(owner.Position) == null){
                inventory.RemoveItem(item);
                return gm.CurrentMap.AddItem(item, owner.Position);
            }
            
        }else{
            Debug.LogError("Inventory is invalid!");
        }
        return false;
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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        return true;
    }

    public override string GetLogText(){
        return owner.Name + " waited around...";
    }
}

