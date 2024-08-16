using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFields;

    private int currentInputFieldIndex = 0;

    [SerializeField] private Texture2D texture;
    private Texture2D texture_clone;
    private Stack<Texture2D> undoStack = new Stack<Texture2D>();
    private Color32[] cur;

    // ��ũ������ �����ϴ� �޼���
    public void OnCapture()
    {
        // ���� ��¥�� �ð��� ����Ͽ� ���� �̸� ����
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string screenshotFileName = "screenshot_" + timestamp + ".png";
        string dir = "Screenshot";

        if (!Directory.Exists(Path.Combine(Application.dataPath, dir)))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, dir));
        }

        // ���� ���� ��� ���� (���� ������Ʈ ���� ��)
        string filePath = Path.Combine(Application.dataPath, dir, screenshotFileName);

        // ��ũ���� ����
        ScreenCapture.CaptureScreenshot(filePath);

        Debug.Log("Screenshot saved to: " + filePath);
    }
    public void OnNextInputField()
    {
        // ���� InputField �ε��� ���
        currentInputFieldIndex = (currentInputFieldIndex + 1) % inputFields.Length;

        // ���� InputField�� ��Ŀ�� �̵�
        EventSystem.current.SetSelectedGameObject(inputFields[currentInputFieldIndex].gameObject);
        inputFields[currentInputFieldIndex].ActivateInputField();
    }

    public void OnUndo()
    {
        if(Input.GetMouseButton(0))
        {
            return;
        }
        if (undoStack.Count > 1)  // �ּ� �ϳ��� ���´� ����
        {
            undoStack.Pop();  // ���� ���¸� ����
            texture_clone = undoStack.Peek();  // ���� ���·� �ǵ���

            ApplyTexture(texture);
        }
    }
    private void ApplyTexture(Texture2D texture)
    {
        cur = texture_clone.GetPixels32();
        texture.SetPixels32(cur);
        texture.Apply();
    }
    public void TextureChanged(Texture2D newTexture)
    {
        undoStack.Push(CloneTexture(newTexture));
    }
    public void ClearStack()
    {
        undoStack.Clear();
        TextureChanged(texture);
    }

    public Texture2D CloneTexture(Texture2D source)
    {
        // ���ο� Texture2D ���� (���� �ؽ�ó�� ũ��, ����, MipMap ���� ���)
        Texture2D clonedTexture = new Texture2D(source.width, source.height, source.format, source.mipmapCount > 1);

        // ���� �ؽ�ó�� ��� �ȼ� �����͸� �����ͼ� �������� ����
        Color[] pixels = source.GetPixels();
        clonedTexture.SetPixels(pixels);
        clonedTexture.Apply(); // �����Ͽ� �ؽ�ó�� ������� �ݿ�

        return clonedTexture;
    }

}
