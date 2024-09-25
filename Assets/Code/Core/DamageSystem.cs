using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//WIP
public class AttackTransaction
{
    public DR_Entity instigator;
    public List<DR_Entity> targets;
}

public class DamageEvent
{
    public DR_Entity attacker;
    public DR_Entity target;
    public DR_Entity item;

    int baseDamage = 0;
    public float multiplier = 1.0f;
    public int addedDamage = 0;

    public bool killed = false;

    public DamageEvent(DR_Entity attacker, DR_Entity target, int baseDamage, DR_Entity item = null){
        this.attacker = attacker;
        this.target = target;
        this.item = item;
        this.baseDamage = baseDamage;
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

    // TODO: pass in originating action here so any animations can be added to it?
    public static void CreateAttackTransaction(DR_Entity instigator, List<DR_Entity> targets, float baseDamageMod = 1.0f){
        AttackTransaction attackTransaction = new AttackTransaction();
        attackTransaction.instigator = instigator;
        attackTransaction.targets = targets;

        AttackTransactionEvent startEvent = new AttackTransactionEvent {
            attackTransaction = attackTransaction
        };

        // Targets may be expanded upon by this
        instigator.OnAttackTransactionCreated?.Invoke(startEvent);

        foreach(var target in attackTransaction.targets){
            int damage = Mathf.CeilToInt(instigator.GetComponent<LevelComponent>().stats.strength * baseDamageMod);
            HandleAttack(DR_GameManager.instance, instigator, target, damage);
        }
    }


    // OLD
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

            if(!targetHealthComp.IsAlive()){
                damageEvent.killed = true;

                LevelComponent targetLevel = target.GetComponent<LevelComponent>();
                LevelComponent attackerLevel = attacker.GetComponent<LevelComponent>();
                if (targetLevel != null && attackerLevel != null){
                    attackerLevel.GiveExp(targetLevel.stats.expGiven);
                }

                //Handle blood
                DR_Cell cell = gm.CurrentMap.GetCell(target.Position);
                cell.AddBlood(Mathf.Max(Mathf.CeilToInt(targetHealthComp.maxHealth * 0.25f), 1));

                //TODO: Is this still needed?
                target.noLongerValid = true;
                gm.CurrentMap.RemoveActor(target);
            }

            //TODO: should put earlier so that abilities can interfere?
            AttackEvent attackEvent = new AttackEvent {
                owner = attacker,
                target = target,
                damageDealt = damageEvent.GetResultingDamage()
            };
            attacker.OnAttackOther?.Invoke(attackEvent);

            AttackEvent attackedEvent = new AttackEvent {
                owner = attacker,
                target = target,
                damageDealt = damageEvent.GetResultingDamage()
            };
            target.OnAttacked?.Invoke(attackedEvent);
        }

        return damageEvent;
    }
}
