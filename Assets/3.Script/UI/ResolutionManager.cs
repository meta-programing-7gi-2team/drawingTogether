using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    [Header("Resolution")]
    public TMP_Dropdown dropdown;
    public RectTransform rect;

    private string[] options = { "1920x1080" , "1680x1050", "1600x900", "1280x1024", "1280x960" };
    private int width, height;
    private int cur = 0, temp = 0;

    void Start()
    {
        List<TMP_Dropdown.OptionData> optionList = new List<TMP_Dropdown.OptionData>();
        foreach(var s in options)
        {
            optionList.Add(new TMP_Dropdown.OptionData(s));
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(optionList);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(SetResolution);
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    public void SetResolution(int index)
    {
        temp = index;
        string[] splits = options[index].Split('x');
        width = int.Parse(splits[0]);
        height = int.Parse(splits[1]);
    }
    public void Apply()
    {
        cur = temp;
        Screen.SetResolution(width, height, Screen.fullScreen);
        gameObject.SetActive(false);
        AudioManager.instance.PlaySFX(AudioManager.instance.Click_SFX);
    }
    public void OnEnable()
    {
        dropdown.value = cur;
        dropdown.RefreshShownValue();
    }
}
