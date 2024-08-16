using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class LoginControl : MonoBehaviourPunCallbacks
{
    [Header("Login")]
    public TMP_InputField l_id_input;
    public TMP_InputField l_pw_input;
    [SerializeField] private TextMeshProUGUI L_Log;
    [SerializeField] private GameObject Loading_UI;

    [Header("SignUp")]
    public TMP_InputField name_input;
    public TMP_InputField s_id_input;
    public TMP_InputField s_pw_input;
    [SerializeField] private TextMeshProUGUI S_Log;
    [SerializeField] private GameObject SighUp_UI;


    public void Login_Btn()
    {
        if (l_id_input.text.Equals(string.Empty) || l_pw_input.text.Equals(string.Empty))
        {
            L_Log.text = "아이디와 비밀번호를 입력하세요.";
            return;
        }

        if (UserInfo_Manager.instance.IsAlreadyLoggedIn(l_id_input.text))
        {
            L_Log.text = "이미 로그인 된 계정입니다.";
            return;
        }

        if (UserInfo_Manager.instance.Login(l_id_input.text, l_pw_input.text))
        {
            User_info info = UserInfo_Manager.instance.info;
            Loading_UI.SetActive(true);
            L_Log.text = string.Empty;
            Debug.Log(info.User_ID + " | " + info.User_Password + "로그인 성공!");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            L_Log.text = "아이디와 비밀번호를 확인해주세요.";
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("MainScene");
        PhotonNetwork.NickName = UserInfo_Manager.instance.info.User_Name;
        PhotonNetwork.JoinLobby();
    }

    public void Open_SignUp()
    {
        L_Log.text = string.Empty;
        SighUp_UI.SetActive(true);
        AudioManager.instance.PlaySFX(AudioManager.instance.Click_SFX);
    }

    public void SighUp_btn()
    {

        if (name_input.text.Equals(string.Empty) || 
            s_id_input.text.Equals(string.Empty) ||
            s_pw_input.text.Equals(string.Empty))
        {
            S_Log.text = "빈칸을 모두 채워주세요";
            return;
        }

        if (UserInfo_Manager.instance.SignUp(s_id_input.text, s_pw_input.text, name_input.text))
        {
            User_info info = UserInfo_Manager.instance.info;
            SighUp_UI.SetActive(false);
            S_Log.text = string.Empty;
        }
        else
        {
            S_Log.text = "아이디가 중복되었습니다. \n다시 입력해주세요.";
        }
    }
}