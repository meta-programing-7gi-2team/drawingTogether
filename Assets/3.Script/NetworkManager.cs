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

    #region 서버 접속을 위한용도
    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);

        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        GameObject Player = PhotonNetwork.Instantiate("PhotonN", Vector3.zero, Quaternion.identity);

        int Room = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        Player.transform.SetParent(seatObjects[Room].transform);

        PhotonView PV = FindObjectOfType<PhotonView>();

        PV.RPC("UpdatePlayerPosition", RpcTarget.OthersBuffered, Room, Player.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void UpdatePlayerPosition(int roomIndex, int viewID)
    {
        GameObject player = PhotonView.Find(viewID).gameObject;

        player.transform.SetParent(seatObjects[roomIndex].transform);
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

    #region 게임 종료
    public void GameExit()
    {
        PhotonNetwork.LeaveLobby(); // 로비나가기
        PhotonNetwork.Disconnect(); // 서버연결종료
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
