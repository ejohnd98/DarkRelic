using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : DR_Component
{   
    public int maxHealth = 10;
    public int currentHealth = 0;

    public HealthComponent(int MaxHealth){
        maxHealth = MaxHealth;
        currentHealth = maxHealth;
    }

    public bool IsAlive(){
        return currentHealth > 0;
    }
}  
