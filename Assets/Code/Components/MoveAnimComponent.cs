using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo: define this in Easings?
public enum EaseType{
    QuadEaseOut,
    Linear
}

public class MoveAnimComponent : DR_Component
{
    public bool isAnimating = false;

    public event Action<MoveAnimComponent> AnimFinished;

    Vector3 a, b;
    Vector3 currentPos;
    float counter = 0.0f;
    public float length = 0.1f;
    public EaseType easing;

    public void SetAnim(Vector2Int target, float time = 0.15f, bool autoStart = true, EaseType easeType = EaseType.QuadEaseOut){
        //temp z
        a = Entity.GetPosFloat(DR_Renderer.GetDepthForEntity(Entity));
        b = a;
        b.x = target.x;
        b.y = target.y;
        easing = easeType;

        length = time;

        currentPos = a;
        counter = 0.0f;

        if (autoStart){
            StartAnim();
        }
    }

    public void StartAnim(){
        if (isAnimating){
            return;
        }

        isAnimating = true;
        DR_Renderer.animsActive++;
    }

    public void StopAnim(){
        if (!isAnimating){
            return;
        }
        currentPos = b;
        isAnimating = false;
        DR_Renderer.animsActive--;
        AnimFinished?.Invoke(this);
    }

    public void AnimStep(float time){
        if (!isAnimating){
            return;
        }

        counter += time / length;
        if (counter > 1.0f){
            StopAnim();
        }
        switch(easing){
            case EaseType.QuadEaseOut:
                currentPos = Easings.QuadEaseOut(a,b, Mathf.Clamp01(counter));
                break;
            case EaseType.Linear:
                currentPos = Easings.Linear(a,b, Mathf.Clamp01(counter));
                break;
        }
        
    }

    public Vector3 GetAnimPosition(float depth = 0.0f){
        return isAnimating? currentPos : Entity.GetPosFloat(depth);
    }
}
