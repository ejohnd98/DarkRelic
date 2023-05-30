using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Entity
{
    public string Name = "Untitled Entity";
    public Vector2Int Position;

    public List<DR_Component> ComponentList;

    public DR_Entity(){
        ComponentList = new List<DR_Component>();
    }

    public bool AddComponent<T>(T NewComponent) where T : DR_Component
    {
        if (HasComponent<T>()){
            Debug.LogError(Name + " already has the following component: " + typeof(T).Name);
            return false;
        }

        ComponentList.Add(NewComponent);
        NewComponent.Entity = this;

        return true;
    }

    public T GetComponent<T>() where T : DR_Component
    {
        foreach (DR_Component component in ComponentList)
        {
            if (component.GetType().Equals(typeof(T)))
            {
                return (T)component;
            }
        }
        return null;
    }

    public bool HasComponent<T>() where T : DR_Component
    {
        return GetComponent<T>() != null;
    }

    public Vector3 GetPosFloat(float z = 0.0f){
        return new Vector3(Position.x, Position.y, z);
    }
}
