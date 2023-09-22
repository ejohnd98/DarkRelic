using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    public static DR_Entity CreateActor(Sprite Sprite, string Name, Alignment alignment = Alignment.ENEMY, int level = 1){
        DR_Entity NewActor = new DR_Entity();

        NewActor.Name = Name;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewActor.AddComponent<HealthComponent>(new HealthComponent(1)); //TODO: no longer should be providing maxhealth directly
        NewActor.AddComponent<InventoryComponent>(new InventoryComponent(25));
        NewActor.AddComponent<TurnComponent>(new TurnComponent());
        NewActor.AddComponent<MoveAnimComponent>(new MoveAnimComponent());
        NewActor.AddComponent<AlignmentComponent>(new AlignmentComponent(alignment));
        NewActor.AddComponent<LevelComponent>(new LevelComponent(level, NewActor));
        
        return NewActor;
    }

    public static DR_Entity CreateProp(Sprite Sprite, string Name){
        DR_Entity NewProp = new DR_Entity();

        NewProp.Name = Name;
        NewProp.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewProp.AddComponent<PropComponent>(new PropComponent());
        
        return NewProp;
    }

    public static DR_Entity CreateEquipmentItem(Sprite Sprite, string Name){
        DR_Entity NewItem = new DR_Entity();

        NewItem.Name = Name;
        NewItem.AddComponent<ItemComponent>(new ItemComponent());
        NewItem.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewItem.AddComponent<EquippableComponent>(new EquippableComponent());
        
        return NewItem;
    }

    public static DR_Entity CreateHealingItem(Sprite Sprite, string Name, int healAmount){
        DR_Entity NewItem = new DR_Entity();

        NewItem.Name = Name;
        NewItem.AddComponent<ItemComponent>(new ItemComponent());
        NewItem.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewItem.AddComponent<HealingConsumableComponent>(new HealingConsumableComponent(healAmount));
        
        return NewItem;
    }

    public static DR_Entity CreateMagicItem(Sprite Sprite, string Name, int damageAmount){
        DR_Entity NewItem = new DR_Entity();

        NewItem.Name = Name;
        NewItem.AddComponent<ItemComponent>(new ItemComponent());
        NewItem.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewItem.AddComponent<MagicConsumableComponent>(new MagicConsumableComponent(damageAmount));
        NewItem.GetComponent<MagicConsumableComponent>().targetClosest = true;
        NewItem.GetComponent<MagicConsumableComponent>().color = new Color(1.0f, 1.0f, 0.529f);
        NewItem.GetComponent<MagicConsumableComponent>().projectileSprite = DR_GameManager.instance.SparkProjectile;
        
        return NewItem;
    }

    public static DR_Entity CreateTargetedMagicItem(Sprite Sprite, string Name, int damageAmount){
        DR_Entity NewItem = new DR_Entity();

        NewItem.Name = Name;
        NewItem.AddComponent<ItemComponent>(new ItemComponent());
        NewItem.GetComponent<ItemComponent>().requireFurtherInputOnUse = true;
        NewItem.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewItem.AddComponent<MagicConsumableComponent>(new MagicConsumableComponent(damageAmount));
        NewItem.GetComponent<MagicConsumableComponent>().targetClosest = false;
        NewItem.GetComponent<MagicConsumableComponent>().color = new Color(0.99f, 0.24f, 0.19f);
        NewItem.GetComponent<MagicConsumableComponent>().projectileSprite = DR_GameManager.instance.FireProjectile;
        
        return NewItem;
    }

    public static DR_Entity CreateDoor(Sprite OpenSprite, Sprite ClosedSprite){
        DR_Entity NewProp = CreateProp(ClosedSprite, "Door");

        NewProp.AddComponent<DoorComponent>(new DoorComponent(OpenSprite, ClosedSprite));
        NewProp.GetComponent<DoorComponent>().SetOpen(false);
        
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
        MoveAnimComponent moveAnim = NewActor.AddComponent<MoveAnimComponent>(new MoveAnimComponent());
        moveAnim.AnimFinished += (MoveAnimComponent moveAnim) => {
                Debug.Log("AnimFinished!");
                DR_GameManager.instance.CurrentMap.RemoveEntity(NewActor);
                NewActor.noLongerValid = true;
                NewActor.DestroyEntity();
                NewActor.isOnMap = false;
                FXSpawner.instance.SpawnParticleFX(end, color);
            };

        DR_GameManager.instance.CurrentMap.AddEntity(NewActor);
        NewActor.isOnMap = true;

        moveAnim.SetAnim(end, 0.2f, true, EaseType.Linear);
        return NewActor;
    }
}
