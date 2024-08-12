using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text ServerText;
    [SerializeField] private string testname;
    private List<string> playerNicknames = new List<string>();
    public Text playerListText;

    void Awake()
    {
        Connect();
    }

    private void Update()
    {
        ServerText.text = (PhotonNetwork.CountOfPlayers) + "�� ������";
    }

    public void Connect()
    {
        Debug.Log("������ ���� ��");

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ���� �Ϸ�");
        PhotonNetwork.NickName = testname;
        playerNicknames.Add(PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� �Ϸ�");
        UpdatePlayerListText();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("�κ񿡼� ����");
        playerNicknames.Remove(PhotonNetwork.NickName);
        UpdatePlayerListText();
    }

    public void test()
    {
        PhotonNetwork.LeaveLobby();
    }

    private void UpdatePlayerListText()
    {
        foreach (string nickname in playerNicknames)
        {
            playerListText.text += nickname + "\n";
        }
    }

}
