using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeToggle : MonoBehaviour {
    
    [HideInInspector]
    public bool isFading = false;

    public AnimationCurve animCurve;
    
    private bool shouldBeVisible = false;
    private float counter = 0.0f;
    private float duration = 0.5f;
    private Image image;
    
    public event Action OnVisibleComplete;
    public event Action OnInvisibleComplete;

    private void Awake() {
        image = GetComponent<Image>();
        counter = image.color.a;
    }

    [ContextMenu("SetVisible")]
    public void SetVisible(){
        SetShouldBeVisible(true);
    }

    [ContextMenu("SetInvisible")]
    public void SetInvisible(){
        SetShouldBeVisible(false);
    }

    public void SetShouldBeVisible(bool isVisible) {
        shouldBeVisible = isVisible;
        isFading = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFading) {
            return;
        }
        
        if (shouldBeVisible && counter < 1.0f) {
            counter += Time.deltaTime / duration;
            if (counter > 1.0f) {
                counter = 1.0f;
                isFading = false;
                OnVisibleComplete?.Invoke();
            }
        } else if (!shouldBeVisible && counter > 0.0f) {
            counter -= Time.deltaTime / duration;
            if (counter < 0.0f) {
                counter = 0.0f;
                isFading = false;
                OnInvisibleComplete?.Invoke();
            }
        }

        Color imageColor = image.color;
        imageColor.a = animCurve.Evaluate(counter);
        image.color = imageColor;
    }
}
