using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelComponent : DR_Component
{
    public event Action<LevelComponent> OnLevelUp;

    public int level = 1;
    public int expGiven = 300;

    int currentExp = 0;

    public void GiveExp(int exp){
        currentExp += exp;
    }

    public void AdvanceLevel(){
        int requiredExp = GetRequiredExpForLevelUp();
        currentExp-=requiredExp;

        level++;
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
}
