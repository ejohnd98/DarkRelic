using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DR_Animation : DR_Component{

    public float length = 1.0f;
    public bool isAnimating = false;

    protected float counter = 0.0f;

    public event Action<DR_Animation> AnimFinished;

    public void StartAnim(){
        counter = 0.0f;
        isAnimating = true;
    }

    public void StopAnim(){
        isAnimating = false;
        AnimFinished?.Invoke(this);
    }

    // This should be overridden to define animation behaviour
    public virtual void AnimStep(float time){
        if (!isAnimating){
            return;
        }

        counter += time / length;
        if (counter > 1.0f){
            counter = 1.0f;
            StopAnim();
        }
    }

    public virtual Vector3 GetAnimPosition(float depth = 0.0f){
        return Entity.GetPosFloat(depth);
    }
}

public class MoveAnimation : DR_Animation {
    public Vector3 start, end, current;
    public EaseType easing;

    public override void AnimStep(float time)
    {
        base.AnimStep(time);
        current = Easings.GetEasedValue(start, end, counter, easing);
    }

    public override Vector3 GetAnimPosition(float depth = 0.0f){
        return isAnimating? current : Entity.GetPosFloat(depth);
    }

    public void SetAnim(Vector2Int target, float time = 0.1f, EaseType easeType = EaseType.QuadEaseOut){
        start = Entity.GetPosFloat(DR_Renderer.GetDepthForEntity(Entity));
        end = start;
        end.x = target.x;
        end.y = target.y;

        easing = easeType;
        length = time;
        current = start;
    }
}

public class AttackAnimation : DR_Animation {
    public Vector3 start, end, current;
    public event Action<DR_Animation> AnimHalfway;

    bool halfwayReached = false;

    public override void AnimStep(float time)
    {
        base.AnimStep(time);

        if (counter < 0.5f){
            current = Easings.EaseInBack(start, end, Mathf.Clamp01(counter*2.0f));
        }else{
            if (!halfwayReached){
                halfwayReached = true;
                AnimHalfway?.Invoke(this);
            }
            current = Easings.QuadEaseOut(end,start, Mathf.Clamp01((counter-0.5f)*2.0f));
        }
    }

    public override Vector3 GetAnimPosition(float depth = 0.0f){
        return isAnimating? current : Entity.GetPosFloat(depth);
    }

    public void SetAnim(Vector2Int target, float time = 0.3f){
        start = Entity.GetPosFloat(DR_Renderer.GetDepthForEntity(Entity));
        
        //raise slightly so attacker is above target
        start.z -= 0.01f;

        end = start;
        end.x = start.x *0.35f + target.x *0.65f;
        end.y = start.y *0.35f + target.y *0.65f;

        length = time;
        current = start;
    }
}
