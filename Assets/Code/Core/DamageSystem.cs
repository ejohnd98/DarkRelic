using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem
{
    public static void HandleAttack(HealthComponent target, DR_Entity attacker){
        if(target != null){
            Debug.Log(attacker.Name + " attacked " + target.Entity.Name);
            target.currentHealth--;
            if(!target.IsAlive()){
                Debug.Log(attacker.Name + " killed " + target.Entity.Name);
                //TODO: make this better. have class to handle garbage collecting
                target.Entity.noLongerValid = true;
            }
        }
    }
}
