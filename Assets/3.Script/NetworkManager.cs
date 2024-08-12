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
        ServerText.text = (PhotonNetwork.CountOfPlayers) + "명 접속중";
    }

    public void Connect()
    {
        Debug.Log("서버에 접속 중");

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버에 연결 완료");
        PhotonNetwork.NickName = testname;
        playerNicknames.Add(PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 연결 완료");
        UpdatePlayerListText();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비에서 퇴장");
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
