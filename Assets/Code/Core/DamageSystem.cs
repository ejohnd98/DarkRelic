using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem
{
    // later change from
    public static void HandleAttack(DR_Actor Attacker, DR_Actor Victim){

        HealthComponent VictimHealth = Victim.GetComponent<HealthComponent>();

        if(VictimHealth != null){
            VictimHealth.currentHealth--;
            if(!VictimHealth.IsAlive()){
                Debug.Log(Attacker.Name + " killed " + Victim.Name);
            }
        }
    }
}
