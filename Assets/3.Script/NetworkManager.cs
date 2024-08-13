using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance = null;
    public string Image_F; // DB�����ؼ�DB���ִ� �̹��� ���ϰ����;���

    private void Awake()
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
    }

    #region ���� ������ ���ѿ뵵

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(); // ���� �������
        Image_F = UserInfo_Manager.instance.info.User_Image;
        PhotonNetwork.NickName = UserInfo_Manager.instance.info.User_Name;
    }

    public override void OnConnected()
    {
        SceneManager.LoadScene("NetWork");
    }

    public override void OnConnectedToMaster() // ���� ���� �Ϸ�Ǹ� ��ȯ�Ǵ� �޼ҵ�
    {
        PhotonNetwork.JoinLobby(); // �κ� ���� ����
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
