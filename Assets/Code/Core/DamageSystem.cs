using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageEvent
{
    public DR_Entity attacker;
    public DR_Entity target;
    public DR_Entity item;

    public UnityEvent OnAttack, OnKill;
    //TODO: OnCrit, OnMiss

    int baseDamage = 0;
    public float multiplier = 1.0f;
    public int addedDamage = 0;

    public bool killed = false;

    public DamageEvent(DR_Entity attacker, DR_Entity target, int baseDamage, DR_Entity item = null){
        this.attacker = attacker;
        this.target = target;
        this.item = item;
        this.baseDamage = baseDamage;

        OnAttack = new UnityEvent();
        OnKill = new UnityEvent();
    }

    public int GetResultingDamage(){
        return (int)((baseDamage + addedDamage) * multiplier);
    }

    public string GetLogText(){
        if (!killed){
            return attacker.Name + " dealt " + GetResultingDamage() + " damage to " + target.Name;
        }else{
            return attacker.Name + " dealt " + GetResultingDamage() + " damage and KILLED " + target.Name;
        }
    }
}

public class DamageSystem
{
    public static DamageEvent HandleAttack(DR_GameManager gm, DR_Entity attacker, DR_Entity target, int damage)
    {
        // add a flag to allow blood splatter on any attack? Bludgeon ability would just always set this to true


        var targetHealthComp = target.GetComponent<HealthComponent>();
        int modifiedDamage = damage;
        DamageEvent damageEvent = new DamageEvent(attacker, target, modifiedDamage);

        if(targetHealthComp != null){
            //Debug testing
            if (attacker.HasComponent<PlayerComponent>() && Input.GetKey(KeyCode.LeftShift)) {
                damageEvent.addedDamage = 999;
            }
            
            
            targetHealthComp.TakeDamage(damageEvent.GetResultingDamage());

            damageEvent.OnAttack?.Invoke();


            if(!targetHealthComp.IsAlive()){
                damageEvent.killed = true;

                LevelComponent targetLevel = target.GetComponent<LevelComponent>();
                LevelComponent attackerLevel = attacker.GetComponent<LevelComponent>();
                if (targetLevel != null && attackerLevel != null){
                    attackerLevel.GiveExp(targetLevel.stats.expGiven);
                }

                damageEvent.OnKill?.Invoke();

                //Handle blood
                DR_Cell cell = gm.CurrentMap.GetCell(target.Position);
                cell.AddBlood(Mathf.Max(Mathf.CeilToInt(targetHealthComp.maxHealth * 0.25f), 1));

                //TODO: Is this still needed?
                target.noLongerValid = true;
                gm.CurrentMap.RemoveActor(target);
            }

            AttackEvent attackEvent = new AttackEvent {
                owner = attacker,
                target = target,
                damageDealt = damageEvent.GetResultingDamage()
            };
            attacker.OnAttackOther?.Invoke(attackEvent);
        }

        return damageEvent;
    }
}
