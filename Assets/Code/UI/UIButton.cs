using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

public class UIButton : Button
{
    public UnityEvent OnMouseDownEvents, OnMouseEnterEvents, OnMouseExitEvents;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnMouseDownEvents?.Invoke();
    }

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