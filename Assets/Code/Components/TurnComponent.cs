using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent : DR_Component
{
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
}
