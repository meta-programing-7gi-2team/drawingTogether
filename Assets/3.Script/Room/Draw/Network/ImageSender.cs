using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ImageSender : MonoBehaviour
{
    public void SendImage(byte[] imageBytes)
    {
        // �̺�Ʈ �ڵ� ����
        byte eventCode = 3;

        // Photon�� ���� �̺�Ʈ�� ����Ʈ �迭 ����
        PhotonNetwork.RaiseEvent(eventCode, imageBytes, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
}