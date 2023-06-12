using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Action {
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
}

