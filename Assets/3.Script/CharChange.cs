using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharChange : MonoBehaviour
{
    [SerializeField] private Image Setting_Img;
    [SerializeField] private Image UserImage;
    [SerializeField] private string Image_Num = string.Empty;

    private void OnEnable()
    {
        gameObject.SetActive(true);
        Setting_Img.sprite = Resources.Load<Sprite>($"Player_Image/{UserInfo_Manager.instance.info.User_Image}");
    }

    public void Image_SetBtu(string name)
    {
        Setting_Img.sprite = Resources.Load<Sprite>($"Player_Image/{name}");
        Image_Num = name;
    }

    public void Image_Complete()
    {
        UserInfo_Manager.instance.info.User_Image = Image_Num;
        // db데이터 전송
        UserImage.sprite = Resources.Load<Sprite>($"Player_Image/{Image_Num}");
        gameObject.SetActive(false);
        Image_Num = string.Empty;
    }
}
