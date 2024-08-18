using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance = null;
    public GameObject test;
    public string roomName;
    public bool Game_Check = false;

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

    #region 서버 접속을 위한용도
    public override void OnJoinedRoom()
    {
        test = PhotonNetwork.Instantiate("PhotonN", Vector3.zero, Quaternion.identity);

        RPCManager rpc = test.GetComponent<RPCManager>();

        rpc.RoomJoinRpc();
        int Time_Count = PlayerPrefs.GetInt("Time_Count");
        SetTime(Time_Count);
    }
    public void SetTime(int time)
    {
        if (!PhotonNetwork.InRoom)  // 방에 들어가 있는지 확인
            return;

        PhotonHashTable roomSetting = PhotonNetwork.CurrentRoom.CustomProperties;
        if (roomSetting.ContainsKey("PlayTime"))
        {
            roomSetting["PlayTime"] = time;
        }
        else
        {
            roomSetting.Add("PlayTime", time);
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomSetting);
    }
    public void SetPlayerImage(string userImage)
    {
        PhotonHashTable customProperties = new PhotonHashTable();
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

    #region 게임 종료
    public void GameExit()
    {
        PhotonNetwork.LeaveLobby(); // 로비나가기
        PhotonNetwork.Disconnect(); // 서버연결종료
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
