using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewContent", menuName = "Custom/Content")]
public class Content : ScriptableObject
{
    public enum ComponentEnum{
        SpriteComponent,
        HealthComponent,
    }

    public string contentName;

    [SerializeReference]
    public List<DR_Component> components = new List<DR_Component>();

    public void AddComponent(DR_Component componentObject){
        Debug.Log(componentObject);
        components.Add(componentObject);
    }
}