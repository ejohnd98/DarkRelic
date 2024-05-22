using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationSystem {

    //TODO: this does not support an entity queuing up multiple animations (game will freeze)

    static List<Tuple<DR_Animation, DR_Entity>> pendingAttackAnimations = new();
    static List<Tuple<DR_Animation, DR_Entity>> pendingMoveAnimations = new();
    static List<Tuple<DR_Animation, DR_Entity>> animationList = new();

    static bool isAnimating = false;

    public static void AddAnimation(DR_Animation anim, DR_Entity entity){
        if (anim is AttackAnimation){
            if (pendingMoveAnimations.Count > 0){
                PlayAllPendingAnimations();
                pendingAttackAnimations.Add(new(anim, entity));
            }else{
                pendingMoveAnimations.Add(new(anim, entity));
                PlayAllPendingAnimations();
            }
        }
        if (anim is MoveAnimation){
            pendingMoveAnimations.Add(new(anim, entity));
        }

        anim.StartAnim();
    }

    public static DR_Animation EntityAlreadyHasAnim(DR_Entity entity){
        foreach (var tuple in pendingAttackAnimations){
            if (tuple.Item2.Equals(entity)){
                return tuple.Item1;
            }
        }
        foreach (var tuple in pendingMoveAnimations){
            if (tuple.Item2.Equals(entity)){
                return tuple.Item1;
            }
        }
        return null;
    }

    public static void UpdateAnims(float time){
        if (!isAnimating){
            return;
        }

        float debugTimeMod = 1.0f;
        if (Input.GetKey(KeyCode.LeftControl)){
            debugTimeMod = 5.0f;
        }

        for (int i = 0; i < animationList.Count; i++){
            DR_Animation anim = animationList[i].Item1;
            anim.AnimStep(time * debugTimeMod);
            if (!anim.isAnimating){
                animationList.RemoveAt(i);
                anim.Entity.RemoveComponent(anim);
                i--;
            }
        }

        if (animationList.Count == 0){
            if (pendingAttackAnimations.Count > 0){
                animationList.AddRange(pendingAttackAnimations);
                pendingAttackAnimations.Clear();
            }else{
                isAnimating = false;
            }
        }
    }

    public static bool HasPendingAnimations(){
        return pendingAttackAnimations.Count > 0 || pendingMoveAnimations.Count > 0;
    }

    public static void PlayAllPendingAnimations(){
        animationList.AddRange(pendingMoveAnimations);
        animationList.AddRange(pendingAttackAnimations);
        pendingMoveAnimations.Clear();
        pendingAttackAnimations.Clear();
        isAnimating = true;
    }

    public static bool IsAnimating(){
        return isAnimating;
    }

}
