using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem
{
    public static void HandleAttack(DR_GameManager gm, HealthComponent target, int damage){
        if(target != null){
            target.TakeDamage(damage);
            if(!target.IsAlive()){
                //TODO: make this better. have class to handle "garbage collecting" of entities
                target.Entity.noLongerValid = true;
                gm.CurrentMap.RemoveActor(target.Entity);
                target.Entity.DestroyEntity();
            }
        }
    }
}
