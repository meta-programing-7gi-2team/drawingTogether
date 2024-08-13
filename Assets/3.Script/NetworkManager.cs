using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance = null;
    public string Image_F; // DB연결해서DB에있는 이미지 파일가져와야함

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #region 서버 접속을 위한용도

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(); // 서버 연결시작
        Image_F = UserInfo_Manager.instance.info.User_Image;
        PhotonNetwork.NickName = UserInfo_Manager.instance.info.User_Name;
    }

    public override void OnConnected()
    {
        SceneManager.LoadScene("NetWork");
    }

    public override void OnConnectedToMaster() // 서버 연결 완료되면 반환되는 메소드
    {
        PhotonNetwork.JoinLobby(); // 로비 접속 시작
    }
    #endregion

    #region 게임 종료
    public void GameExit()
    {
        PhotonNetwork.LeaveLobby(); // 로비나가기
        PhotonNetwork.Disconnect(); // 서버연결종료
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    #endregion

}
