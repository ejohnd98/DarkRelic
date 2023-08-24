using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CellObj : MonoBehaviour
{
    public SpriteRenderer overlayRenderer;

    public void SetSelected(bool selected){
        overlayRenderer.enabled = selected;
    }
}