using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Action {
    public bool loggable = false;

    public virtual string GetLogText(){
        return "";
    }
}

public class MoveAction : DR_Action {
    public Vector2Int pos = Vector2Int.zero;

    public MoveAction (int x, int y){
        pos = new Vector2Int(x,y);
    }

    public MoveAction (Vector2Int pos){
        this.pos = pos;
    }
}

public class AttackAction : DR_Action {
    public HealthComponent target;
    public DR_Entity attacker;

    public AttackAction (HealthComponent target, DR_Entity attacker = null){
        this.target = target;
        this.attacker = attacker;
        loggable = true;
    }

    public override string GetLogText(){
        return attacker.Name + " attacked " + target.Entity.Name + "!";
    }
}

public class StairAction : DR_Action {
    public StairComponent stairs;

    public StairAction (StairComponent stairs){
        this.stairs = stairs;
        loggable = true;
    }

    public override string GetLogText(){
        return stairs.goesDeeper? "You descended down a set of stairs." : "You climbed up a set of stairs.";
    }
}

public class DoorAction : DR_Action {
    public DoorComponent target;
    public DR_Entity opener;

    public DoorAction (DoorComponent target, DR_Entity opener = null){
        this.target = target;
        this.opener = opener;
    }
}

public class WaitAction : DR_Action {

    public WaitAction(bool logAction = false){
        loggable = logAction;
    }

    public override string GetLogText(){
        return "you waited around...";
    }
}

