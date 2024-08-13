using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingToggle : MonoBehaviour
{
    private Toggle toggle;
    private CanvasGroup icon;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        icon = GetComponent<CanvasGroup>();
    }

    public void onValueChanged()
    {
        icon.alpha = toggle.isOn ? 1f : 0.4f;
    }

}
