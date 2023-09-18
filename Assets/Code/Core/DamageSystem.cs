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

    public DamageEvent(DR_Entity attacker, DR_Entity target, DR_Entity item = null){
        this.attacker = attacker;
        this.target = target;
        this.item = item;

        OnAttack = new UnityEvent();
        OnKill = new UnityEvent();

        baseDamage = 1; //TODO: figure out where this comes from (str/atk stat probably)
    }

    public int GetResultingDamage(){
        return (int)((baseDamage + addedDamage) * multiplier);
    }
}

public class DamageSystem
{
    public static void HandleAttack(DR_GameManager gm, DR_Entity attacker, HealthComponent target, int damage){

        DamageEvent damageEvent = new DamageEvent(attacker, target.Entity);
        InventoryComponent attackerInventory = attacker.GetComponent<InventoryComponent>();
        if (attackerInventory != null){
            foreach (DR_Entity item in attackerInventory.items){
                EquippableComponent equippable = item.GetComponent<EquippableComponent>();
                if (equippable == null || !equippable.isEquipped){
                    continue;
                }
                foreach (DR_Modifier modifier in equippable.modifiers){
                    damageEvent.OnAttack.AddListener(() => {modifier.OnAttack(gm, damageEvent);});
                    damageEvent.OnKill.AddListener(() => {modifier.OnKill(gm, damageEvent);});
                    modifier.ApplyAttackerDamageChanges(gm, damageEvent);
                }
            }
        }

        InventoryComponent targetInventory = target.Entity.GetComponent<InventoryComponent>();
        if (targetInventory != null){
            foreach (DR_Entity item in targetInventory.items){
                EquippableComponent equippable = item.GetComponent<EquippableComponent>();
                if (equippable == null || !equippable.isEquipped){
                    continue;
                }
                foreach (DR_Modifier modifier in equippable.modifiers){
                    damageEvent.OnAttack.AddListener(() => {modifier.OnHit(gm, damageEvent);});
                    damageEvent.OnKill.AddListener(() => {modifier.OnKilled(gm, damageEvent);});
                    modifier.ApplyDefenderDamageChanges(gm, damageEvent);
                }
            }
        }

        if(target != null){
            target.TakeDamage(damageEvent.GetResultingDamage());

            damageEvent.OnAttack?.Invoke();

            if(!target.IsAlive()){
                
                LevelComponent targetLevel = target.Entity.GetComponent<LevelComponent>();
                LevelComponent attackerLevel = attacker.GetComponent<LevelComponent>();
                if (targetLevel != null && attackerLevel != null){
                    attackerLevel.GiveExp(targetLevel.expGiven);
                }

                damageEvent.OnKill?.Invoke();

                //TODO: make this better. have class to handle "garbage collecting" of entities
                target.Entity.noLongerValid = true;
                gm.CurrentMap.RemoveActor(target.Entity);
                target.Entity.DestroyEntity();
            }
        }
    }
}
