using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : DR_Component
{   
    public int maxHealth = 10;
    public int currentHealth = 0;

    public HealthComponent(){}

    public HealthComponent(int MaxHealth){
        maxHealth = MaxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount){
        currentHealth = Mathf.Clamp(currentHealth-amount, 0, maxHealth);
    }

    public int Heal(int amount){
        int newHealth = Mathf.Clamp(currentHealth+amount, 0, maxHealth);
        int recovered = newHealth - currentHealth;
        currentHealth = newHealth;
        return recovered;
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
}  
