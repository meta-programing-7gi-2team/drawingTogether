using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class Setup_Login_Controller : MonoBehaviourPunCallbacks
{
    [Header("Login")]
    public TMP_InputField ID_input;
    public TMP_InputField Password_input;
    [SerializeField] private TextMeshProUGUI Log;
    [SerializeField] private GameObject Login_UI;
    [SerializeField] private GameObject Loding_UI;
    [SerializeField] private GameObject Room_Panel;

    [Header("SetUp")]
    public TMP_InputField NickName_input;
    public TMP_InputField id_input;
    public TMP_InputField password_input;
    [SerializeField] private TextMeshProUGUI S_Log;
    [SerializeField] private GameObject SetUp_UI;

    public void Login_Btn()
    {
        if (ID_input.text.Equals(string.Empty) || Password_input.text.Equals(string.Empty))
        {
            Log.text = "���̵�� ��й�ȣ�� �Է��ϼ���.";
            return;
        }

        if (UserInfo_Manager.instance.Login(ID_input.text, Password_input.text))
        {
            User_info info = UserInfo_Manager.instance.info;
            Loding_UI.SetActive(true);
            Log.text = string.Empty;
            NetworkManager.instance.Connect();
            Debug.Log(info.User_ID + " | " + info.User_Password + "�α��� ����!");
        }
        else
        {
            Log.text = "���̵�� ��й�ȣ�� Ȯ�����ּ���.";
        }
    }

    public void SetActive_SetUpUI()
    {
        Login_UI.SetActive(false);
        Log.text = string.Empty;
        SetUp_UI.SetActive(true);
    }

    public void SetUp_Btn()
    {
        if (NickName_input.text.Equals(string.Empty) || id_input.text.Equals(string.Empty) || password_input.text.Equals(string.Empty))
        {
            S_Log.text = "�г��Ӱ� ���̵�, ��й�ȣ�� �Է��ϼ���.";
            return;
        }

        if (UserInfo_Manager.instance.SetUp(id_input.text, password_input.text, NickName_input.text))
        {
            User_info info = UserInfo_Manager.instance.info;
            SetUp_UI.SetActive(false);
            S_Log.text = string.Empty;
            Login_UI.SetActive(true);
        }
        else
        {
            S_Log.text = "���̵� �ߺ��Ǿ����ϴ�. \n�ٽ� �Է����ּ���.";
        }
    }

    public override void OnJoinedLobby()
    {
        Loding_UI.SetActive(false);
        Room_Panel.SetActive(true);
    }
}