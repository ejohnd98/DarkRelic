using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CellObj : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer overlayRenderer;
    public SpriteRenderer bloodStainOverlay;
    public SpriteRenderer bloodOverlay;
    public SpriteRenderer webOverlayTemp;
    public SpriteRenderer altarItem;
    public Canvas altarPriceCanvas;
    public TextMeshProUGUI altarPrice;

    public bool hasAnimation = false;
    public Sprite[] animFrames;
    public float animTimer = 0.0f;
    public float animLength = 2.0f;

    public AltarComponent altar;

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

    public void SetAltarItem(AltarComponent altar){
        this.altar = altar;
        if (altar.interactable && (altar.altarType == AltarType.ITEM_ALTAR || altar.altarType == AltarType.CURSED_ALTAR)){
            altarItem.gameObject.SetActive(true);
            altarItem.sprite = altar.altarAbilityContent.abilitySprite;
            altar.OnAltarUsed += UpdateAltarItemVisibility;
        }
    }

    public void UpdateAltarItemVisibility(DR_Event e){
        altarItem.gameObject.SetActive(altar.interactable);
    }

    void OnDestroy()
    {
        if (altar != null){
            altar.OnAltarUsed -= UpdateAltarItemVisibility;
        }
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