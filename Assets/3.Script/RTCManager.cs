using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RTCManager : MonoBehaviourPunCallbacks
{

    public override void OnEnable()
    {
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player joined. ActorNumber: " + newPlayer.ActorNumber + ", NickName: " + newPlayer.NickName);
        //hotonView.RPC("Room", RpcTarget.All, newPlayer.ActorNumber, newPlayer.NickName);
    }

}
