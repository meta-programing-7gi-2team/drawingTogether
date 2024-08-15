using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RPCManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] seatObjects;
    private string userImage;
    private List<Queue> RoomList = new List<Queue>();

    private void Start()
    {
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

        userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

        if (photonView.IsMine)
        {
            int Room = PhotonNetwork.CurrentRoom.PlayerCount - 1;
            transform.SetParent(seatObjects[Room].transform);

            photonView.RPC("Player_C", RpcTarget.OthersBuffered, Room);

            photonView.RPC("Room", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.NickName, userImage);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string Player_I = NetworkManager.instance.GetPlayerImage(newPlayer);
    
        // 새로 들어온 플레이어에게 기존 플레이어들의 정보를 전송
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == newPlayer)
                continue; // 새로 들어온 플레이어는 건너뜀
    
            string existingPlayerImage = NetworkManager.instance.GetPlayerImage(player);
    
            photonView.RPC("Room", newPlayer, player.ActorNumber, player.NickName, existingPlayerImage);
        }

        // 모든 클라이언트에게 새 플레이어 정보를 전송

        photonView.RPC("Room", RpcTarget.All, newPlayer.ActorNumber, newPlayer.NickName, Player_I);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer);
        photonView.RPC("LeftRoom", RpcTarget.All, otherPlayer.ActorNumber);
    }

    [PunRPC]
    public void LeftRoom(int otherPlayer)
    {
        int playerIndex = otherPlayer - 1;

        GameObject LeftPlayer = seatObjects[playerIndex];

        Text player_t = LeftPlayer.transform.GetChild(0).GetComponent<Text>();
        Image player_m = LeftPlayer.transform.GetChild(1).GetComponent<Image>();
        Color color = player_m.color;
        color.a = 0;
        player_m.color = color;

        player_t.text = string.Empty;
    }

    [PunRPC]
    public void Player_C(int room)
    {
        if (photonView.IsMine)
        {
            transform.SetParent(seatObjects[room].transform);
        }
    }

    [PunRPC]
    public void Room(int ActorNumber, string NickName, string image)
    {
        int playerIndex = ActorNumber - 1;

        if (playerIndex < seatObjects.Length)
        {
            GameObject seatObject = seatObjects[playerIndex];
        
            Text playerNameText = seatObject.transform.GetChild(0).GetComponent<Text>();
            Image playerImage = seatObject.transform.GetChild(1).GetComponent<Image>();
            Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{image}");
            Color color = playerImage.color;
            color.a = 1;
            playerImage.color = color;
        
            playerNameText.text = NickName;
            playerImage.sprite = playerSprite;
        }
        else
        {
            Debug.Log("No available seat for player " + NickName);
        }
    }
}
