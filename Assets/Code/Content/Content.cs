using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentBase : ScriptableObject
{
    public string contentName;

    public string guid;

    void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
        }
    }

    public virtual Sprite GetContentSprite(){
        return null;
    }
}

[CreateAssetMenu(fileName = "NewContent", menuName = "DR Content")]
public class Content : ContentBase
{
    

    [SerializeReference]
    public List<DR_Component> components = new List<DR_Component>();

    public void AddComponent(DR_Component componentObject){
        components.Add(componentObject);
    }

    public override Sprite GetContentSprite(){
        foreach(DR_Component component in components){
            if(component is SpriteComponent spriteComp){
                return spriteComp.GetCurrentSprite();
            }
        }
        return null;
    }
}