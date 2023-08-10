using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIButton : MonoBehaviour
{
    public UnityEvent OnMouseDownEvents, OnMouseEnterEvents, OnMouseExitEvents;

    void OnMouseDown() {
        OnMouseDownEvents?.Invoke();
    }

    private void OnMouseEnter() {
        OnMouseEnterEvents?.Invoke();
    }

    private void OnMouseExit() {
        OnMouseExitEvents?.Invoke();
    }
}