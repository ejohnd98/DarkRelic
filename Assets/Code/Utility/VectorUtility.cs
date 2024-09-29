using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class VectorUtility{
    public static Vector2Int V2toV2I(Vector2 vec){
        return new Vector2Int(
            Mathf.RoundToInt(vec.x),
            Mathf.RoundToInt(vec.y)
            );
    }

    public static Vector2 V2ItoV2(Vector2Int vec){
        return new Vector2(vec.x, vec.y);
    }
}