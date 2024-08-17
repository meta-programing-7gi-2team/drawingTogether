using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button Start_Btu;
    [SerializeField] private GameObject DrawingTool;
    [SerializeField] private GameObject InGameUI;
    [SerializeField] private GameObject InputField;
    private Queue<Player> players_Q = new Queue<Player>();
    private Player currentPlayer;
    private string GameWord = string.Empty;

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameObject Btu = GameObject.FindGameObjectWithTag("GameStart");

            Start_Btu = Btu.GetComponent<Button>();

            Start_Btu.onClick.AddListener(GameStart);
        }
    }


    public void GameStart()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) return;

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerQueue();

            StartTurn();
        }
    }

    private void PlayerQueue()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false; // 방입장 못하게 막는거
        PhotonNetwork.CurrentRoom.IsVisible = false; // 방 안보이게 하는거

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            players_Q.Enqueue(player);
        }
    }

    [PunRPC]
    private void StartTurn()
    {
        if (PhotonNetwork.IsMasterClient && players_Q.Count > 0)
        {
            currentPlayer = players_Q.Dequeue();

            Debug.Log($"{currentPlayer.NickName}님의 턴입니다.");

            photonView.RPC("PlayerTurnGame", RpcTarget.All, currentPlayer.ActorNumber);

            StartCoroutine(StartPlayerTurn(30f));
        }
    }

    [PunRPC]
    private void PlayerTurnGame(int actorNumber)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber); // 현재 누구의 턴인 플레이어를 담아준다.

        // 턴인 플레이어는 팔레트와 그림그리기를 활성화하고 게임단어를 GameWord에 넣고 보이게한다.

        // 턴이 아닌유저는 채팅과 그림을 보는 곳을 활성화 게임단어를 동일하게 GameWord에 담고 안보이게한다음 동일한 단어 입력시 무언가 실행해야한다.
    }

    private IEnumerator StartPlayerTurn(float turnDuration)
    {
        yield return new WaitForSeconds(turnDuration);
        EndTurn();
    }

    private void EndTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            players_Q.Enqueue(currentPlayer); // 턴이 끝난 현재 플레이어를 다시 Queue에 저장

            photonView.RPC("StartTurn", RpcTarget.MasterClient); // 다음 턴을 시작 마스터 클라만 작동
        }
    }

}
