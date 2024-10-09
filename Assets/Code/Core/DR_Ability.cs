using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Ability : DR_EffectBase
{
    //TODO: move ability specific stuff into this class from base
}

// TODO: generalize to projectile (or further to targeted?) ability
public class BloodBoltAbility : DR_Ability
{
    public bool killed = false;
    public DR_Entity target;

    [Copy]
    public float range = 8;

    [Copy]
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
        DamageSystem.CreateAttackTransaction(owner, new(){target}, GetStrengthModifier());

        // TODO: have entities handle their own hurt/killed sounds
        killed = false;//damageEvent.killed;
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
                    //TODO: replace this
                    DamageSystem.CreateAttackTransaction(owner, new(){cell.Actor}, 0.5f);
                    cell.AddBlood(1); //Temp just to show SOMETHING 
                    return;
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

public class DamageMirror : DR_Ability
{
    public DamageMirror(){
        triggeredByPlayer = false;
        
    }

    public override void OnAdded(){
        owner.OnAttacked += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e)
    {
        var attackedEvent = e as AttackEvent;
        if (attackedEvent.owner == null){
            return;
        }
        int reflectedDamage = Mathf.FloorToInt(attackedEvent.damageDealt * GetReflectAmount());
        Debug.Log($"reflected {reflectedDamage} back to {attackedEvent.owner.Name}");
        attackedEvent.owner.GetComponent<HealthComponent>().TakeDamage(reflectedDamage);
    }

    private float GetReflectAmount(){
        return MathF.Pow(1.1f, count) - 1.0f;
    }

    private float GetStrengthModifier(){
        return MathF.Pow(0.9f, count);
    }

    public override void ApplyStatModifiers(StatsModifier statsModifier)
    {
        statsModifier.strength.multiplier *= GetStrengthModifier();
    }

    public override string GetFormattedDescription(){
        return string.Format($"Reflects {GetReflectAmount().ToString("P1")} of damage back on the attacker, but reduces own strength by {(1.0f - GetStrengthModifier()).ToString("P1")}.");
    }
}

public class ChainLightningAbility : DR_Ability
{
    // Possibilities:
    // Random chance for each attack to also attack an adjacent/nearby enemy (that attack also has a chance, but can't target an enemy already hit by ability?)
    // OR hit all adjacent enemies from attack but deal % of the damage (might be overpowered as it will still trigger other abilities on a lot of enemies)
    // 

    public ChainLightningAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.OnAttackTransactionCreated += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e){

        var attackEvent = e as AttackTransactionEvent;
        var attackTransaction = attackEvent.attackTransaction;

        int range = 2;
        var gm = DR_GameManager.instance;

        // Just to test this works, may need adjusting
        for (int i = 0; i < count; i++){

            List<DR_Entity> possibleTargets = new();
        
            foreach(var existingTarget in attackTransaction.targets){
                for(int dy = -range; dy <= range; dy++){
                    for(int dx = -range; dx <= range; dx++){
                        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 2){
                            continue;
                        }

                        Vector2Int pos = existingTarget.Position + new Vector2Int(dx,dy);
                        var possibleTarget = gm.CurrentMap.GetActorAtPosition(pos);
                        if (possibleTarget != null && possibleTarget != owner
                            && !attackTransaction.targets.Contains(possibleTarget) 
                            && !possibleTargets.Contains(possibleTarget)){
                            
                            possibleTargets.Add(possibleTarget);
                        }
                    }
                }
            }

            if (possibleTargets.Count > 0){
                int randIndex = UnityEngine.Random.Range(0, possibleTargets.Count);
                Debug.Log($"Adding {possibleTargets[randIndex].Name} to attack targets");
                attackTransaction.targets.Add(possibleTargets[randIndex]);

                //TEMP TEST:
                gm.turnSystem.currentAction.animations.Add(new AbilityAnimation(possibleTargets[randIndex]));
            }else{
                Debug.Log("Could not find extra target");
            }
        }
    }

    private float GetDamagePercent(){
        return 0.1f + (0.25f * Mathf.Log(count, 2));
    }

    public override string GetFormattedDescription(){
        float percent = GetDamagePercent();
        return string.Format("TBD {0:0%}", percent);
    }
}

public class SpiderWebAbility : DR_Ability
{
    public bool killed = false;
    public List<DR_Entity> targets;

    [Copy]
    public float range = 8;

    [Copy]
    public Sprite projectileSprite;

    public SpiderWebAbility(){
        baseBloodCost = 1;
        cooldownLength = 2;
    }

