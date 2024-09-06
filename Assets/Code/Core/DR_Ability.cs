using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class AbilityPropertyAttribute : PropertyAttribute { }

public abstract class DR_Ability
{
    public enum AbilityType{
        Common,
        Rare,
        Unholy,
        Cursed
    }

    public AbilityType abilityType;
    public bool triggeredByPlayer = true;
    public string abilityName = "";
    public Sprite sprite;
    public List<ActionInput> actionInputs;
    public List<DR_Entity> relatedEntities = new();

    public int count = 1;
    public int baseBloodCost = 0;

    //TODO: reflect this in UI and ability SO
    public int cooldownLength = 0; //represents cooldown turns (1 would mean you can use every other turn at most
    private int cooldown = 0;

    public DR_Entity owner;
    public string contentGuid = "";

    public virtual void OnAdded(){
        // Owner is guaranteed to be set here
        // Should only be for init stuff and not every time this ability is picked up
    }

    public virtual void TickCooldown(){
        if (cooldown > 0){
            Debug.Log("Tick cooldown on " + owner.Name + ": " + abilityName + " (" + cooldown + "->"+ (cooldown-1) +")");
            cooldown--;
        }
    }

    public virtual bool CanBePerformed(){
        if (cooldown > 0){
            return false;
        }

        if (owner.GetComponent<AIComponent>() is AIComponent aiComp 
            && aiComp.ignoreAbilityBloodCost){
            return true;
        }
        int bloodCost = GetBloodCost();
        if (bloodCost > 0){
            if (!owner.HasComponent<InventoryComponent>()){
                Debug.LogError(owner.Name + " tried to use ability ("+ abilityName +") that requires blood, but has no inventory component!");
                return false;
            }
            return owner.GetComponent<InventoryComponent>().blood + owner.GetComponent<HealthComponent>().currentHealth >= bloodCost;
        }
        return true;
    }

    public void Trigger(DR_Event e){
        if (owner.GetComponent<AIComponent>() is AIComponent aiComp 
            && aiComp.ignoreAbilityBloodCost){

        }else{
            int bloodCost = GetBloodCost();
            if (bloodCost > 0){
                var inventory = owner.GetComponent<InventoryComponent>();
                int bloodToUse = Mathf.Min(inventory.blood, bloodCost);
                inventory.SpendBlood(bloodToUse);
                if (bloodCost - bloodToUse > 0){
                    owner.GetComponent<HealthComponent>().TakeDamage(bloodCost - bloodToUse);
                    if (owner.GetComponent<HealthComponent>().currentHealth <= 0){
                        Debug.Log("Player tried to use too much blood!");
                    }
                }
            }
        }
        
        if (cooldownLength != 0){
            cooldown = cooldownLength + 1;
        }
        OnTrigger(e);
    }

    public virtual void ApplyStatModifiers(StatsModifier statsModifier){
    }

    protected virtual void OnTrigger(DR_Event e){
    }

    public void ResetInputs(){
        actionInputs = new();
        relatedEntities = new();
        SetupInputs();
    }

    public virtual List<DR_Entity> GetRelatedEntities(){
        return relatedEntities;
    }

    protected virtual void SetupInputs(){
    }

    public virtual int GetBloodCost(){
        return baseBloodCost;
    }

    public virtual string GetFormattedDescription(){
        //TODO: get a list of things to insert?
        return "Description not filled out!";
    }

    //TODO: need to think about how this can be driven by scriptable objects as that's where the sprite, name, and description would be
    // Don't necessarily want to make some elaborate generic system though, but can maybe just specify those things, and then see
    // if a dropdown can be created with the ability child class
    // Then when creating the ability at runtime it will just assign the data from the scriptable object to the ability instance

    // Where do entity specific events live?
    // They shouldn't necessarily live on DR_Entity
    // Perhaps they are added to each individual component?
    // What about stuff not tied to components, such as door opening?
    
    // Possibly it does live on the DR_Entity, and then the components trigger them
    // Then things can subscribe to them and they can be triggered without worry.

    // CURRENT TODO:
    // Move ability to scriptable object (initially can be default type?)
    // Create an ability action for those which are player triggered. Can be pretty barebones, but will have the "waiting for input" stage if needed

    // Future:
    // Have relic grant ability
    // Add events for basic stuff on Entity and some way for abilities to subscribe (or just hardcode that)
}

// TODO: generalize to projectile (or further to targeted?) ability
public class BloodBoltAbility : DR_Ability
{
    public bool killed = false;
    public DR_Entity target;

    [AbilityProperty]
    public float range = 8;

    [AbilityProperty]
    public Sprite projectileSprite;

    public BloodBoltAbility(){
        baseBloodCost = 1;
        cooldownLength = 2;
    }

    //TODO: allow targeting ground
    protected override void SetupInputs(){
        //TODO: cap range? Eventually will want to precompute targets so they can shown through UI
        actionInputs.Add(new ActionInput((Vector2Int pos) => {
            return DR_GameManager.instance.CurrentMap.GetIsVisible(pos)
            && DR_GameManager.instance.CurrentMap.GetActorAtPosition(pos) != null
            && (pos - owner.Position).magnitude < range;
        }));
    }

