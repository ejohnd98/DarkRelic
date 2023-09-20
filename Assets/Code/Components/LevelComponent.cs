using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Mathematics;

public class Stats{
    public int strength = 1;
    public int maxHealth = 1;
}

public class LevelComponent : DR_Component
{
    public event Action<LevelComponent> OnLevelUp;

    public int level = 1;
    public int expGiven = 300;
    public Stats stats;

    int currentExp = 0;

    // TODO: because this component is created before being added, the owning entity is not valid yet
    // should figure out a better way to handle this, but for now pass it in here
    public LevelComponent(int level, DR_Entity owner){
        Entity = owner;
        this.level = level;
        stats = new Stats();
        UpdateStats();
    }

    public void GiveExp(int exp){
        currentExp += exp;
    }

    public void AdvanceLevel(){
        int requiredExp = GetRequiredExpForLevelUp();
        currentExp-=requiredExp;

        level++;
        UpdateStats();
        OnLevelUp?.Invoke(this);
    }

    public bool RequiresLevelUp(){
        int requiredExp = GetRequiredExpForLevelUp();
        return (currentExp >= requiredExp);
    }

    int GetRequiredExpForLevelUp(){
        int levelUpBase = 200;
        int levelUpFactor = 150;

        return levelUpBase + (levelUpFactor * (level-1));
    }

    void UpdateStats(){
        //TODO: this should be defined somewhere else.
        Stats level1Stats = new Stats();
        level1Stats.strength = 1;
        level1Stats.maxHealth = 5;

        Stats level50Stats = new Stats();
        level50Stats.strength = 100;
        level50Stats.maxHealth = 500;

        float levelFraction = (level-1) / 100.0f;
        stats.strength = (int)(((1.0f-levelFraction) * level1Stats.strength) + (levelFraction * level50Stats.strength));
        stats.maxHealth = (int)(((1.0f-levelFraction) * level1Stats.maxHealth) + (levelFraction * level50Stats.maxHealth));

        HealthComponent healthComponent = Entity.GetComponent<HealthComponent>();
        healthComponent.SetMaxHealth(stats.maxHealth);
    }
}
