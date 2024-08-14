using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class Exit : MonoBehaviourPunCallbacks
{
    public void Exit_Btu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainScene");
    }
}
