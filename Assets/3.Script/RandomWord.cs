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
        // Resources �������� words.txt ������ TextAsset���� �ε�
        TextAsset wordFile = Resources.Load<TextAsset>("words");

        if (wordFile != null)
        {
            // TextAsset�� ������ �� �پ� �迭�� ��ȯ
            words = wordFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogError("Resources ������ words.txt ������ �����ϴ�.");
        }
    }
    public void ChangeText()
    {
        if (words != null && words.Length > 0)
        {
            // �������� �ܾ� ����
            string randomWord = words[random.Next(words.Length)];
            while (text.text.Equals(randomWord))
            {
                randomWord = words[random.Next(words.Length)];
            }
            text.text = randomWord;
        }
        else
        {
            Debug.LogError("words.txt ���Ͽ� �ܾ �����ϴ�.");
        }
    }
}
