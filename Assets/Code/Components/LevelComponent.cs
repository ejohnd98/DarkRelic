using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Mathematics;

public class Stats{
    public int strength = 1;
    public int maxHealth = 1;
    public int expGiven = 1;
    public float turnLength = 1.0f;
}

public class StatModifier{
    public float multiplier = 1.0f;
    public float addedValue = 0.0f;
}

public class StatsModifier{
    public StatModifier strength = new();
    public StatModifier maxHealth = new();
    public StatModifier expGiven = new();
    public StatModifier turnLength = new();

    public Stats GetResultingStats(Stats stats){
        return new Stats(){
            strength = Mathf.CeilToInt((stats.strength + strength.addedValue) * strength.multiplier),
            maxHealth = Mathf.CeilToInt((stats.maxHealth + maxHealth.addedValue) * maxHealth.multiplier),
            expGiven = Mathf.CeilToInt((stats.expGiven + expGiven.addedValue) * expGiven.multiplier),
            turnLength = (stats.turnLength + turnLength.addedValue) * turnLength.multiplier
        };
    }
}

public class LevelComponent : DR_Component
{
    public event Action<LevelComponent> OnLevelUp;

    [Copy]
    public int level = 1;
    [Copy]
    public float expScale = 1.0f;
    [Copy]
    public int currentExp = 0;

    [Copy]
    public float healthScale = 1.0f;
    [Copy]
    public float strengthScale = 1.0f;
    [Copy]
    public float turnLengthScale = 1.0f;

    // This should not be copied as it will be created in OnComponentAdded
    public Stats stats;

    public LevelComponent(){}

    // TODO: because this component is created before being added, the owning entity is not valid yet
    // should figure out a better way to handle this, but for now pass it in here
    public LevelComponent(int level){
        this.level = level;
    }

    public override void OnComponentAdded()
    {
        base.OnComponentAdded();
        stats = new Stats();
        UpdateStats();
    }

    public void GiveExp(int exp){
        currentExp += exp;
    }

    public void AdvanceLevel(){
        int requiredExp = GetRequiredExpForLevelUp(level);
        currentExp-=requiredExp;

        level++;
        UpdateStats();
        OnLevelUp?.Invoke(this);
    }

    public bool RequiresLevelUp(){
        int requiredExp = GetRequiredExpForLevelUp(level);
        return (currentExp >= requiredExp);
    }

    public static int GetRequiredExpForLevelUp(int currentLevel){
        int levelUpBase = 200;
        int levelUpFactor = 150;

        return levelUpBase + (levelUpFactor * (currentLevel-1));
    }

    public void UpdateStats() {

        stats = GetLevelStats(level, this);

        if (Entity.GetComponent<AbilityComponent>() is AbilityComponent abilityComponent){
            StatsModifier modifier = abilityComponent.GetStatsModifier();
            stats = modifier.GetResultingStats(stats);
        }
        
        HealthComponent healthComponent = Entity.GetComponent<HealthComponent>();
        healthComponent.SetMaxHealth(stats.maxHealth);
    }

    public static Stats GetLevelStats(int level, LevelComponent comp) {
        Stats level1Stats = new()
        {
            strength = 1,
            maxHealth = 5,
            expGiven = 100
        };

        Stats level50Stats = new()
        {
            strength = 80,
            maxHealth = 400,
            expGiven = 1000
        };

        float levelFraction = (level-1) / 100.0f;

        return new Stats {
            strength = Mathf.CeilToInt(comp.strengthScale * Mathf.Lerp(level1Stats.strength, level50Stats.strength, levelFraction)),
            maxHealth = Mathf.CeilToInt(comp.healthScale * Mathf.Lerp(level1Stats.maxHealth, level50Stats.maxHealth, levelFraction)),
            expGiven = Mathf.CeilToInt(comp.expScale * Mathf.Lerp(level1Stats.expGiven, level50Stats.expGiven, levelFraction)),
            turnLength = comp.turnLengthScale * Mathf.Lerp(level1Stats.turnLength, level50Stats.turnLength, levelFraction)
        };
    }

    public override string GetDetailsDescription()
    {
        return "Level: " + level + "\nStr: " + stats.strength;
    }
}
