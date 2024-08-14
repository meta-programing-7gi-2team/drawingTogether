using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RTCManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] seatObjects;

    private void Start()
    {
        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");
    }

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


        // 새로 들어온 플레이어에게 기존 플레이어들의 정보를 전송
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == newPlayer)
                continue; // 새로 들어온 플레이어는 건너뜀

            string existingPlayerImage = GetPlayerImage(player);
            photonView.RPC("Room", newPlayer, player.ActorNumber, player.NickName, existingPlayerImage);
        }

        // 모든 클라이언트에게 새 플레이어 정보를 전송
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
