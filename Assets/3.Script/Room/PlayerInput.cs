using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFields; // 순서대로 넣은 InputField들

    private int currentInputFieldIndex = 0;

    // 스크린샷을 저장하는 메서드
    public void OnCapture()
    {
        // 현재 날짜와 시간을 사용하여 파일 이름 생성
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string screenshotFileName = "screenshot_" + timestamp + ".png";
        string dir = "Screenshot";

        if (!Directory.Exists(Path.Combine(Application.dataPath, dir)))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, dir));
        }

        // 파일 저장 경로 설정 (현재 프로젝트 폴더 내)
        string filePath = Path.Combine(Application.dataPath, dir, screenshotFileName);

        // 스크린샷 저장
        ScreenCapture.CaptureScreenshot(filePath);

        Debug.Log("Screenshot saved to: " + filePath);
    }
    public void OnNextInputField()
    {
        // 현재 InputField 인덱스 계산
        currentInputFieldIndex = (currentInputFieldIndex + 1) % inputFields.Length;

        // 다음 InputField로 포커스 이동
        EventSystem.current.SetSelectedGameObject(inputFields[currentInputFieldIndex].gameObject);
        inputFields[currentInputFieldIndex].ActivateInputField();
    }
}
