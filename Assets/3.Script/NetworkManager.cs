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

    #region ���� ������ ���ѿ뵵
    void Awake()
    {
        Connect();
    }

    private void Update()
    {
        //ServerText.text = (PhotonNetwork.CountOfPlayers) + "�� ������";
    }

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

    #region ����� ����
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(Roominput.text == "" ? "Room" + Random.Range(0, 100) : Roominput.text, RoomSetting);
    }

    public override void OnCreatedRoom() // ������� �����ϸ� ������ �޼ҵ�
    {
        Debug.Log("����� �Ϸ�");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // ���� ���� ������ٸ� ȣ��Ǵ� �޼ҵ�
    {
        Debug.Log("Test");

        for (int i = 0; i < roomList.Count && i < Room_Btu.Length; i++)
        {
            RoomInfo room = roomList[i];
            Button roomButton = Room_Btu[i];


            // ��ư�� �ؽ�Ʈ ���� (Button �Ʒ��� �ִ� Text ������Ʈ�� ã�Ƽ� ����)
            Text buttonText = roomButton.GetComponentInChildren<Text>();
            buttonText.text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})";
        }
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
