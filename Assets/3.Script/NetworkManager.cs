using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string testname;
    [SerializeField] private InputField Roominput;
    [SerializeField] private Button[] Room_Btu;
    private RoomOptions RoomSetting = new RoomOptions { MaxPlayers = 8, IsVisible = true, IsOpen = true, EmptyRoomTtl = 0 };

    #region 서버 접속을 위한용도
    void Awake()
    {
        Connect();
    }

    private void Update()
    {
        //ServerText.text = (PhotonNetwork.CountOfPlayers) + "명 접속중";
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(); // 서버 연결시작
    }

    public override void OnConnectedToMaster() // 서버 연결 완료되면 반환되는 메소드
    {
        PhotonNetwork.JoinLobby(); // 로비 접속 시작
    }

    public override void OnJoinedLobby() // 로비 접속 완료되면 반환되는 메소드
    {
        Debug.Log("로비 연결");
    }
    #endregion

    #region 방생성 관련
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(Roominput.text == "" ? "Room" + Random.Range(0, 100) : Roominput.text, RoomSetting);
    }

    public override void OnCreatedRoom() // 방생성에 성공하면 나오는 메소드
    {
        Debug.Log("방생성 완료");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // 방이 새로 생성됬다면 호출되는 메소드
    {
        Debug.Log("Test");

        for (int i = 0; i < roomList.Count && i < Room_Btu.Length; i++)
        {
            RoomInfo room = roomList[i];
            Button roomButton = Room_Btu[i];


            // 버튼의 텍스트 설정 (Button 아래에 있는 Text 컴포넌트를 찾아서 설정)
            Text buttonText = roomButton.GetComponentInChildren<Text>();
            buttonText.text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})";
        }
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
