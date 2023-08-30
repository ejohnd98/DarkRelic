using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Entity
{
    public string Name = "Untitled Entity";
    public Vector2Int Position;
    public int id = -1;

    public bool noLongerValid = false;
    public bool isOnMap = false;

    public List<DR_Component> ComponentList;

    public DR_Entity(){
        ComponentList = new List<DR_Component>();
        DR_GameManager.instance.entitesCreated++;
        id = DR_GameManager.instance.entitesCreated;
    }

    public bool Equals(DR_Entity other){
        return (id != -1) && (id == other.id);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj as DR_Entity);
    }

    public override int GetHashCode()
    {
        return id;
    }

    public T AddComponent<T>(T NewComponent) where T : DR_Component
    {
        if (HasComponent<T>()){
            Debug.LogError(Name + " already has the following component: " + typeof(T).Name);
            return null;
        }

        ComponentList.Add(NewComponent);
        NewComponent.Entity = this;

        return NewComponent;
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

    public void RemoveComponentByType<T>() where T : DR_Component
    {
        DR_Component componentToRemove = null;
        foreach (DR_Component component in ComponentList)
        {
            if (component.GetType().Equals(typeof(T)))
            {
                componentToRemove = component;
            }
        }
        RemoveComponent(componentToRemove);
    }

    public void RemoveComponent(DR_Component Component)
    {
        if (Component != null){
            Component.OnComponentRemoved();
            ComponentList.Remove(Component);
        }
    }

    public void DestroyEntity(){
        foreach (DR_Component component in ComponentList)
        {
            component.OnComponentRemoved();
        }
    }

    public bool HasComponent<T>() where T : DR_Component
    {
        return GetComponent<T>() != null;
    }

    public Vector3 GetPosFloat(float z = 0.0f){
        return new Vector3(Position.x, Position.y, z);
    }

    public int DistanceTo(Vector2Int pos){
        Vector2Int diff = Position - pos;
        return Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
    }
}
