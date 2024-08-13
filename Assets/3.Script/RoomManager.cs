using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField Roominput;
    [SerializeField] private Button[] Room_Btu;
    private RoomOptions RoomSetting = new RoomOptions { MaxPlayers = 8, IsVisible = true, IsOpen = true, EmptyRoomTtl = 0 };


    #region 방관련
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(Roominput.text == "" ? "Room" + Random.Range(0, 100) : Roominput.text, RoomSetting);
    }

    public override void OnCreatedRoom() // 방생성에 성공하면 나오는 메소드
    {
        Debug.Log("방생성 완료");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // 방이 새로 생성되었다면 호출되는 메소드
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
    #endregion
}
