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
    public static DamageEvent HandleAttack(DR_GameManager gm, DR_Entity attacker, HealthComponent target, int damage)
    {

        int modifiedDamage = damage;
        if (attacker.GetComponent<InventoryComponent>() is InventoryComponent inventory
            && inventory.RelicInventory.ContainsKey(RelicType.DAMAGE_RELIC))
        {
            modifiedDamage = Mathf.CeilToInt(modifiedDamage * (1.0f + (inventory.RelicInventory[RelicType.DAMAGE_RELIC].count * 0.05f)));
            Debug.Log("Orig: " + damage + ", modified: " + modifiedDamage);
        }
        
        DamageEvent damageEvent = new DamageEvent(attacker, target.Entity, modifiedDamage);
        InventoryComponent attackerInventory = attacker.GetComponent<InventoryComponent>();
        // if (attackerInventory != null){
        //     foreach (DR_Entity item in attackerInventory.items){
        //         EquippableComponent equippable = item.GetComponent<EquippableComponent>();
        //         if (equippable == null || !equippable.isEquipped){
        //             continue;
        //         }
        //         foreach (DR_Modifier modifier in equippable.modifiers){
        //             damageEvent.OnAttack.AddListener(() => {modifier.OnAttack(gm, damageEvent);});
        //             damageEvent.OnKill.AddListener(() => {modifier.OnKill(gm, damageEvent);});
        //             modifier.ApplyAttackerDamageChanges(gm, damageEvent);
        //         }
        //     }
        // }

        // InventoryComponent targetInventory = target.Entity.GetComponent<InventoryComponent>();
        // if (targetInventory != null){
        //     foreach (DR_Entity item in targetInventory.items){
        //         EquippableComponent equippable = item.GetComponent<EquippableComponent>();
        //         if (equippable == null || !equippable.isEquipped){
        //             continue;
        //         }
        //         foreach (DR_Modifier modifier in equippable.modifiers){
        //             damageEvent.OnAttack.AddListener(() => {modifier.OnHit(gm, damageEvent);});
        //             damageEvent.OnKill.AddListener(() => {modifier.OnKilled(gm, damageEvent);});
        //             modifier.ApplyDefenderDamageChanges(gm, damageEvent);
        //         }
        //     }
        // }

        if(target != null){
            //Debug testing
            if (attacker.HasComponent<PlayerComponent>() && Input.GetKey(KeyCode.LeftShift)) {
                damageEvent.addedDamage = 999;
            }
            
            
            target.TakeDamage(damageEvent.GetResultingDamage());

            damageEvent.OnAttack?.Invoke();


            if(!target.IsAlive()){
                damageEvent.killed = true;

                LevelComponent targetLevel = target.Entity.GetComponent<LevelComponent>();
                LevelComponent attackerLevel = attacker.GetComponent<LevelComponent>();
                if (targetLevel != null && attackerLevel != null){
                    attackerLevel.GiveExp(targetLevel.stats.expGiven);
                }

                damageEvent.OnKill?.Invoke();

                //Handle blood
                DR_Cell cell = gm.CurrentMap.GetCell(target.Entity.Position);
                cell.blood += Mathf.Max(Mathf.CeilToInt(target.maxHealth * 0.25f), 1);
                cell.bloodStained = true;


                //TODO: make this better. have class to handle "garbage collecting" of entities
                target.Entity.noLongerValid = true;
                gm.CurrentMap.RemoveActor(target.Entity);
            }
        }

        return damageEvent;
    }
}
