using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ImageSender : MonoBehaviour
{
    public void SendImage(byte[] imageBytes)
    {
        // 이벤트 코드 설정
        byte eventCode = 3;

        // Photon을 통해 이벤트로 바이트 배열 전송
        PhotonNetwork.RaiseEvent(eventCode, imageBytes, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
}