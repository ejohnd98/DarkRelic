using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewContent", menuName = "DR Content")]
public class Content : ScriptableObject
{
    public string contentName;

    [SerializeReference]
    public List<DR_Component> components = new List<DR_Component>();

    public void AddComponent(DR_Component componentObject){
        components.Add(componentObject);
    }
}