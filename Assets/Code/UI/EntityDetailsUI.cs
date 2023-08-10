using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// When given an entity, this will extract any useful information and display it to the player
// Can start off as just a string
public class EntityDetailsUI : MonoBehaviour
{
    DR_Entity entity;
    public GameObject DetailsUIParent;
    public TextMeshProUGUI DetailsText; 
    public Image DetailsImage;

    public void SetEntity(DR_Entity newEntity){
        entity = newEntity;
        UpdateUI();
    }

    public void HideUI(){
        entity = null;
        DetailsUIParent.SetActive(false);
    }

    private void UpdateUI(){
        if (entity == null){
            DetailsUIParent.SetActive(false);
            return;
        }
        string detailsText = entity.Name;
        HealthComponent health = entity.GetComponent<HealthComponent>();
        if (health != null){
            detailsText += "\n" + health.currentHealth + " / " + health.maxHealth;
        }
        SpriteComponent spriteComp = entity.GetComponent<SpriteComponent>();
        if (spriteComp != null){
            DetailsImage.sprite = spriteComp.Sprite;
            DetailsImage.gameObject.SetActive(true);
        }else{
            DetailsImage.gameObject.SetActive(false);
        }

        DetailsText.text = detailsText;
        DetailsUIParent.SetActive(true);

    }
}
