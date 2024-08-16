using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;


public class ChatBubbleControl : MonoBehaviourPunCallbacks
{
    public InputField input_F; // 공용
    public Text word; // 각 플레이어
    public TMP_Text text;
    public Transform bubble; // 각 플레이어

    private void Start()
    {
        Invoke("Test", 0.8f);
    }

    private void Test()
    {
        GameObject inputs = GameObject.FindGameObjectWithTag("Input");
        GameObject TMP = GameObject.FindGameObjectWithTag("TMPChat");
        GameObject parentObject = gameObject.transform.parent.gameObject;

        Debug.Log(parentObject);

        text = TMP.GetComponent<TMP_Text>();
        input_F = inputs.GetComponent<InputField>();
        word = parentObject.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
        bubble = parentObject.transform.GetChild(3).GetComponent<Transform>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!photonView.IsMine) return;

            if(!NetworkManager.instance.Game_Check)
            {
                Send();
            }
            else
            {
                Bubble_Send();
            }
        }
    }

    public void Send()
    {
        photonView.RPC("Chat", RpcTarget.All, PhotonNetwork.LocalPlayer, input_F.text);
        input_F.text = string.Empty;

        EventSystem.current.SetSelectedGameObject(input_F.gameObject, null);
        input_F.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    public void Bubble_Send()
    {
        photonView.RPC("BubbleChat", RpcTarget.All, input_F.text);
        input_F.text = string.Empty;

        EventSystem.current.SetSelectedGameObject(input_F.gameObject, null);
        input_F.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void RemoveExtraLines()
    {
        if (text.textInfo.lineCount > 18)
        {
            // 텍스트를 줄 단위로 나눔
            string[] lines = text.text.Split('\n');

            // 첫 번째 줄을 제거하고 남은 줄을 다시 조합
            string newText = string.Join("\n", lines, 1, lines.Length - 1);

            // 텍스트 업데이트
            text.text = newText;
        }
    }

    [PunRPC]
    public void Chat(Player player, string message)
    {
        text.text += "\n" + player.NickName + " : " + message;
        RemoveExtraLines();
    }

    [PunRPC]
    public void BubbleChat(string message)
    {
        word.text = message;

        bubble.DOScale(Vector3.one, 0.2f);

        Invoke("Close", 1.5f);
    }

    public void Close()
    {
        bubble.DOScale(Vector3.zero, 0.2f);
    }

}
