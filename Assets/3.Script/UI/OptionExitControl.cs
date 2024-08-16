using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class OptionExitControl : MonoBehaviour
{
    private GameObject optionPanel;

    private void Start()
    {
        optionPanel = FindObjectOfType<ResolutionManager>(true).gameObject;
    }

    public void Open_Option()
    {
        optionPanel.SetActive(true);
    }

    public void Room_Exit()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainScene");
        AudioManager.instance.PlayBGM(AudioManager.instance.Lobby_BGM);
    }

    public void Game_Exit()
    {
        NetworkManager.instance.GameExit();
    }
    //public void Lobby_Exit()
    //{
    //    PhotonNetwork.LeaveLobby(); // 로비나가기
    //    PhotonNetwork.Disconnect(); // 서버연결종료
    //    UserInfo_Manager.instance.Logout(UserInfo_Manager.instance.info.User_ID);
    //    SceneManager.LoadScene("LoginScene");
    //    AudioManager.instance.PlayBGM(AudioManager.instance.Login_BGM);
    //}
}
