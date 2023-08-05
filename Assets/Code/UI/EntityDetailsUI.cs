using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// When given an entity, this will extract any useful information and display it to the player
// Can start off as just a string
public class EntityDetailsUI : MonoBehaviour
{
    DR_Entity entity;
    public GameObject DetailsUIParent;
    public TextMeshProUGUI DetailsText; 

    public void SetEntity(DR_Entity newEntity){
        entity = newEntity;
        UpdateUI();
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

        DetailsText.text = detailsText;
        DetailsUIParent.SetActive(true);

    }
}
