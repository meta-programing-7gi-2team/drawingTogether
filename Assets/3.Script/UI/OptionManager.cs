using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public RectTransform rect;
    private string[] options = { "1920x1080" , "1680x1050", "1600x900", "1280x1024", "1280x960" };
    private int width, height;

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
        string[] splits = options[index].Split('x');
        width = int.Parse(splits[0]);
        height = int.Parse(splits[1]);
        Debug.Log(width);
        Debug.Log(height);
    }
    public void Apply()
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }
}
