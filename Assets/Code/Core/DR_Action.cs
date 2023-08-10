using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Action {
    public bool loggable = false;
    public DR_Entity owner;

    public virtual bool Perform(DR_GameManager gm){
        //TODO: only log if action was successful (and log a different message if not?)
        LogSystem.instance.AddLog(this);
        return false;
    }

    public virtual string GetLogText(){
        return "";
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
        loggable = true;
    }

    public override string GetLogText(){
        return owner.Name + " attacked " + target.Entity.Name + "!";
    }

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        DamageSystem.HandleAttack(target, owner);

        //TODO: do this somewhere else?
        if (!target.IsAlive()){
            gm.CurrentMap.RemoveActor(target.Entity);
        }

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

public class ItemAction : DR_Action {
    public DR_Entity target;
    public DR_Item item;

    public ItemAction (DR_Item item, DR_Entity user, DR_Entity target){
        this.item = item;
        this.owner = user;
        this.target = target;
        loggable = true;
    }

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        return item.UseItem(gm, owner, target);
    }

    public override string GetLogText(){
        //todo: get this from the item itself
        return owner.Name + " used " + item.Name;
    }
}

public class PickupAction : DR_Action {
    public DR_Item item;

    public PickupAction (DR_Item item, DR_Entity user){
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
            Debug.Log("Inventory is invalid!");
        }
        return false;
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

    public override bool Perform(DR_GameManager gm){
        base.Perform(gm);
        return true;
    }

    public override string GetLogText(){
        return owner.Name + " waited around...";
    }
}

