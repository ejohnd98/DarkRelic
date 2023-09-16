using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: this is essentially a copy of MoveAnimComponent. Create base anim class?
// also maybe make these components only on entities while the anim is active?
// also also, create class for projectile animations with the same base
public class AttackAnimComponent : DR_Component
{
    public bool isAnimating = false;

    Vector3 a, b;
    Vector3 currentPos;
    float counter = 0.0f;
    public float length = 0.1f;

    public void SetAnim(Vector2Int target, float time = 0.3f, bool autoStart = true){
        //temp z
        a = Entity.GetPosFloat(DR_Renderer.GetDepthForEntity(Entity));

        //raise slightly so attacker is above target
        a.z -= 0.01f;

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
        currentPos = a;
        isAnimating = false;
        DR_Renderer.animsActive--;

        // testing removing component after use
        Entity.RemoveComponent(this);
    }

    public void AnimStep(float time){
        if (!isAnimating){
            return;
        }

        counter += time / length;
        if (counter > 1.0f){
            StopAnim();
        }

        if (counter < 0.5f){
            currentPos = Easings.EaseInBack(a, b, Mathf.Clamp01(counter*2.0f));
        }else{
            currentPos = Easings.QuadEaseOut(b,a, Mathf.Clamp01((counter-0.5f)*2.0f));
        }
        
    }

    public Vector3 GetAnimPosition(float depth = 0.0f){
        return isAnimating? currentPos : Entity.GetPosFloat(depth);
    }

    public override void OnComponentRemoved()
    {
        base.OnComponentRemoved();
        StopAnim();
    }
}
