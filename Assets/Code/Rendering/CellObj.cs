using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CellObj : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer overlayRenderer;
    public SpriteRenderer bloodStainOverlay;
    public SpriteRenderer bloodOverlay;

    public bool hasAnimation = false;
    public Sprite[] animFrames;
    public float animTimer = 0.0f;
    public float animLength = 2.0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBlood(DR_Cell cell){
        if (cell == null){
            bloodOverlay.gameObject.SetActive(false);
            bloodStainOverlay.gameObject.SetActive(false);
            return;
        }
        //TODO: later could use different sprites depending on amount
        bloodOverlay.gameObject.SetActive(cell.blood > 0);
        bloodStainOverlay.gameObject.SetActive(cell.bloodStained);
    }

    public void SetSelected(bool selected){
        overlayRenderer.enabled = selected;
    }

    public void SetAnim(SpriteComponent spriteComp){
        hasAnimation = spriteComp.hasAnimation;
        animFrames = spriteComp.animFrames;
        animLength = spriteComp.animLength;
        spriteRenderer.sprite = animFrames[0];
    }
    
    void Update()
    {
        if (hasAnimation){
            animTimer += Time.deltaTime / animLength;
            if (animTimer >= 1.0f){
                animTimer = 0.0f;
            }
            int spriteIndex = Mathf.FloorToInt(animTimer * animFrames.Length);
            spriteRenderer.sprite = animFrames[spriteIndex];
        }
    }
}