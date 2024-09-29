using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityTransformPair{
    public DR_Entity entity;
    public Transform transform;

    public EntityTransformPair(DR_Entity entity){
        this.entity = entity;
    }
}

public abstract class ActionAnimation{

    //TODO: replace entity/rendererObj with this field
    public EntityTransformPair owner;
    public Transform rendererObj; //Should this live here?
    public RenderedAction action;

    // This will be used so that inheriting anim classes can add any entities they need the transforms for to this list.
    // Then before animation starts SetupWithRenderer() will be called and fill out the transform properties of anything in this list
    // This shouldn't be accessed otherwise (child classes should define their own variables for the needed EntityTransformPairs)
    protected List<EntityTransformPair> entityTransformPairs = new();

    public float length = 1.0f;
    public bool isAnimating = false;
    public bool ignoreTimer = false;

    protected float counter = 0.0f;
    public float depth = 0.0f;

    public event Action<ActionAnimation> AnimStarted;
    public event Action<ActionAnimation> AnimFinished;

    public void StartAnim(){
        counter = 0.0f;
        isAnimating = true;
        AnimStart();
        AnimStarted?.Invoke(this);
    }

    public void StopAnim(){
        isAnimating = false;
        AnimEnd();
        AnimFinished?.Invoke(this);
    }

    public virtual void AnimStart(){

    }

    public virtual void AnimEnd(){
        
    }

    // This should be overridden to define animation behaviour
    public virtual void AnimStep(float time){
        if (!isAnimating || ignoreTimer){
            return;
        }

        counter += time / length;
        if (counter > 1.0f){
            counter = 1.0f;
            StopAnim();
        }
    }

    public void SetupWithRenderer(GameRenderer renderer, RenderedAction action){
        this.action = action; //TODO: probably won't need this after refactor? not sure yet
        entityTransformPairs.Add(owner);

        foreach (var pair in entityTransformPairs){
            renderer.EntityObjects.TryGetValue(pair.entity, out GameObject entityObj);
            if (entityObj != null){
                pair.transform = entityObj.transform;
            }else{
                Debug.LogAssertion("SetupWithRenderer could not find transform for entity: " + pair.entity.Name);
            }
        }
    }
}

public class MoveAnimation : ActionAnimation {
    public Vector3 start, end, current;
    public EaseType easing;

    public MoveAnimation(DR_Entity entity, Vector2 start, Vector2 end, float time = 0.05f, EaseType easeType = EaseType.Linear){
        this.owner = new(entity);
        this.start = start;
        this.end = end;
        this.length = time;
        this.easing = easeType;

        // TODO: This can surely be refactored away somewhere
        depth = GameRenderer.GetDepthForEntity(entity);
        this.start.z = depth;
        this.end.z = depth;

        current = start;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("move2");
        };
    }

    // TODO: remove when possible
    public MoveAnimation(RenderedAction action, Transform rendererObj, Vector2 start, Vector2 end, float time = 0.05f, EaseType easeType = EaseType.Linear){
        this.rendererObj = rendererObj;
        this.action = action;
        this.owner = new(action.originalAction.owner);
        this.start = start;
        this.end = end;

        depth = GameRenderer.GetDepthForEntity(owner.entity);
        this.start.z = depth;
        this.end.z = depth;

        easing = easeType;
        length = time;
        current = start;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("move2");
        };
    }

    public override void AnimStep(float time)
    {
        base.AnimStep(time);
        current = Easings.GetEasedValue(start, end, counter, easing);
        if (owner.transform != null)
            owner.transform.position = current;
    }
}

public class AttackAnimation : ActionAnimation {
    public Vector3 start, target, current;
    public event Action<ActionAnimation> AnimHalfway;

    public EntityTransformPair attackTarget;

    bool halfwayReached = false;

    // TODO: get rid of vector parameters once this constructor is called as the action is being executed (right now it's later so pos of entity might be out of sync with visuals)
    public AttackAnimation(DR_Entity attacker, DR_Entity target, Vector2 startPos, Vector2 targetPos, float time = 0.25f){
        owner = new(attacker);
        attackTarget = new(target);
        entityTransformPairs.Add(attackTarget);

        this.start = startPos;
        this.target.x = start.x *0.35f + targetPos.x *0.65f;
        this.target.y = start.y *0.35f + targetPos.y *0.65f;

        depth = GameRenderer.GetDepthForEntity(owner.entity) - 0.01f; // raise slightly so attacker is above target
        this.start.z = depth;
        this.target.z = depth;

        length = time;
        current = start;
    }

