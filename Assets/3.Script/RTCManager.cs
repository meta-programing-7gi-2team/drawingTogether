using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RTCManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] seatObjects;

    private string GetPlayerImage(Player player)
    {
        if (player.CustomProperties.ContainsKey("UserImage"))
        {
            return (string)player.CustomProperties["UserImage"];
        }
        return null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player joined. ActorNumber: " + newPlayer.ActorNumber + ", NickName: " + newPlayer.NickName);
        string Player_I = GetPlayerImage(newPlayer);

        if (seatObjects == null || seatObjects.Length == 0)
        {
            seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");
        }

        photonView.RPC("Room", RpcTarget.All, newPlayer.ActorNumber, newPlayer.NickName, Player_I);
    }

    [PunRPC]
    public void Room(int ActorNumber, string NickName, string image)
    {
        int playerIndex = ActorNumber - 1;

        if (playerIndex < seatObjects.Length)
        {
            GameObject seatObject = seatObjects[playerIndex];

            Text playerNameText = seatObject.transform.GetChild(0).GetComponent<Text>();
            Image Playerimage = seatObject.transform.GetChild(1).GetComponent<Image>();
            Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{image}");

            playerNameText.text = NickName;
            Playerimage.sprite = playerSprite;
        }
        else
        {
            Debug.Log("No available seat for player " + NickName);
        }
    }
}
