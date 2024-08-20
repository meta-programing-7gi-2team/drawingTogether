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
    public string ChangeText(string preWord)
    {
        string word = string.Empty;
        if (words != null && words.Length > 0)
        {
            // 랜덤으로 단어 선택
            string randomWord = words[random.Next(words.Length)];
            while (preWord.Equals(randomWord))
            {
                randomWord = words[random.Next(words.Length)];
            }
            word = randomWord;
        }
        else
        {
            Debug.LogError("words.txt 파일에 단어가 없습니다.");
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
    //    float duration = LimitTime;  // 슬라이더가 100에서 0으로 감소하는 데 걸리는 시간
    //    float startValue = timeSlider.value;
    //    float endValue = 0f;  // 슬라이더의 목표값 (0)

    //    while (elapsedTime < duration)
    //    {
    //        elapsedTime += Time.deltaTime;  // 경과 시간 증가
    //        timeSlider.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);  // Lerp를 사용하여 값을 부드럽게 감소
    //        yield return null;  // 다음 프레임까지 대기
    //    }

    //    timeSlider.value = endValue;  // 최종적으로 슬라이더를 0으로 설정
    //    //todo: 턴 종료 추가 필요
    //    //임시 호출
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
