using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class RPCManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] seatObjects;
    [SerializeField] private GameObject Start_Btu;
    [SerializeField] private string TargetObject;

    private void BubbleSort(GameObject[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
        {
            for (int j = 0; j < arr.Length - i - 1; j++)
            {
                if (arr[j].name.CompareTo(arr[j + 1].name) > 0)
                {
                    GameObject temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
            }
        }
    }

    public void RoomJoinRpc()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

            int RoomMax = PhotonNetwork.CurrentRoom.MaxPlayers;

            BubbleSort(seatObjects);

            for (int i = 0; i < RoomMax; i++)
            {
                seatObjects[i].SetActive(true);
            }

            for (int i = RoomMax; i < seatObjects.Length; i++)
            {
                seatObjects[i].SetActive(false);
            }

            foreach (GameObject seat in seatObjects)
            {
                if (seat.transform.childCount == 4)
                {
                    gameObject.transform.SetParent(seat.transform);

                    TargetObject = seat.name;
                    break;
                }
            }

            seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

            string userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

            photonView.RPC("Room", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, userImage, TargetObject);

            Start_Btu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("GameStart"));

            Start_Btu.SetActive(true);
        }
        else
        {
            Invoke("Room_C", 0.5f);

            Start_Btu = GameObject.FindGameObjectWithTag("GameStart");

            Start_Btu.SetActive(false);
        }
    }

    public void Room_C()
    {
        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

        int RoomMax = PhotonNetwork.CurrentRoom.MaxPlayers;

        BubbleSort(seatObjects);

        for (int i = 0; i < RoomMax; i++)
        {
            seatObjects[i].SetActive(true);
        }

        for (int i = RoomMax; i < seatObjects.Length; i++)
        {
            seatObjects[i].SetActive(false);
        }

        foreach (GameObject seat in seatObjects)
        {
            if (seat.transform.childCount == 4)
            {
                gameObject.transform.SetParent(seat.transform);

                TargetObject = seat.name;
                break;
            }
        }

        string userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

        photonView.RPC("Room", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, userImage, TargetObject);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

        photonView.RPC("Room", newPlayer, PhotonNetwork.LocalPlayer.NickName, userImage, TargetObject);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        photonView.RPC("LeaveRoom", RpcTarget.All, otherPlayer.NickName);
    }

    [PunRPC]
    public void LeaveRoom(string NickName)
    {
        foreach (GameObject seat in seatObjects)
        {
            Text playerNameText = seat.transform.GetChild(0).GetComponent<Text>();
            Text playercount = seat.transform.GetChild(2).GetComponent<Text>();

            if (playerNameText.text == NickName)
            {
                playerNameText.text = string.Empty;
                playercount.text = string.Empty;

                Image playerImage = seat.transform.GetChild(1).GetComponent<Image>();
                playerImage.sprite = null;
                Color color = playerImage.color;
                color.a = 0;
                playerImage.color = color;
                break;
            }
        }
    }

    [PunRPC]
    public void Room(string NickName, string image, string targetObjectName)
    {
        GameObject targetObject = GameObject.Find(targetObjectName);

        if (targetObject != null)
        {
            gameObject.transform.SetParent(targetObject.transform);

            Text playerNameText = targetObject.transform.GetChild(0).GetComponent<Text>();
            Image playerImage = targetObject.transform.GetChild(1).GetComponent<Image>();
            Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{image}");
            Color color = playerImage.color;
            color.a = 1;
            playerImage.color = color;

            playerNameText.text = NickName;
            playerImage.sprite = playerSprite;
        }
    }
}
