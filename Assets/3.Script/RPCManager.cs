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
    [SerializeField] private string TargetObject;
    private List<Queue<Player>> Game_Num = new List<Queue<Player>>();

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

        seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");
    }

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

            foreach (GameObject seat in seatObjects)
            {
                if (seat.transform.childCount == 4)
                {
                    gameObject.transform.SetParent(seat.transform);
                    Debug.Log($"Player assigned to {seat.name}");

                    TargetObject = seat.name;
                    break;
                }
            }

            seatObjects = GameObject.FindGameObjectsWithTag("Player_Room");

            string userImage = NetworkManager.instance.GetPlayerImage(PhotonNetwork.LocalPlayer);

            photonView.RPC("Room", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, userImage, TargetObject);
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

        foreach (GameObject seat in seatObjects)
        {
            if (seat.transform.childCount == 4)
            {
                gameObject.transform.SetParent(seat.transform);
                Debug.Log($"Player assigned to {seat.name}");

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
    public void Room(string NickName, string image, string targetObjectName)
    {
        Debug.Log(NickName);
        Debug.Log("Target Object Name: " + targetObjectName);

        // targetObjectName을 사용하여 오브젝트 찾기
        GameObject targetObject = GameObject.Find(targetObjectName);

        if (targetObject != null)
        {
            // 부모-자식 관계 설정 또는 다른 로직 수행
            gameObject.transform.SetParent(targetObject.transform);
            Debug.Log($"Player assigned to {targetObject.name}");

            // UI 설정
            Text playerNameText = targetObject.transform.GetChild(0).GetComponent<Text>();
            Image playerImage = targetObject.transform.GetChild(1).GetComponent<Image>();
            Sprite playerSprite = Resources.Load<Sprite>($"Player_Image/{image}");
            Color color = playerImage.color;
            color.a = 1;
            playerImage.color = color;

            playerNameText.text = NickName;
            playerImage.sprite = playerSprite;
        }
        else
        {
            Debug.LogError($"Target object with name {targetObjectName} not found.");
        }
    }

}
