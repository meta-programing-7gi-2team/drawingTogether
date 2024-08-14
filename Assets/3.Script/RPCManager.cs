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

    private void Start()
    {
        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        Player player = PhotonNetwork.LocalPlayer;

        string userImage = NetworkManager.instance.GetPlayerImage(player);

        photonView.RPC("Room", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.NickName, userImage);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player joined. ActorNumber: " + newPlayer.ActorNumber + ", NickName: " + newPlayer.NickName);

        string Player_I = NetworkManager.instance.GetPlayerImage(newPlayer);


        // ���� ���� �÷��̾�� ���� �÷��̾���� ������ ����
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == newPlayer)
                continue; // ���� ���� �÷��̾�� �ǳʶ�

            string existingPlayerImage = NetworkManager.instance.GetPlayerImage(player);
            photonView.RPC("Room", newPlayer, player.ActorNumber, player.NickName, existingPlayerImage);
        }

        // ��� Ŭ���̾�Ʈ���� �� �÷��̾� ������ ����
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
