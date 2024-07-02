using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActionAnimation{

    public DR_Entity entity;
    public Transform rendererObj; //Should this live here?
    public RenderedAction action;

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

    public virtual Vector3 GetAnimPosition(float depth = 0.0f){
        return entity.GetPosFloat(depth);
    }
}

public class MoveAnimation : ActionAnimation {
    public Vector3 start, end, current;
    public EaseType easing;

    public MoveAnimation(RenderedAction action, Transform rendererObj, Vector2 start, Vector2 end, float time = 0.05f, EaseType easeType = EaseType.Linear){
        this.rendererObj = rendererObj;
        this.action = action;
        this.entity = action.originalAction.owner;
        this.start = start;
        this.end = end;

        depth = GameRenderer.GetDepthForEntity(entity);
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
        if (rendererObj != null)
            rendererObj.position = current;
    }

    public override Vector3 GetAnimPosition(float depth = 0.0f){
        return current;
    }
}

public class AttackAnimation : ActionAnimation {
    public Vector3 start, target, current;
    public event Action<ActionAnimation> AnimHalfway;

    Transform targetRendererObj;

    bool halfwayReached = false;

    public AttackAnimation(RenderedAction action, Transform rendererObj, Transform targetRendererObj ,Vector2 start, Vector2 target, float time = 0.25f){
        this.rendererObj = rendererObj;
        this.targetRendererObj = targetRendererObj;
        this.action = action;
        this.entity = action.originalAction.owner;
        this.start = start;
        
        this.target.x = start.x *0.35f + target.x *0.65f;
        this.target.y = start.y *0.35f + target.y *0.65f;

        depth = GameRenderer.GetDepthForEntity(entity) - 0.01f; // raise slightly so attacker is above target
        this.start.z = depth;
        this.target.z = depth;

        length = time;
        current = start;
    }

    public override void AnimStart()
    {
        if (entity.HasComponent<PlayerComponent>()){
            GameRenderer.instance.lockCameraPos = true;
        }
    }

    public override void AnimEnd()
    {
        if (entity.HasComponent<PlayerComponent>()){
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
                        Vector2Int bloodPos = new(Mathf.RoundToInt(targetRendererObj.position.x), Mathf.RoundToInt(targetRendererObj.position.y));
                        GameRenderer.instance.SetBlood(bloodPos);

                        FXSpawner.instance.SpawnDeathFX(attack.target, targetRendererObj.position);
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

        rendererObj.position = current;
    }

    public override Vector3 GetAnimPosition(float depth = 0.0f){
        return current;
    }
}

public class AbilityAnimation : ActionAnimation {

    public AbilityAnimation(RenderedAction action, Transform rendererObj, float time = 0.25f){
        this.rendererObj = rendererObj;
        this.action = action;
        this.entity = action.originalAction.owner;
        this.length = time;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("abilityPlaceholder");
        };
    }

    public override void AnimStart()
    {
        FXSpawner.instance.SpawnParticleFX(entity.Position, new Color(0.9f, 0.9f, 1.0f));
    }
}

public class ProjectileAnimation : ActionAnimation {

    public Transform targetRendererObj;

    //TODO: What if there's multiple projectiles?
    Transform projectileObj;
    Vector3 a, b;
    Vector2Int targetPos;

    public ProjectileAnimation(RenderedAction action, Transform rendererObj, Transform targetRenderObj, float time = 0.2f){
        this.rendererObj = rendererObj;
        this.action = action;
        this.entity = action.originalAction.owner;
        this.length = time;
        this.targetRendererObj = targetRenderObj;

        AnimStarted += (anim)=>{
            SoundSystem.instance.PlaySound("abilityPlaceholder");
        };
    }

    public override void AnimStart()
    {
        //Super messy. Should be getting the renderer obj corresponding to the target entity
        targetPos = action.originalAction.actionInputs[0].inputValue;
        a = GameRenderer.instance.EntityObjects[action.originalAction.owner].transform.position;
        b = new Vector3(targetPos.x, targetPos.y, -1);

        projectileObj = FXSpawner.instance.SpawnSprite(new Vector2Int(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y)), null, new Color(0.722f, 0.145f, 0.247f));
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
            DR_Entity targetEntity = bloodBoltAbility.target;

            if (bloodBoltAbility.killed){
                //TODO: more proper way of setting blood (this pulls the latest data which may not be accurate to the current visuals)
                Vector2Int bloodPos = new(Mathf.RoundToInt(targetRendererObj.position.x), Mathf.RoundToInt(targetRendererObj.position.y));
                GameRenderer.instance.SetBlood(bloodPos);

                FXSpawner.instance.SpawnDeathFX(targetEntity, targetRendererObj.position);
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

    public StairAnimation(RenderedAction action){
        this.action = action;
        this.entity = action.originalAction.owner;
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