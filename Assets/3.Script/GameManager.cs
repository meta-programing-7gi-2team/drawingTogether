using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button Start_Btu;
    [SerializeField] private GameObject DrawingTool;
    [SerializeField] private GameObject InGameUI;
    [SerializeField] private GameObject InputField;
    public Text gametext { get; private set; }
    private Queue<Player> players_Q = new Queue<Player>();
    private Player currentPlayer;
    private string GameWord = string.Empty;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject Btu = GameObject.FindGameObjectWithTag("GameStart");
            Start_Btu = Btu.GetComponent<Button>();
            Start_Btu.onClick.AddListener(GameStart);
        }

        DrawingTool = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("DrawingTool"));
        InGameUI = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("GameUI"));
        InputField = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("Input"));
        gametext = InGameUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();

        DrawingTool.SetActive(false);
        InGameUI.SetActive(false);
    }

    public void GameStart()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) return;

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerQueue();

            GameObject Btu = Start_Btu.gameObject;
            Btu.SetActive(false);

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
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber); // ���� ������ ���� �÷��̾ ����ش�..

        NetworkManager.instance.Game_Check = true;

        gametext.text = string.Empty;

        if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(actorNumber)) // �ΰ��� �ѹ��� �����ϸ� ���� �÷��̾�
        {
            GameWord = "�׽�Ʈ";
            gametext.text = GameWord; // �ΰ��ӿ� ���̰����� �ؽ�Ʈ
            InGameUI.SetActive(true);
            DrawingTool.SetActive(true); // �ȷ�Ʈ
            InputField.SetActive(false); // ��ǲ�ʵ� �ݱ�
        }
        else // �������� ������ ���� �׸��׸��� ���� �ƴ� �÷��̾��
        {
            InGameUI.SetActive(true);
            DrawingTool.SetActive(false); // �ȷ�Ʈ
            InputField.SetActive(true); // ��ǲ�ʵ� �ݱ�
        }
    }

    private IEnumerator StartPlayerTurn(float turnDuration)
    {
        yield return new WaitForSeconds(turnDuration);
        EndTurn();
    }

    public void EndTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            players_Q.Enqueue(currentPlayer); // ���� ���� ���� �÷��̾ �ٽ� Queue�� ����

            photonView.RPC("StartTurn", RpcTarget.MasterClient); // ���� ���� ���� ������ Ŭ�� �۵�
        }
    }

}
