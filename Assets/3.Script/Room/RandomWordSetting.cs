using System;
using UnityEngine;
using Random = System.Random;

public class RandomWordSetting : MonoBehaviour
{
    //[SerializeField] private Text wordText;
    //[SerializeField] private Text roundText;
    //[SerializeField] Slider timeSlider;
    //[SerializeField] private int maxRound = 8;
    private Random random = new Random();
    private string[] words;
    private int curRound = 1;
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
    public string ChangeText(string preWord)
    {
        string word = string.Empty;
        if (words != null && words.Length > 0)
        {
            // �������� �ܾ� ����
            string randomWord = words[random.Next(words.Length)];
            while (preWord.Equals(randomWord))
            {
                randomWord = words[random.Next(words.Length)];
            }
            word = randomWord;
        }
        else
        {
            Debug.LogError("words.txt ���Ͽ� �ܾ �����ϴ�.");
        }
        return word;
    }
    //public void TimeRest()
    //{
    //    roundText.text = string.Format("Round : {0}", curRound);
    //    timeSlider.value = 100;
    //    if (timeCoroutine != null)
    //    {
    //        StopCoroutine(Time_co());
    //    }
    //    timeCoroutine = StartCoroutine(Time_co());
    //}

    //private IEnumerator Time_co()
    //{
    //    float elapsedTime = 0f;
    //    float duration = LimitTime;  // �����̴��� 100���� 0���� �����ϴ� �� �ɸ��� �ð�
    //    float startValue = timeSlider.value;
    //    float endValue = 0f;  // �����̴��� ��ǥ�� (0)

    //    while (elapsedTime < duration)
    //    {
    //        elapsedTime += Time.deltaTime;  // ��� �ð� ����
    //        timeSlider.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);  // Lerp�� ����Ͽ� ���� �ε巴�� ����
    //        yield return null;  // ���� �����ӱ��� ���
    //    }

    //    timeSlider.value = endValue;  // ���������� �����̴��� 0���� ����
    //    //todo: �� ���� �߰� �ʿ�
    //    //�ӽ� ȣ��
    //    if (curRound < maxRound)
    //    {
    //        curRound++;
    //        ChangeText();
    //        TimeRest();
    //        Drawable drawable = FindObjectOfType<Drawable>();
    //        if(drawable != null)
    //        {
    //            drawable.ResetCanvas();
    //        }
    //    }
    //}
}
