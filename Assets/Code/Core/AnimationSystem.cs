using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationSystem {

    static List<DR_Animation> animationList = new List<DR_Animation>();

    public static void AddAnimation(DR_Animation anim){
        animationList.Add(anim);
        anim.StartAnim();
    }

    //TODO rework to allow queueing up of animations (like in C++ version of project)
    public static void UpdateAnims(float time){
        float debugTimeMod = 1.0f;
        if (Input.GetKey(KeyCode.LeftControl)){
            debugTimeMod = 5.0f;
        }

        for (int i = 0; i < animationList.Count; i++){
            DR_Animation anim = animationList[i];
            anim.AnimStep(time * debugTimeMod);
            if (!anim.isAnimating){
                animationList.RemoveAt(i);
                anim.Entity.RemoveComponent(anim);
                i--;
            }
        }
    }

    public static bool IsAnimating(){
        return animationList.Count > 0;
    }

}
