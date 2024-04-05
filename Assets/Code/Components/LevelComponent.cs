using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Mathematics;

public class Stats{
    public int strength = 1;
    public int maxHealth = 1;
    public int expGiven = 1;
}

public class LevelComponent : DR_Component
{
    public event Action<LevelComponent> OnLevelUp;

    [Copy]
    public int level = 1;
    [Copy]
    public float expScale = 1.0f;
    [Copy]
    int currentExp = 0;

    //TODO: expand on this concept and have different enemies just have a scale applied to some generic set of stats
    [Copy]
    public float healthScale = 1.0f;
    [Copy]
    public float strengthScale = 1.0f;

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
        
        HealthComponent healthComponent = Entity.GetComponent<HealthComponent>();
        healthComponent.SetMaxHealth(stats.maxHealth);
    }

    public static Stats GetLevelStats(int level, LevelComponent comp) {
        Stats level1Stats = new Stats();
        level1Stats.strength = 1;
        level1Stats.maxHealth = 5;
        level1Stats.expGiven = 300;

        Stats level50Stats = new Stats();
        level50Stats.strength = 100;
        level50Stats.maxHealth = 500;
        level50Stats.expGiven = 1500;
        
        float levelFraction = (level-1) / 100.0f;

        return new Stats {
            strength = Mathf.CeilToInt(comp.strengthScale * Mathf.Lerp(level1Stats.strength, level50Stats.strength, levelFraction)),
            maxHealth = Mathf.CeilToInt(comp.healthScale * Mathf.Lerp(level1Stats.maxHealth, level50Stats.maxHealth, levelFraction)),
            expGiven = Mathf.CeilToInt(comp.expScale * Mathf.Lerp(level1Stats.expGiven, level50Stats.expGiven, levelFraction))
        };
    }
}
