using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    private Vector3 pointScale = new Vector3(1.2f, 1.2f, 1f);

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.localScale = pointScale;
        AudioManager.instance.PlaySFX(AudioManager.instance.CursorOn_SFX);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.localScale = Vector3.one;
    }
}
