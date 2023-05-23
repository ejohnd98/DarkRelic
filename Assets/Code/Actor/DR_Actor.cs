using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Actor
{
    //Separate out into components?
    public Vector2Int Position;
    public Sprite Sprite;

    public DR_Actor(Vector2Int pos, Sprite spr){
        Position = pos;
        Sprite = spr;
    }
}
