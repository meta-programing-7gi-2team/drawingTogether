using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFields; // ������� ���� InputField��

    private int currentInputFieldIndex = 0;

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
}
