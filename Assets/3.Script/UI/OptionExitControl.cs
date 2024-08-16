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
}
