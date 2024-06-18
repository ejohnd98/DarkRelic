using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Ability
{
    public bool triggeredByPlayer = true;
    public Sprite sprite;

    public virtual bool CanBePerformed(){
        return true;
    }

    public virtual void OnTrigger(DR_Event e){
        TestEvent testEvent = e as TestEvent;
        Debug.Log("OnTrigger: " + this.GetType().Name + " from: " + testEvent.GetType().Name + ", " + testEvent.test);
    }

    //TODO: need to think about how this can be driven by scriptable objects as that's where the sprite, name, and description would be
    // Don't necessarily want to make some elaborate generic system though, but can maybe just specify those things, and then see
    // if a dropdown can be created with the ability child class
    // Then when creating the ability at runtime it will just assign the data from the scriptable object to the ability instance

    // Where do entity specific events live?
    // They shouldn't necessarily live on DR_Entity
    // Perhaps they are added to each individual component?
    // What about stuff not tied to components, such as door opening?
    
    // Possibly it does live on the DR_Entity, and then the components trigger them
    // Then things can subscribe to them and they can be triggered without worry.

    // CURRENT TODO:
    // Move ability to scriptable object (initially can be default type?)
    // Create an ability action for those which are player triggered. Can be pretty barebones, but will have the "waiting for input" stage if needed

    // Future:
    // Have relic grant ability
    // Add events for basic stuff on Entity and some way for abilities to subscribe (or just hardcode that)
}

public class TestAbility : DR_Ability
{
    public TestAbility(){
        DR_EventSystem.TestEvent += OnTrigger;
        sprite = GameRenderer.instance.FloorTexture;
    }
}
