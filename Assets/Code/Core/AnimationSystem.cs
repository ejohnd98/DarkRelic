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

    // Possible way forward:
    // Create an UpdateRendererBase method or similar that will completely update the current scene (otherwise it is not updated)
    // Then when actions are performed they are added to a queue on the renderer.
    // At the start of each players turn the UpdateRendererBase method is called
    // If action queue is not empty, step through them and perform any needed animations (to start, can skip this to get basic update logic working)
    // 
    //Things to figure out:
    // Q: How to determine animation from action?
    //    A: Have either a switch statement which maps action to anim type, or have some anim type enum/class type specified in the action.
    //       The animation will be similar to existing class but not a component. will contain reference to action perhaps or at least thw owning/affected entities.
    // Q: How to handle enemies which are killed/removed during an action
    //    A: rework how entites are added/removed and have the base action class contain lists of added/removed entities (alongside position?)
    // Q: How to handle a door open action
    //    A: No way to close doors at the moment, bu have action specify what the outcome was. Then can get sprite from DoorComponent?
    // Q: How to handle showing stats while the map is out of date?
    //    A: Don't. turn off details panel while animations are playing (or grey out and don't update)
    // Q: How to handle logs
    //    A: Simple. Just wait to log the message until the action is processed by the renderer
    //
    // Random thought: move turn debt to be based off speed stat, and scale certain animations with this speed (using default player speed as a base 1.0 scalar?) 



    //TODO: each time an animation is added to the queue it checks whether existing animations need to be played first.
	//this is true whenever an attack animation is added
	//OR if a previous move animation is using the same entity

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
