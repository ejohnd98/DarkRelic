using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    public static DR_Entity CreateActor(Sprite Sprite, string Name, int maxHealth = 10, Alignment alignment = Alignment.ENEMY){
        DR_Entity NewActor = new DR_Entity();

        NewActor.Name = Name;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewActor.AddComponent<HealthComponent>(new HealthComponent(maxHealth));
        NewActor.AddComponent<InventoryComponent>(new InventoryComponent(25));
        NewActor.AddComponent<TurnComponent>(new TurnComponent());
        NewActor.AddComponent<MoveAnimComponent>(new MoveAnimComponent());
        NewActor.AddComponent<AlignmentComponent>(new AlignmentComponent(alignment));
        
        return NewActor;
    }

    public static DR_Entity CreateProp(Sprite Sprite, string Name){
        DR_Entity NewProp = new DR_Entity();

        NewProp.Name = Name;
        NewProp.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewProp.AddComponent<PropComponent>(new PropComponent());
        
        return NewProp;
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
}
