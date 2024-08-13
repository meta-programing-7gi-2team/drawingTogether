using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RandomWord : MonoBehaviour
{
    [SerializeField] private Text text;
    private System.Random random = new System.Random();
    private string[] words;
    private void Awake()
    {
        // Resources 폴더에서 words.txt 파일을 TextAsset으로 로드
        TextAsset wordFile = Resources.Load<TextAsset>("words");

        if (wordFile != null)
        {
            // TextAsset의 내용을 한 줄씩 배열로 변환
            words = wordFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogError("Resources 폴더에 words.txt 파일이 없습니다.");
        }
    }
    public void ChangeText()
    {
        if (words != null && words.Length > 0)
        {
            // 랜덤으로 단어 선택
            string randomWord = words[random.Next(words.Length)];
            while (text.text.Equals(randomWord))
            {
                randomWord = words[random.Next(words.Length)];
            }
            text.text = randomWord;
        }
        else
        {
            Debug.LogError("words.txt 파일에 단어가 없습니다.");
        }
    }
}
