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
        PhotonNetwork.CurrentRoom.IsOpen = false; // ������ ���ϰ� ���°�
        PhotonNetwork.CurrentRoom.IsVisible = false; // �� �Ⱥ��̰� �ϴ°�

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

            Debug.Log($"{currentPlayer.NickName}���� ���Դϴ�.");

            photonView.RPC("PlayerTurnGame", RpcTarget.All, currentPlayer.ActorNumber);

            StartCoroutine(StartPlayerTurn(30f));
        }
    }

    [PunRPC]
    private void PlayerTurnGame(int actorNumber)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber); // ���� ������ ���� �÷��̾ ����ش�.

        // ���� �÷��̾�� �ȷ�Ʈ�� �׸��׸��⸦ Ȱ��ȭ�ϰ� ���Ӵܾ GameWord�� �ְ� ���̰��Ѵ�.

        // ���� �ƴ������� ä�ð� �׸��� ���� ���� Ȱ��ȭ ���Ӵܾ �����ϰ� GameWord�� ��� �Ⱥ��̰��Ѵ��� ������ �ܾ� �Է½� ���� �����ؾ��Ѵ�.
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
            players_Q.Enqueue(currentPlayer); // ���� ���� ���� �÷��̾ �ٽ� Queue�� ����

            photonView.RPC("StartTurn", RpcTarget.MasterClient); // ���� ���� ���� ������ Ŭ�� �۵�
        }
    }

}
