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

            Debug.Log($"{currentPlayer.NickName}���� ���Դϴ�.");

            photonView.RPC("PlayerTurnGame", RpcTarget.All, currentPlayer.ActorNumber);

            StartCoroutine(StartPlayerTurn(30f));
        }
    }

    [PunRPC]
    private void PlayerTurnGame(int actorNumber)
    {
        // ��� Ŭ���̾�Ʈ���� ���� ���� �÷��̾ ã���ϴ�.
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        if (player != null)
        {
            Debug.Log($"It's {player.NickName}'s turn now!");
            // UI ������Ʈ �� �ʿ��� �۾��� ���⼭ ����
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
            // ���� �÷��̾ �ٽ� ť�� �ְ� ���� ���� �����մϴ�.
            players_Q.Enqueue(currentPlayer);
            photonView.RPC("StartNextTurn", RpcTarget.MasterClient); // ���� �� ����
        }
    }

}
