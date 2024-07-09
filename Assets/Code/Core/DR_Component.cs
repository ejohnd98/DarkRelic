using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class CopyAttribute : Attribute { }

[System.Serializable]
public abstract class DR_Component
{
    #if UNITY_EDITOR
    // Used for ContentEditor to display component names as element values
    [Copy]
    [HideInInspector]
    public string key = "";
    #endif

    public DR_Component(){
        #if UNITY_EDITOR
        key = GetType().Name;
        #endif
    }

    
    public DR_Component(DR_Component other){
        CopyDataFromComponent(other, this);
    }

    // Called after adding to an entity
    public virtual void OnComponentAdded()
    {
        
    }

    public DR_Entity Entity;

    public virtual bool Trigger(DR_GameManager gm, DR_Entity user, DR_Entity target){
        return false;
    }

    public virtual void OnComponentRemoved(){
        
    }

    public static void CopyDataFromComponent(DR_Component original, DR_Component target){
        Type sourceType = original.GetType();
        Type destinationType = target.GetType();

        foreach (FieldInfo sourceField in sourceType.GetFields())
        {
            // Check if this should be copied
            if (!Attribute.IsDefined(sourceField, typeof(CopyAttribute))){
                continue;
            }

            FieldInfo destinationField = destinationType.GetField(sourceField.Name);
            if (destinationField != null){
                var value = sourceField.GetValue(original);
                destinationField.SetValue(target, value);
            }
        }
    }

    public virtual string GetDetailsDescription(){
        return "NOT IMPLEMENTED FOR " + this.GetType().Name;
    }
}
