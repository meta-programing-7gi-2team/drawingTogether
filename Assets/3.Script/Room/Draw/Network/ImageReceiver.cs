using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ImageReceiver : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private Image imageUI;
    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        // �̺�Ʈ �ڵ尡 3�� ��쿡�� ó��
        if (photonEvent.Code == 3)
        {
            byte[] imageBytes = (byte[])photonEvent.CustomData;
            Texture2D receivedTexture = new Texture2D(2, 2);
            receivedTexture.LoadImage(imageBytes); // ����Ʈ �迭�� Texture2D�� ��ȯ

            // ���� �ؽ�ó�� �ʿ��� ���� ���
            if (receivedTexture != null && imageUI != null)
            {
                // Texture2D�� Sprite�� ��ȯ
                Rect rect = new Rect(0, 0, receivedTexture.width, receivedTexture.height);
                Sprite sprite = Sprite.Create(receivedTexture, rect, new Vector2(0.5f, 0.5f));

                // UI Image ������Ʈ�� Sprite �Ҵ�
                imageUI.sprite = sprite;
            }
        }
    }
}
