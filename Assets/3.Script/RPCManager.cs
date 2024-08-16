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

    private void Start()
    {
        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("나는 방장");

            int RoomMax = PhotonNetwork.CurrentRoom.MaxPlayers;

            for (int i = 0; i < RoomMax; i++)
            {
                seatObjects[i].SetActive(true);
            }

            for (int i = RoomMax; i < seatObjects.Length; i++)
            {
                seatObjects[i].SetActive(false);
            }

            seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

            userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

            photonView.RPC("Room", PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer.NickName, userImage);

            photonView.RPC("Room", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer.NickName, userImage);
        }
        else
        {
            Debug.Log("나는 클라");
            int RoomMax = PhotonNetwork.CurrentRoom.MaxPlayers;

            for (int i = 0; i < RoomMax; i++)
            {
                seatObjects[i].SetActive(true);
            }

            for (int i = RoomMax; i < seatObjects.Length; i++)
            {
                seatObjects[i].SetActive(false);
            }

            //seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");
            //
            //userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);
            //
            //photonView.RPC("Room", PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer.NickName, userImage);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string Player_I = NetworkManager.instance.GetPlayerImage(newPlayer);
   
        //photonView.RPC("Room", RpcTarget.Others, newPlayer.NickName, Player_I);
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
    public void Room(string NickName, string image)
    {
        foreach (GameObject seat in seatObjects)
        {
            if (seat.transform.childCount == 4)
            {
                gameObject.transform.SetParent(seat.transform);
                Debug.Log($"Player assigned to {seat.name}");
    
                Transform parentTransform = gameObject.transform.parent;
    
                Text playerNameText = parentTransform.transform.GetChild(0).GetComponent<Text>();
                Image playerImage = parentTransform.transform.GetChild(1).GetComponent<Image>();
                Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{image}");
                Color color = playerImage.color;
                color.a = 1;
                playerImage.color = color;
    
                playerNameText.text = NickName;
                playerImage.sprite = playerSprite;
                break;
            }
    }
        }

}