    protected override void OnTrigger(DR_Event e){
        Debug.Log("BloodBoltAbility Triggered with input: " + actionInputs[0].inputValue);

        DR_Entity owner = e.owner;
        target = DR_GameManager.instance.CurrentMap.GetActorAtPosition(actionInputs[0].inputValue);

        relatedEntities.Add(target);
        relatedEntities.Add(owner);

        int baseDamage = Mathf.CeilToInt(owner.GetComponent<LevelComponent>().stats.strength * GetStrengthModifier());
        var damageEvent = DamageSystem.HandleAttack(DR_GameManager.instance, owner, target, baseDamage);

        killed = damageEvent.killed;
    }

    private float GetStrengthModifier(){
        return 0.5f * Mathf.Log(count + 1, 2) - 0.25f;
    }

    public override string GetFormattedDescription(){
        float percent = GetStrengthModifier();
        return string.Format("Fires a bloody projectile that deals {0:0%} of a regular attack's damage.", percent);
    }
}

public class BloodWalkAbility : DR_Ability
{
    public BloodWalkAbility(){
        baseBloodCost = 5;
    }

    public override int GetBloodCost()
    {
        //TODO: maybe scale with distance?
        return  Mathf.CeilToInt(baseBloodCost * Mathf.Pow(0.5f, count-1));
    }

    protected override void SetupInputs(){
        //TODO BUG: require visibility too!
        actionInputs.Add(new ActionInput((Vector2Int pos) => {
            var cell = DR_GameManager.instance.CurrentMap.GetCell(pos);
            return cell.bloodStained && !cell.BlocksMovement();
            }));
    }

    protected override void OnTrigger(DR_Event e){
        relatedEntities.Add(owner);

        var pos = actionInputs[0].inputValue;
        var gm = DR_GameManager.instance;

        if(!gm.CurrentMap.CanMoveActor(owner, pos)){
            Debug.LogAssertion("BloodWalkAbility failed to execute!");
            return;
        }
        
        gm.CurrentMap.MoveActor(owner, pos);

        DR_Cell cell = gm.CurrentMap.GetCell(pos);
        cell.CollectBlood(owner);
    }
}

public class BludgeonAbility : DR_Ability
{
    public BludgeonAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.OnAttackOther += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e){

        var attackEvent = e as AttackEvent;

        var attackOrigin = attackEvent.owner.Position;
        var pos = attackEvent.target.Position;
        var gm = DR_GameManager.instance;

        // TODO: only bloody tiles if enough damage is done, and only partially on tiles if not enough to cover all
        int bloodAmountPerTile = Mathf.CeilToInt(
                Mathf.Clamp(GetBloodPercent() * attackEvent.damageDealt / 4.0f, 1, attackEvent.target.GetComponent<HealthComponent>().maxHealth)
            );

        // Kind of messy way of getting the "splatter area"
        List<Vector2Int> positions = new()
        {
            pos,
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.right,
            pos + Vector2Int.left
        };

        float distToRemove = 10000.0f;
        Vector2Int posToRemove = Vector2Int.zero;
        foreach(var cellPos in positions){
            float dist = (cellPos - attackOrigin).magnitude;
            if (dist < distToRemove){
                distToRemove = dist;
                posToRemove = cellPos;
            }
        }

        positions.Remove(posToRemove);

        foreach(var cellPos in positions){
            if (gm.CurrentMap.GetCell(cellPos).BlocksMovement(true)){
                continue;
            }
            gm.CurrentMap.GetCell(cellPos).AddBlood(bloodAmountPerTile);
        }
    }

    private float GetBloodPercent(){
        return 0.1f + (0.25f * Mathf.Log(count, 2));
    }

    public override string GetFormattedDescription(){
        float percent = GetBloodPercent();
        return string.Format("Splatters {0:0%} of dealt damage as blood around the target. (minimum 1 per tile)", percent);
    }
}

public class CrystalChaliceAbility : DR_Ability
{
    public CrystalChaliceAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.GetComponent<InventoryComponent>().OnPickedUpBlood += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e){

        var bloodEvent = e as BloodChangeEvent;

        HealthComponent healthComp = owner.GetComponent<HealthComponent>();
        int healAmount = Mathf.FloorToInt(bloodEvent.bloodDelta * GetHealPercent());
        healthComp.Heal(healAmount);
    }

    private float GetHealPercent(){
        return 0.1f + (0.3f * Mathf.Log(count, 2));
    }

    public override string GetFormattedDescription(){
        float percent = GetHealPercent();
        return string.Format("Recover {0:0%} of picked up blood as health (rounded down).", percent);
    }
}

