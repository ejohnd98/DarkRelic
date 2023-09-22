using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSpawner : MonoBehaviour
{
    public static FXSpawner instance;

    public static float FXDepth = -1.1f;

    public GameObject FXObj;

    void Awake()
    {
        if (instance != null){
            Debug.LogError("FXSpawner already exists!");
        }
        instance = this;
    }

    public void SpawnParticleFX(Vector2Int pos, Color color){
        GameObject FXObject = Instantiate(FXObj,new Vector3(pos.x, pos.y, FXDepth), Quaternion.identity, transform);
        var main = FXObject.GetComponent<ParticleSystem>().main;
        main.startColor = color;
    }
}
