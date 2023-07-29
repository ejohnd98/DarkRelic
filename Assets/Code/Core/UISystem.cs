using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;
    public Transform HealthBarPivot; //TODO make healthbar wrapper class (so enemies can have health bars too)

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void RefreshUI(){
        UpdateHealthBar();
    }

    void UpdateHealthBar(){
        HealthComponent PlayerHealth = DR_GameManager.instance.GetPlayer().GetComponent<HealthComponent>();
        float HealthFraction = Mathf.Clamp01(PlayerHealth.currentHealth / (float) PlayerHealth.maxHealth);

        HealthBarPivot.localScale = new Vector3(HealthFraction, 1.0f, 1.0f);
    }

    void Update()
    {
        //TODO Call this from other parts of game when needed instead of every tick
        RefreshUI();
    }
}