    //TODO: allow targeting ground
    protected override void SetupInputs(){
        actionInputs.Add(new ActionInput((Vector2Int pos) => {
            return DR_GameManager.instance.CurrentMap.GetIsVisible(pos)
            && (pos - owner.Position).magnitude <= range;
        }));
    }

    private int GetWebRadius(){
        return 1 + count;
    }

    protected override void OnTrigger(DR_Event e){
        Vector2Int centerPos = actionInputs[0].inputValue;
        Debug.Log("SpiderWebAbility Triggered with input: " + centerPos);

        DR_Entity owner = e.owner;
        var gm  =  DR_GameManager.instance;

        // Not actually ignoring the player
        targets = gm.CurrentMap.GetEntitiesInRadius(centerPos, GetWebRadius());

        relatedEntities.Add(owner);
        relatedEntities.AddRange(targets);

        // draw all targets towards center:

        bool movedTarget;
        int tempLimit = 500;

        Dictionary<DR_Entity, MoveAnimation> moveAnimDict = new();
        foreach(var target in targets){
            var moveAnim = new MoveAnimation(target, target.Position, target.Position, 0.1f);
            moveAnimDict[target] = moveAnim;
        }

        do{
            movedTarget = false;
            foreach(var target in targets){
                Vector2Int diff = centerPos - target.Position;
                if (diff == Vector2Int.zero){
                    continue;
                }

                Vector2Int dirX = new(Math.Sign(diff.x), 0);
                Vector2Int dirY = new(0, Math.Sign(diff.y));

                bool canMoveX = dirX.magnitude != 0 && gm.CurrentMap.CanMoveActor(target, target.Position + dirX);
                bool canMoveY = dirY.magnitude != 0 && gm.CurrentMap.CanMoveActor(target, target.Position + dirY);

                if (canMoveX && (!canMoveY || (canMoveY && (dirX.magnitude > dirY.magnitude)))){
                    //Move X
                    gm.CurrentMap.MoveActor(target, target.Position + dirX);
                    
                    movedTarget = true;
                }else if (canMoveY){
                    //Move Y
                    gm.CurrentMap.MoveActor(target, target.Position + dirY);
                    movedTarget = true;
                }else{
                    //Could not move
                }
            }
            tempLimit--;

        }while(movedTarget && tempLimit > 0);
        if (tempLimit < 1){
            Debug.LogAssertion("tempLimit is " + tempLimit);
        }

        foreach(var target in targets){
            moveAnimDict[target].end = VectorUtility.V2ItoV2(target.Position);
            gm.turnSystem.currentAction.animations.Add(moveAnimDict[target]);
        }

        //TODO: debuffs
        foreach(var target in targets){
            target.GetComponent<HealthComponent>().AddStatusEffect(new BleedStatusEffect());

            //Extremely temp
            target.GetComponent<TurnComponent>().CurrentDebt -= 20;
        }

        //TODO: TEMP Anim TEST:
        foreach(var target in targets){
            //gm.turnSystem.currentAction.animations.Add(new AbilityAnimation(target));
        }
    }

    public override string GetFormattedDescription(){
        //TODO: allow mousing over statuses referenced here so that they can be read about on the item itself
        // OR: in description, add a legend kind of like FEH does below regular description (probably makes more sense to do this)
        return string.Format($"Pulls together all entities within a {GetWebRadius()} radius of the targeted space and inflicts WEB");
    }
}

public class BloodlustAbility : DR_Ability
{
    public BloodlustAbility(){
        triggeredByPlayer = false;
    }

    public override void OnAdded()
    {
        owner.GetComponent<LevelComponent>().UpdateStats();
        owner.OnMove += OnTrigger;
    }

    protected override void OnTrigger(DR_Event e){
        owner.GetComponent<LevelComponent>().UpdateStats();
    }

    private float GetStatModifier(bool onBloodyTile){
        return Mathf.Max(onBloodyTile ? (1.0f + (count * 0.1f)) : (1.0f - (count * 0.1f)), 0.0f);
    }

    public override void ApplyStatModifiers(StatsModifier statsModifier)
    {
        bool onBloodyTile = DR_GameManager.instance.CurrentMap.GetCell(owner.Position).bloodStained;
        statsModifier.strength.multiplier *= GetStatModifier(onBloodyTile);
    }

    public override string GetFormattedDescription(){
        return string.Format("Increases strength by {0:0%} while on bloodstained tiles. Reduces by {1:0%} otherwise.", (1.0f - GetStatModifier(true)), (1.0f - GetStatModifier(false)));
    }
}