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
        // 이벤트 코드가 3인 경우에만 처리
        if (photonEvent.Code == 3)
        {
            byte[] imageBytes = (byte[])photonEvent.CustomData;
            Texture2D receivedTexture = new Texture2D(2, 2);
            receivedTexture.LoadImage(imageBytes); // 바이트 배열을 Texture2D로 변환

            // 받은 텍스처를 필요한 곳에 사용
            if (receivedTexture != null && imageUI != null)
            {
                // Texture2D를 Sprite로 변환
                Rect rect = new Rect(0, 0, receivedTexture.width, receivedTexture.height);
                Sprite sprite = Sprite.Create(receivedTexture, rect, new Vector2(0.5f, 0.5f));

                // UI Image 컴포넌트에 Sprite 할당
                imageUI.sprite = sprite;
            }
        }
    }
}
