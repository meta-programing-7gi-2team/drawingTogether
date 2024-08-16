using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button Start_Btu;
    private Queue<Player> players_Q = new Queue<Player>();
    private Player currentPlayer;

    private void Start()
    {
       GameObject Btu = GameObject.FindGameObjectWithTag("GameStart");

        Start_Btu = Btu.GetComponent<Button>();

        Start_Btu.onClick.AddListener(GameStart);
    }


    public void GameStart()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) return;

        if (PhotonNetwork.IsMasterClient)
        {
            InitializePlayerQueue();
            StartNextTurn();
        }
    }

    private void InitializePlayerQueue()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            players_Q.Enqueue(player);
        }
    }

    private void StartNextTurn()
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
        // 모든 클라이언트에서 현재 턴인 플레이어를 찾습니다.
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        if (player != null)
        {
            Debug.Log($"It's {player.NickName}'s turn now!");
            // UI 업데이트 등 필요한 작업을 여기서 수행
        }
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
            // 현재 플레이어를 다시 큐에 넣고 다음 턴을 시작합니다.
            players_Q.Enqueue(currentPlayer);
            photonView.RPC("StartNextTurn", RpcTarget.MasterClient); // 다음 턴 시작
        }
    }

}
