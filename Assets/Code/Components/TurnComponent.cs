using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent : DR_Component
{
    [Copy]
    public int ActionDebt = 1;
    int CurrentDebt = 0;

    public bool CanTakeTurn(){
        return CurrentDebt >= 0;
    }

    public void SpendTurn(){
        CurrentDebt -= ActionDebt;
    }

    public void RecoverDebt(int amount){
        CurrentDebt = Mathf.Min(CurrentDebt + amount, 0);
    }

    public override void OnComponentRemoved()
    {
        base.OnComponentRemoved();

        //todo: figure out a better way to do this
        DR_GameManager.instance.turnSystem.RemoveEntityTurnComponent(this);
    }
}
