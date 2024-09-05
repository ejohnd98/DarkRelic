using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    public static DR_Entity CreateEntityFromContent(Content content)
    {
        DR_Entity newEntity = new DR_Entity();

        List<Type> componentTypes = new List<Type>();

        for(int i = 0; i < content.components.Count; i++)
        {
            Type type = content.components[i].GetType();
            componentTypes.Add(type);
        }

        for (int i = 0; i < componentTypes.Count; i++)
        {
            var type = componentTypes[i];
            // Check if the type is a subtype of DR_Component
            if (typeof(DR_Component).IsAssignableFrom(type) && type != typeof(DR_Component))
            {
                // Create an instance of the type
                DR_Component newComponent = (DR_Component)Activator.CreateInstance(type);
                DR_Component.CopyDataFromComponent(content.components[i], newComponent);
                newEntity.AddComponent(newComponent);
            }
        }

        newEntity.Name = content.contentName;
        newEntity.contentGuid = content.guid;

        foreach(var component in newEntity.ComponentList){
            component.OnEntityCreatedFromContent();
        }

        return newEntity;
    }
    
    public static DR_Entity CreateActor(Sprite Sprite, string Name, Alignment alignment = Alignment.ENEMY, int level = 1){
        DR_Entity NewActor = new DR_Entity();

        NewActor.Name = Name;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewActor.AddComponent<HealthComponent>(new HealthComponent(1)); //TODO: no longer should be providing maxhealth directly
        NewActor.AddComponent<InventoryComponent>(new InventoryComponent());
        NewActor.AddComponent<TurnComponent>(new TurnComponent());
        NewActor.AddComponent<AlignmentComponent>(new AlignmentComponent(alignment));
        NewActor.AddComponent<LevelComponent>(new LevelComponent(level));
        
        return NewActor;
    }

    public static DR_Entity CreateProp(Sprite Sprite, string Name){
        DR_Entity NewProp = new DR_Entity();

        NewProp.Name = Name;
        NewProp.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewProp.AddComponent<PropComponent>(new PropComponent());
        
        return NewProp;
    }

    public static DR_Entity CreateDoor(Sprite OpenSprite, Sprite ClosedSprite){
        DR_Entity NewProp = CreateProp(ClosedSprite, "Door");
        return NewProp;
    }

    public static DR_Entity CreateStairs(Sprite spr, bool goesDeeper){
        DR_Entity NewProp = CreateProp(spr, "Stairs " + (goesDeeper? "Down" : "Up"));

        NewProp.AddComponent<StairComponent>(new StairComponent(goesDeeper));
        NewProp.GetComponent<PropComponent>().blocksSight = false;
        
        return NewProp;
    }

    public static DR_Entity CreateGoal(Sprite spr){
        DR_Entity NewProp = CreateProp(spr, "Goal");

        NewProp.AddComponent<GoalComponent>(new GoalComponent());
        NewProp.GetComponent<PropComponent>().blocksSight = false;
        
        return NewProp;
    }

    public static DR_Entity CreateProjectileEntityAtPosition(Sprite Sprite, string Name, Vector2Int start, Vector2Int end, Color color){
        DR_Entity NewActor = new DR_Entity();

        NewActor.Name = Name;
        NewActor.Position = start;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        // MoveAnimation moveAnim = NewActor.AddComponent<MoveAnimation>(new());
        // moveAnim.AnimFinished += (DR_Animation moveAnim) => {
        //         //Debug.Log("AnimFinished!");
        //         DR_GameManager.instance.CurrentMap.RemoveEntity(NewActor);
        //         NewActor.noLongerValid = true;
        //         NewActor.DestroyEntity();
        //         NewActor.isOnMap = false;
        //         FXSpawner.instance.SpawnParticleFX(end, color);
        //     };

        DR_GameManager.instance.CurrentMap.AddEntity(NewActor);
        NewActor.isOnMap = true;

        //moveAnim.SetAnim(end, 0.2f, EaseType.Linear);
        //AnimationSystem.AddAnimation(moveAnim, NewActor);
        return NewActor;
    }
}
