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

        // ���� �ð� ������ ������ �̹����� ����
        if (timeSinceLastSend >= sendInterval)
        {
            // �̹����� ����Ʈ �迭�� ��ȯ
            byte[] imageBytes = ImageUtils.ConvertTextureToByteArray(textureToSend);

            // �̹����� ��Ʈ��ũ�� ����
            imageSender.SendImage(imageBytes);

            // �ð� �ʱ�ȭ
            timeSinceLastSend = 0f;
        }
    }
}
