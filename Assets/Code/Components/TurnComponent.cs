using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnComponent : DR_Component
{
    //TODO: move this to be based off a speed stat?
    [Copy]
    public int ActionDebt = 1;
    float CurrentDebt = 0.0f;

    public bool CanTakeTurn(){
        return CurrentDebt >= 0;
    }

    public void SpendTurn()
    {
        float debt = ActionDebt;
        if (Entity.GetComponent<InventoryComponent>() is InventoryComponent inventory
            && inventory.RelicInventory.ContainsKey(RelicType.SPEED_RELIC))
        {
            debt *= Mathf.Clamp(1.0f - (inventory.RelicInventory[RelicType.SPEED_RELIC] * 0.05f), 0.1f, 1.0f);
            Debug.Log("Speed Orig: " + ActionDebt + ", modified: " + debt);
        }
        CurrentDebt -= debt;
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
