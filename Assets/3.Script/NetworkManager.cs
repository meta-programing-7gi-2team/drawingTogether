using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance = null;
    public string roomName;
    private int Room_Count;
    [SerializeField] private GameObject[] seatObjects;

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

    public void JoinRoom()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        roomName = clickedButton.transform.GetChild(0).GetComponent<Text>().text;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("IngameUI");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPlayerImage(UserInfo_Manager.instance.info.User_Image);

        PhotonNetwork.JoinRoom(roomName);

        roomName = string.Empty;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);

        PhotonNetwork.Instantiate("PhotonN", Vector3.zero, Quaternion.identity);
    }

    public void SetPlayerImage(string userImage)
    {
        Hashtable customProperties = new Hashtable();
        customProperties["UserImage"] = userImage;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public string GetPlayerImage(Player player)
    {
        if (player.CustomProperties.ContainsKey("UserImage"))
        {
            return (string)player.CustomProperties["UserImage"];
        }
        return null;
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
