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
    [SerializeField] private string RoomName;
    [SerializeField] private int Player_Count = 0;
    [SerializeField] private int Time_Count = 60;
    List<RoomInfo> myList = new List<RoomInfo>();

    #region ¹æ°ü·Ã
    public override void OnJoinedLobby()
    {
        Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{UserInfo_Manager.instance.info.User_Image}");
        UserImage.sprite = playerSprite;
        Nickname.text = ($"{PhotonNetwork.NickName}");
    }

    public override void OnLeftRoom()
    {
        Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{UserInfo_Manager.instance.info.User_Image}");
        UserImage.sprite = playerSprite;
        Nickname.text = ($"{PhotonNetwork.NickName}");
    }

    public void Player_Count_T(int Player)
    {
        PlayerPrefs.SetInt("Player_Count", Player);
    }
    public void Time_Count_T(int Time)
    {
        PlayerPrefs.SetInt("Time_Count", Time);
    }

    public void CreateRoom()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadScene("IngameUI");
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        NetworkManager.instance.SetPlayerImage(UserInfo_Manager.instance.info.User_Image);

        Player_Count = PlayerPrefs.GetInt("Player_Count");

        RoomOptions RoomSetting = new RoomOptions { MaxPlayers = Player_Count, IsVisible = true, IsOpen = true, EmptyRoomTtl = 0 };
        PhotonNetwork.CreateRoom(Roominput.text == "" ? "Room" + Random.Range(0, 100) : Roominput.text, RoomSetting);

        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public void JoinRoom()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
        RoomName = clickedButton.transform.GetChild(0).GetComponent<Text>().text;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("IngameUI");
        AudioManager.instance.PlayBGM(AudioManager.instance.InGame_BGM);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        NetworkManager.instance.SetPlayerImage(UserInfo_Manager.instance.info.User_Image);

        PhotonNetwork.JoinRoom(RoomName);

        RoomName = string.Empty;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;

        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }

        for (int i = 0; i < Room_Btu.Length; i++)
        {
            Button roomButton = Room_Btu[i];

            Text RoomText = roomButton.transform.GetChild(0).GetComponent<Text>();
            Text PlayerText = roomButton.transform.GetChild(1).GetComponent<Text>();

            RoomText.text = string.Empty;
            PlayerText.text = string.Empty;

            roomButton.interactable = false;
        }

        for (int i = 0; i < myList.Count; i++)
        {
            RoomInfo room = myList[i];
            Button roomButton = Room_Btu[i]
                ;
            roomButton.interactable = true;

            Text RoomText = roomButton.transform.GetChild(0).GetComponent<Text>();
            Text PlayerText = roomButton.transform.GetChild(1).GetComponent<Text>();
            RoomText.text = room.Name;
            PlayerText.text = $"({room.PlayerCount}/{room.MaxPlayers})";
        }
    }

    #endregion
}
