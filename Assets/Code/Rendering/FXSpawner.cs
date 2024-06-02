using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSpawner : MonoBehaviour
{
    public static FXSpawner instance;

    public static float FXDepth = -1.1f;
    public static float fadeTime = 0.3f;

    public GameObject FXObj;
    public Material whiteMat;

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

    public void SpawnDeathFX(DR_Entity killedEntity, Vector3 pos){
        GameObject deathSprite = new GameObject(killedEntity.Name + " deathSprite");
        deathSprite.transform.position = pos - Vector3.forward * 0.02f;
        SpriteRenderer newRenderer = deathSprite.AddComponent<SpriteRenderer>();
        newRenderer.sprite = killedEntity.GetComponent<SpriteComponent>().GetCurrentSprite();
        newRenderer.material = whiteMat;

        FadeAwayThenDelete fade = deathSprite.AddComponent<FadeAwayThenDelete>();
        fade.fadeTime = fadeTime;
    }
}
