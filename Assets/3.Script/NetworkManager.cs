using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance = null;
    [SerializeField] private GameObject[] seatObjects;
    private int Room_Count;
    public PhotonView targetPhotonView;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            PlayerPrefs.SetInt("Player_Count", 8);
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
        PhotonNetwork.NickName = UserInfo_Manager.instance.info.User_Name;
    }

    public override void OnConnectedToMaster() // ���� ���� �Ϸ�Ǹ� �ݵǴ� �޼ҵ�
    {
        PhotonNetwork.JoinLobby(); // �κ� ���� ����
    }

    public override void OnCreatedRoom() // ������� �����ϸ� ������ �޼ҵ�
    {
        Debug.Log("����� �Ϸ�");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("IngameUI");
    }

    private void SetPlayerImage(string userImage)
    {
        Hashtable customProperties = new Hashtable();
        customProperties["UserImage"] = userImage; 
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Room_Count = PlayerPrefs.GetInt("Player_Count");

        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        for (int i = 0; i < Room_Count; i++)
        {
            seatObjects[i].SetActive(true);
        }

        for (int i = Room_Count; i < seatObjects.Length; i++)
        {
            seatObjects[i].SetActive(false);
        }

        SetPlayerImage(UserInfo_Manager.instance.info.User_Image);
        PhotonNetwork.Instantiate("PhotonN", Vector3.zero, Quaternion.identity);
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
