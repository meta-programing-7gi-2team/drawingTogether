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
    public GameObject player;
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
    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);

        player = PhotonNetwork.Instantiate("PhotonN", Vector3.zero, Quaternion.identity);

        int Room_C = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        int RoomMax = PhotonNetwork.CurrentRoom.MaxPlayers;

        for (int i = 0; i < RoomMax; i++)
        {
            seatObjects[i].SetActive(true);
        }

        for (int i = RoomMax; i < seatObjects.Length; i++)
        {
            seatObjects[i].SetActive(false);
        }

        //player.transform.SetParent(seatObjects[Room_C].transform);
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
        UserInfo_Manager.instance.OnApplicationQuit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
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
