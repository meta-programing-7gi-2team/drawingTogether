using UnityEngine;

public static class ImageUtils
{
    public static byte[] ConvertTextureToByteArray(Texture2D texture)
    {
        // Texture2D를 PNG로 변환
        byte[] imageBytes = texture.EncodeToPNG();
        return imageBytes;
    }
}
