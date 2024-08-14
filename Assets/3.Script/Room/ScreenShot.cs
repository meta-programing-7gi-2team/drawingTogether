using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenShot : MonoBehaviour
{
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
}
