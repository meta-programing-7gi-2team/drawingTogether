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
    [SerializeField] private GameObject[] seatObjects;
    public string roomName;
    public PhotonView targetPhotonView;
    private int Room_Count;

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

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(); // 서버 연결시작
        PhotonNetwork.NickName = UserInfo_Manager.instance.info.User_Name;
    }

    public override void OnConnectedToMaster() // 서버 연결 완료되면 콜되는 메소드
    {
        PhotonNetwork.JoinLobby(); // 로비 접속 시작
    }

    public override void OnCreatedRoom() // 방생성에 성공하면 나오는 메소드
    {
        Debug.Log("방생성 완료");

        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        Room_Count = PlayerPrefs.GetInt("Player_Count");

        for (int i = 0; i < Room_Count; i++)
        {
            seatObjects[i].SetActive(true);
        }

        for (int i = Room_Count; i < seatObjects.Length; i++)
        {
            seatObjects[i].SetActive(false);
        }
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
    }

    public void SetPlayerImage(string userImage)
    {
        Hashtable customProperties = new Hashtable();
        customProperties["UserImage"] = userImage;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);

        PhotonNetwork.Instantiate("PhotonN", Vector3.zero, Quaternion.identity);
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
