using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimComponent : DR_Component
{
    public bool isMoving = false;

    Vector3 a, b;
    Vector3 currentPos;
    float counter = 0.0f;
    public float length = 0.15f;

    public void SetAnim(Vector2Int target, float time = 0.15f, bool autoStart = true){
        //temp z
        a = Entity.GetPosFloat(-1.0f);
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

        isMoving = false;
        DR_Renderer.animsActive--;
    }

    public void AnimStep(float time){
        if (!isMoving){
            return;
        }

        counter += time / length;
        if (counter >= 1.0f){
            StopAnim();
        }
        currentPos = Easings.QuadEaseOut(a,b, counter);
    }

    public Vector3 GetAnimPosition(){
        return isMoving? currentPos : Entity.GetPosFloat();
    }
}
