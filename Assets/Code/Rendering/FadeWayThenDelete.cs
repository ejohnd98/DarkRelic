using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAwayThenDelete : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color spriteColor = Color.white;
    public float fadeTime = 0.4f;
    public float progress = 0.0f;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if(progress < fadeTime){
            progress += Time.deltaTime;
            spriteColor.a = 1.0f - (progress/fadeTime);
            spriteRenderer.color = spriteColor;
        }else{
            Destroy(this.gameObject);
        }
    }
}
