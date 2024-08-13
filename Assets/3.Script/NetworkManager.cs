using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance = null;

    #region ���� ������ ���ѿ뵵
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Connect();
    }

    //private void Update()
    //{
    //    ServerText.text = (PhotonNetwork.CountOfPlayers) + "�� ������";
    //}

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(); // ���� �������
    }

    public override void OnConnectedToMaster() // ���� ���� �Ϸ�Ǹ� ��ȯ�Ǵ� �޼ҵ�
    {
        PhotonNetwork.JoinLobby(); // �κ� ���� ����
    }

    public override void OnJoinedLobby() // �κ� ���� �Ϸ�Ǹ� ��ȯ�Ǵ� �޼ҵ�
    {
        Debug.Log("�κ� ����");
    }
    #endregion

    #region ���� ����
    public void GameExit()
    {
        PhotonNetwork.LeaveLobby(); // �κ񳪰���
        PhotonNetwork.Disconnect(); // ������������
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    #endregion

}
