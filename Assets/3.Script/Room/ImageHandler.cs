using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    public Texture2D textureToSend;
    private ImageSender imageSender;
    public float sendInterval = 0.1f;
    private float timeSinceLastSend;

    private void Start()
    {
        imageSender = GetComponent<ImageSender>();
        timeSinceLastSend = 0f;
    }

    private void Update()
    {
        timeSinceLastSend += Time.deltaTime;

        // 일정 시간 간격이 지나면 이미지를 전송
        if (timeSinceLastSend >= sendInterval)
        {
            // 이미지를 바이트 배열로 변환
            byte[] imageBytes = ImageUtils.ConvertTextureToByteArray(textureToSend);

            // 이미지를 네트워크로 전송
            imageSender.SendImage(imageBytes);

            // 시간 초기화
            timeSinceLastSend = 0f;
        }
    }
}
