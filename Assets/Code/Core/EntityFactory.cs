using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : MonoBehaviour
{
    public static DR_Entity CreateActor(Sprite Sprite, string Name, int maxHealth = 10){
        DR_Entity NewActor = new DR_Entity();

        NewActor.Name = Name;
        NewActor.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewActor.AddComponent<HealthComponent>(new HealthComponent(maxHealth));
        NewActor.AddComponent<InventoryComponent>(new InventoryComponent(9));
        NewActor.AddComponent<TurnComponent>(new TurnComponent());
        NewActor.AddComponent<MoveAnimComponent>(new MoveAnimComponent());
        
        return NewActor;
    }

    public static DR_Entity CreateProp(Sprite Sprite, string Name){
        DR_Entity NewProp = new DR_Entity();

        NewProp.Name = Name;
        NewProp.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewProp.AddComponent<PropComponent>(new PropComponent());
        
        return NewProp;
    }

    public static DR_Item CreateHealingItem(Sprite Sprite, string Name, int healAmount){
        DR_Item NewItem = new DR_Item();

        NewItem.Name = Name;
        NewItem.AddComponent<ItemComponent>(new ItemComponent(NewItem));
        NewItem.AddComponent<SpriteComponent>(new SpriteComponent(Sprite));
        NewItem.AddComponent<HealingConsumableComponent>(new HealingConsumableComponent(healAmount));
        
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