//TODO: typo
//TODO: figure out how this stacks and a better way for it to actually hit things
public class ForecefulEntryAbility : DR_Ability
{
    public ForecefulEntryAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.GetComponent<TurnComponent>().OnActionEnd += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e){

        var actionEvent = e as ActionEvent;

        //TODO: have a way for abilities to tack on needed animations to actions?

        if (actionEvent.action is DoorAction doorAction){
            if (!doorAction.target.IsOpen()){
                // Only trigger when opening door
                return;
            }
            var gm = DR_GameManager.instance;
            int baseDamage = owner.GetComponent<LevelComponent>().stats.strength;
            foreach(var cell in gm.CurrentMap.GetAdjacentCells(doorAction.target.Entity.Position)){
                if (cell.Actor != null && cell.Actor != owner){
                    DamageSystem.HandleAttack(gm, owner, cell.Actor, Mathf.FloorToInt(baseDamage * 0.5f));
                    cell.AddBlood(1); //Temp just to show SOMETHING 
                }
            }
        }
    }

    public override string GetFormattedDescription(){
        return string.Format("To-Do: This ability currently sucks!");
    }
}

public class HealthBoostAbility : DR_Ability
{
    public HealthBoostAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.GetComponent<LevelComponent>().UpdateStats();
    }

    public override void ApplyStatModifiers(StatsModifier statsModifier)
    {
        statsModifier.maxHealth.addedValue += 10.0f * count;
    }

    public override string GetFormattedDescription(){
        return string.Format("Increases health by {0}.", (int)(count * 10.0f));
    }
}

public class StrengthBoostAbility : DR_Ability
{
    public StrengthBoostAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.GetComponent<LevelComponent>().UpdateStats();
    }

    public override void ApplyStatModifiers(StatsModifier statsModifier)
    {
        statsModifier.strength.multiplier *= (1.0f + (count * 0.05f));
    }

    public override string GetFormattedDescription(){
        return string.Format("Increases strength by {0:0%}.", (count * 0.05f));
    }
}

public class TurnSpeedBoostAbility : DR_Ability
{
    public TurnSpeedBoostAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.GetComponent<LevelComponent>().UpdateStats();
    }

    public override void ApplyStatModifiers(StatsModifier statsModifier)
    {
        statsModifier.turnLength.multiplier *= Mathf.Pow(0.9f, count);
    }

    public override string GetFormattedDescription(){
        return string.Format("Reduces turn length by {0:0%}.", (1.0f - (Mathf.Pow(0.9f, count))));
    }
}

public class BarbedAnkletAbility : DR_Ability
{
    public BarbedAnkletAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.OnMove += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e){

        var moveEvent = e as MoveEvent;
        var gm = DR_GameManager.instance;

        var inventory = owner.GetComponent<InventoryComponent>();
        var healthComp = owner.GetComponent<HealthComponent>();

        int bloodAmount = count;

        // Take from blood if possible, otherwise inflict damage
        if (inventory != null && inventory.blood > 0){
            if (inventory.blood < bloodAmount){
                bloodAmount -= inventory.blood;
                inventory.SpendBlood(inventory.blood);
                healthComp.TakeDamage(bloodAmount);
            }else{
                inventory.SpendBlood(bloodAmount);
            }
        }else{
            healthComp.TakeDamage(bloodAmount);
        }

        gm.CurrentMap.GetCell(moveEvent.startPos).AddBlood(count);
    }

    public override string GetFormattedDescription(){
        return string.Format($"Drains {count} blood for every tile walked, leaving them bloodstained.");
    }
}

public class BloodPickupRangeAbility : DR_Ability
{
    public BloodPickupRangeAbility(){
        triggeredByPlayer = false;
    }

    public void CollectBlood(DR_GameManager gm, Vector2Int pos){
        DR_Map map = gm.CurrentMap;

        for (int dx = -count; dx <= count; dx++){
            for (int dy = -count; dy <= count; dy++){
                Vector2Int offset = new(dx, dy);
                if (!map.ValidPosition(pos + offset) || offset.magnitude > count){
                    continue;
                }

                map.GetCell(pos + offset).CollectBlood(owner);
            }
        }
    }

    public override string GetFormattedDescription(){
        return string.Format($"Expands blood pick up range by {count} {(count > 1 ? "tiles" : "tile")}.");
    }
}

public class DeadlyHealAbility : DR_Ability
{
    public DeadlyHealAbility(){
    }

    public override bool CanBePerformed(){
        var healthComp = owner.GetComponent<HealthComponent>();
        if (healthComp.currentHealth >= healthComp.maxHealth){
            return false;
        }
        return base.CanBePerformed();
    }

    private float GetNewMaxHealthPercent(){
        return Mathf.Clamp01(0.9f + (Mathf.Log10(count) * 0.15f));
    }

    protected override void OnTrigger(DR_Event e){
        relatedEntities.Add(owner);

        var healthComp = owner.GetComponent<HealthComponent>();
        var levelComp = owner.GetComponent<LevelComponent>();

        levelComp.healthScale *= GetNewMaxHealthPercent();
        levelComp.UpdateStats();

        healthComp.HealFully();
    }

    public override string GetFormattedDescription(){
        return string.Format($"Fully heals but permanently reduces max health by {(1.0f - GetNewMaxHealthPercent()).ToString("P1")}");
    }
}