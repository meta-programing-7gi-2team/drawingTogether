using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ChatBubbleControl : MonoBehaviour
{
    public InputField input;
    public Text word;
    public Transform bubble;


    public void Send()
    {
        word.text = input.text;
        input.text = string.Empty;
        bubble.DOScale(Vector3.one, 0.2f);

        Invoke("Close", 1.5f);
    }

    public void Close()
    {
        bubble.DOScale(Vector3.zero, 0.2f);
    }
}