    public AttackAnimation(RenderedAction action, Transform rendererObj, Transform targetRendererObj ,Vector2 start, Vector2 target, float time = 0.25f){
        this.rendererObj = rendererObj;
        this.action = action;
        this.owner = new(action.originalAction.owner);
        this.start = start;
        
        this.target.x = start.x *0.35f + target.x *0.65f;
        this.target.y = start.y *0.35f + target.y *0.65f;

        depth = GameRenderer.GetDepthForEntity(owner.entity) - 0.01f; // raise slightly so attacker is above target
        this.start.z = depth;
        this.target.z = depth;

        length = time;
        current = start;

        if (action.originalAction is AttackAction attack){
            attackTarget = new(attack.target);
            entityTransformPairs.Add(attackTarget);
        }
    }

    public override void AnimStart()
    {
        if (owner.entity.HasComponent<PlayerComponent>()){
            GameRenderer.instance.lockCameraPos = true;
        }
    }

    public override void AnimEnd()
    {
        if (owner.entity.HasComponent<PlayerComponent>()){
            GameRenderer.instance.lockCameraPos = false;
        }
    }

    public override void AnimStep(float time){
        base.AnimStep(time);

        if (counter < 0.5f){
            current = Easings.EaseInBack(start, target, Mathf.Clamp01(counter*2.0f));
        }else{
            if (!halfwayReached){
                halfwayReached = true;
                AnimHalfway?.Invoke(this);

                float cameraShakeAmount = 0.5f;

                if (action.originalAction is AttackAction attack){
                    if (attack.killed){
                        //TODO: more proper way of setting blood (this pulls the latest data which may not be accurate to the current visuals)
                        //TODO again. This is very bad. Should maybe have actions contain a list of world changes as well like blood?
                        Vector2Int bloodPos = attackTarget.entity.Position;
                        GameRenderer.instance.SetBlood(bloodPos);

                        FXSpawner.instance.SpawnDeathFX(attack.target, attackTarget.transform.position);
                        GameRenderer.instance.RemoveEntityObj(attack.target);

                        cameraShakeAmount *= 2.0f;
                    }

                    if (attack.target.HasComponent<PlayerComponent>()){
                        cameraShakeAmount *= 1.5f;
                    }

                    SoundSystem.instance.PlaySound(attack.killed ? "death" : "attack");

                    CameraShake.ShakeCamera(cameraShakeAmount);
                }
            }
            current = Easings.QuadEaseOut(target,start, Mathf.Clamp01((counter-0.5f)*2.0f));
        }

        owner.transform.position = current;
    }
}

public class AbilityAnimation : ActionAnimation {

    public AbilityAnimation(DR_Entity entity, float time = 0.25f){
        owner = new(entity);
        length = time;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("abilityPlaceholder");
        };
    }

    public AbilityAnimation(RenderedAction action, Transform rendererObj, float time = 0.25f){
        this.rendererObj = rendererObj;
        this.action = action;
        this.owner = new(action.originalAction.owner);
        this.length = time;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("abilityPlaceholder");
        };
    }

    public override void AnimStart()
    {
        FXSpawner.instance.SpawnParticleFX(VectorUtility.V2toV2I(owner.transform.position), new Color(0.9f, 0.9f, 1.0f));
    }
}

public class ProjectileAnimation : ActionAnimation {

    public Transform targetRendererObj;
    public EntityTransformPair attackTarget;

    Transform projectileObj;
    Vector3 a, b;
    Vector2Int targetPos;
    Sprite spr;

    Vector2 start;
    Vector2 end;

