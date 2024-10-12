using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : DR_Component
{   
    [Copy]
    public int maxHealth = 10;
    [Copy]
    public int currentHealth = 0;

    public Action<DR_Event> OnHealthChanged;

    //TODO: should this be in its own component or elsewhere?
    public List<StatusEffect> statusEffects = new();

    public void TickStatusEffects(float amount){
        for (int i = 0; i < statusEffects.Count; i++){
            StatusEffect effect = statusEffects[i];
            effect.Tick(amount, out bool removed);
            if (removed){
                i--;
            }
        }
    }

    public void ApplyStatsModifiers(StatsModifier statsModifier){

        foreach(var statusEffect in statusEffects){
            statusEffect.ApplyStatModifiers(statsModifier);
        }
    }

    public void AddStatusEffect(StatusEffect statusEffect){
        var t = statusEffect.GetType();

        foreach (var effect in statusEffects){
            if (effect.GetType() == t){
                // for now, don't allow duplicates and just return
                return;
            }
        }
        statusEffect.owner = Entity;
        statusEffects.Add(statusEffect);
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffects.Remove(statusEffect);
    }

    // Temp (probably)
    public bool HasStatusEffect(Type statusType){
        foreach (var status in statusEffects){
            if (status.GetType() == statusType){
                return true;
            }
        }
        return false;
    }

    public HealthComponent(){}

    public HealthComponent(int MaxHealth){
        maxHealth = MaxHealth;
    }

    public override void OnComponentAdded()
    {
        base.OnComponentAdded();
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(
            new HealthChangeEvent(){owner = Entity, healthComp = this}
        );
    }

    public void TakeDamage(int amount){
        currentHealth = Mathf.Clamp(currentHealth-amount, 0, maxHealth);
        OnHealthChanged?.Invoke(
            new HealthChangeEvent(){owner = Entity, healthComp = this}
        );
    }

    public int Heal(int amount){
        int newHealth = Mathf.Clamp(currentHealth+amount, 0, maxHealth);
        int recovered = newHealth - currentHealth;
        currentHealth = newHealth;
        OnHealthChanged?.Invoke(
            new HealthChangeEvent(){owner = Entity, healthComp = this, delta = recovered}
        );
        return recovered;
    }

    public int HealFully() {
        return Heal(maxHealth - currentHealth);
    }

    public bool IsAlive(){
        return currentHealth > 0;
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        Heal(amount);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        int healthChange = newMaxHealth - maxHealth;
        maxHealth = newMaxHealth;
        if (healthChange > 0){
            Heal(healthChange);
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public override string GetDetailsDescription()
    {
        return "HP: " + currentHealth + " / " + maxHealth;
    }
}  
