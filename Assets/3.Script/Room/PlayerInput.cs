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

    public void OnUndo()
    {
        if(Input.GetMouseButton(0))
        {
            return;
        }
        if (undoStack.Count > 1)  // 최소 하나의 상태는 유지
        {
            undoStack.Pop();  // 현재 상태를 버림
            texture_clone = undoStack.Peek();  // 이전 상태로 되돌림

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
        // 새로운 Texture2D 생성 (원본 텍스처의 크기, 포맷, MipMap 여부 사용)
        Texture2D clonedTexture = new Texture2D(source.width, source.height, source.format, source.mipmapCount > 1);

        // 원본 텍스처의 모든 픽셀 데이터를 가져와서 복제본에 적용
        Color[] pixels = source.GetPixels();
        clonedTexture.SetPixels(pixels);
        clonedTexture.Apply(); // 적용하여 텍스처에 변경사항 반영

        return clonedTexture;
    }

}
