using UnityEngine;

public static class ImageUtils
{
    public static byte[] ConvertTextureToByteArray(Texture2D texture)
    {
        // Texture2D�� PNG�� ��ȯ
        byte[] imageBytes = texture.EncodeToPNG();
        return imageBytes;
    }
}