    //TODO: could also just give positions to decouple from entities?
    public ProjectileAnimation(DR_Entity attacker, DR_Entity target, Vector2 start, Vector2 end, float time = 0.2f, Sprite spr = null){
        owner = new(attacker);
        attackTarget = new(target);
        entityTransformPairs.Add(attackTarget);

        this.start = start;
        this.end = end;

        length = time;
        this.spr = spr;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("abilityPlaceholder");
        };
    }

    public ProjectileAnimation(RenderedAction action, Transform rendererObj, Transform targetRenderObj, float time = 0.2f, Sprite spr = null){
        this.rendererObj = rendererObj;
        this.action = action;
        this.owner = new(action.originalAction.owner);
        this.length = time;
        this.targetRendererObj = targetRenderObj;
        this.spr = spr;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("abilityPlaceholder");
        };
    }

    public override void AnimStart()
    {
        //TODO: when setting up with renderer in base class should create any renderer objs that do not yet exist?
        targetPos = action.originalAction.actionInputs[0].inputValue;
        a = start;
        b = new Vector3(end.x, end.y, -1);

        projectileObj = FXSpawner.instance.SpawnSprite(new Vector2Int(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y)), spr, new Color(0.722f, 0.145f, 0.247f));

        //TODO: if source is not visible, lerp alpha of sprite from zero?
    }

    public override void AnimStep(float time){
        base.AnimStep(time);

        Vector3 pos = Easings.GetEasedValue(a,b,counter, EaseType.EaseInQuad);
        pos.z = projectileObj.position.z;
        projectileObj.position = pos;
    }

    public override void AnimEnd()
    {
        GameObject.Destroy(projectileObj.gameObject);
        
        FXSpawner.instance.SpawnParticleFX(targetPos, new Color(0.722f, 0.145f, 0.247f));
                //TODO: this is a lot of duplicate code from the attack animation. Should have common ground of some sort

        float cameraShakeAmount = 0.5f;

        if (action.originalAction is AbilityAction abilityAction && abilityAction.ability is BloodBoltAbility bloodBoltAbility){
            //TODO: replace with attackTarget.entity
            DR_Entity targetEntity = bloodBoltAbility.target;

            if (bloodBoltAbility.killed){
                //TODO: more proper way of setting blood (this pulls the latest data which may not be accurate to the current visuals)
                Vector2Int bloodPos = new(Mathf.RoundToInt(attackTarget.transform.position.x), Mathf.RoundToInt(attackTarget.transform.position.y));
                GameRenderer.instance.SetBlood(bloodPos);

                FXSpawner.instance.SpawnDeathFX(targetEntity, attackTarget.transform.position);
                GameRenderer.instance.RemoveEntityObj(targetEntity);

                cameraShakeAmount *= 2.0f;
            }

            if (targetEntity.HasComponent<PlayerComponent>()){
                cameraShakeAmount *= 1.5f;
            }

            SoundSystem.instance.PlaySound(bloodBoltAbility.killed ? "death" : "attack");

            CameraShake.ShakeCamera(cameraShakeAmount);
        }
    }
}

public class StairAnimation : ActionAnimation {

    public StairAnimation(DR_Entity entity){
        this.owner = new(entity);
        ignoreTimer = true;
    }

    public StairAnimation(RenderedAction action){
        this.action = action;
        this.owner = new(action.originalAction.owner);
        ignoreTimer = true;
    }

    public override void AnimStart()
    {
        StairAction stairAction = action.originalAction as StairAction;
        SoundSystem.instance.PlaySound(stairAction.stairs.goesDeeper ? "descend" : "ascend");

        ImageFadeToggle blackOverlay = DR_GameManager.instance.blackOverlay;

        Action OnFadeOut = null;
        OnFadeOut = () => {
            blackOverlay.OnVisibleComplete -= OnFadeOut;
            StopAnim();
            //GameRenderer.instance.FullyUpdateRenderer();
            blackOverlay.SetShouldBeVisible(false);
            GameRenderer.instance.lockCameraPos = false;
            
            //TODO: figure out way to force camera pos here
        };

        Action OnFadeIn = null;
        OnFadeIn = () => {
            blackOverlay.OnVisibleComplete -= OnFadeIn;
            //StopAnim();
        };
        
        blackOverlay.OnVisibleComplete += OnFadeOut;
        blackOverlay.OnInvisibleComplete += OnFadeIn;

        blackOverlay.SetShouldBeVisible(true);
        GameRenderer.instance.lockCameraPos = true;
    }
}