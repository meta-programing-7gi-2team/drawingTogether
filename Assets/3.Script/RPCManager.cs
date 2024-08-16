using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RPCManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] seatObjects;
    [SerializeField] private GameObject Start_Btu;
    private List<Queue<Player>> Game_Num = new List<Queue<Player>>();

    public void RoomJoinRpc()
    {
        if(PhotonNetwork.IsMasterClient)
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

            seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

            string userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

            photonView.RPC("Room", PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer.NickName, userImage);
        }
        else
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

            seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

            Invoke("Room_C", 0.5f);

            Start_Btu = GameObject.FindGameObjectWithTag("GameStart");

            Start_Btu.SetActive(false);
        }
    }

    public void Room_C()
    {
        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        string userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

        photonView.RPC("Room", PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer.NickName, userImage);

        photonView.RPC("Room", RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName, userImage);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);       

        photonView.RPC("Room", newPlayer, PhotonNetwork.LocalPlayer.NickName, userImage);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        photonView.RPC("LeaveRoom", RpcTarget.Others, otherPlayer.NickName);
    }

    [PunRPC]
    public void LeaveRoom(string NickName)
    {
        foreach (GameObject seat in seatObjects)
        {
            Text playerNameText = seat.transform.GetChild(0).GetComponent<Text>();

            if (playerNameText.text == NickName)
            {
                playerNameText.text = string.Empty;

                Image playerImage = seat.transform.GetChild(1).GetComponent<Image>();
                playerImage.sprite = null;
                Color color = playerImage.color;
                color.a = 0;
                playerImage.color = color;

                Debug.Log($"{NickName} has left {seat.name}");
                break;
            }
        }
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
