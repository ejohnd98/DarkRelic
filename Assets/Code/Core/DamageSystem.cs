using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem
{
    public static void HandleAttack(DR_GameManager gm, DR_Entity attacker, HealthComponent target, int damage){
        if(target != null){
            target.TakeDamage(damage);
            if(!target.IsAlive()){

                LevelComponent targetLevel = target.Entity.GetComponent<LevelComponent>();
                LevelComponent attackerLevel = attacker.GetComponent<LevelComponent>();
                if (targetLevel != null && attackerLevel != null){
                    attackerLevel.GiveExp(targetLevel.expGiven);
                }

                //TODO: make this better. have class to handle "garbage collecting" of entities
                target.Entity.noLongerValid = true;
                gm.CurrentMap.RemoveActor(target.Entity);
                target.Entity.DestroyEntity();
            }
        }
    }
}
