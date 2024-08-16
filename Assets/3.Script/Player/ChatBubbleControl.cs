using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        if (PhotonNetwork.IsMasterClient)
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
        else
        {
            Invoke("Test", 0.8f);
        }
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
            if (photonView.IsMine) return;

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
        photonView.RPC("Chat", RpcTarget.All, input_F.text);
    }

    public void Bubble_Send()
    {
        photonView.RPC("BubbleChat", RpcTarget.All, input_F.text);
    }

    [PunRPC]
    public void Chat(string message)
    {
        text.text += "\n" + PhotonNetwork.NickName + " : " + message;
        input_F.text = string.Empty;
    }

    [PunRPC]
    public void BubbleChat(string message)
    {
        word.text = message;
        input_F.text = string.Empty;

        bubble.DOScale(Vector3.one, 0.2f);

        Invoke("Close", 1.5f);
    }

    public void Close()
    {
        bubble.DOScale(Vector3.zero, 0.2f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(word.text);
        }
        else
        {
            word.text = (string)stream.ReceiveNext();
        }
    }
}
