using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimComponent : DR_Component
{
    public bool isMoving = false;

    Vector3 a, b;
    Vector3 currentPos;
    float counter = 0.0f;
    public float length = 0.1f;

    public void SetAnim(Vector2Int target, float time = 0.1f, bool autoStart = true){
        //temp z
        a = Entity.GetPosFloat(Entity.HasComponent<PropComponent>() ? DR_Renderer.PropDepth : DR_Renderer.ActorDepth);
        b = a;
        b.x = target.x;
        b.y = target.y;

        length = time;

        currentPos = a;
        counter = 0.0f;

        if (autoStart){
            StartAnim();
        }
    }

    public void StartAnim(){
        if (isMoving){
            return;
        }

        isMoving = true;
        DR_Renderer.animsActive++;
    }

    public void StopAnim(){
        if (!isMoving){
            return;
        }
        currentPos = b;
        isMoving = false;
        DR_Renderer.animsActive--;
    }

    public void AnimStep(float time){
        if (!isMoving){
            return;
        }

        counter += time / length;
        if (counter > 1.0f){
            StopAnim();
        }
        currentPos = Easings.QuadEaseOut(a,b, Mathf.Clamp01(counter));
    }

    public Vector3 GetAnimPosition(float depth = 0.0f){
        return isMoving? currentPos : Entity.GetPosFloat(depth);
    }
}
