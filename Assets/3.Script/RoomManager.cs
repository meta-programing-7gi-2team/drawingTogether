using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField Roominput;
    [SerializeField] private Button[] Room_Btu;
    [SerializeField] private Text Nickname;
    [SerializeField] private Image UserImage;
    [SerializeField] private int Player_Count = 8;

    public override void OnJoinedLobby() // �κ� ���� �Ϸ�Ǹ� ��ȯ�Ǵ� �޼ҵ�
    {
        Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{UserInfo_Manager.instance.info.User_Image}");
        UserImage.sprite = playerSprite;
        Nickname.text = ($"{PhotonNetwork.NickName} ��");
    }

    #region �����

    public void Player_Count_T(int Player)
    {
        Player_Count = Player;
    }

    public void CreateRoom()
    {
        RoomOptions RoomSetting = new RoomOptions { MaxPlayers = Player_Count, IsVisible = true, IsOpen = true, EmptyRoomTtl = 0 };

        PhotonNetwork.CreateRoom(Roominput.text == "" ? "Room" + Random.Range(0, 100) : Roominput.text, RoomSetting);

    }

    public override void OnCreatedRoom() // ������� �����ϸ� ������ �޼ҵ�
    {
        Debug.Log("����� �Ϸ�");
        SceneManager.LoadScene("IngameUI");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // ���� ���� �����Ǿ��ٸ� ȣ��Ǵ� �޼ҵ�
    {
        for (int i = 0; i < Room_Btu.Length; i++)
        {
            Button roomButton = Room_Btu[i];

            Text RoomText = roomButton.transform.GetChild(0).GetComponent<Text>();
            Text PlayerText = roomButton.transform.GetChild(1).GetComponent<Text>();

            RoomText.text = string.Empty;
            PlayerText.text = string.Empty;

            roomButton.interactable = false;
        }

        for (int i = 0; i < roomList.Count && i < Room_Btu.Length; i++)
        {
            RoomInfo room = roomList[i];
            Button roomButton = Room_Btu[i];

            if (room.RemovedFromList || room.PlayerCount == 0)
            {
                continue;
            }

            roomButton.interactable = true;

            Text RoomText = roomButton.transform.GetChild(0).GetComponent<Text>();
            Text PlayerText = roomButton.transform.GetChild(1).GetComponent<Text>();
            RoomText.text = room.Name;
            PlayerText.text = $"({room.PlayerCount}/{room.MaxPlayers})";
        }
    }

    public void JoinRoom()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        string roomName = clickedButton.transform.GetChild(0).GetComponent<Text>().text;
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);
        GameObject myObject = PhotonNetwork.Instantiate("PhotonN", Vector3.zero, Quaternion.identity);
        PhotonView photonView = myObject.GetComponent<PhotonView>();
        Debug.Log("Instantiated object with Photon View ID: " + photonView.ViewID);
    }

}
