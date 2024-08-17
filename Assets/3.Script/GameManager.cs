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
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber); // 현재 누구의 턴인 플레이어를 담아준다..

        NetworkManager.instance.Game_Check = true;

        gametext.text = string.Empty;

        if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(actorNumber)) // 두개의 넘버가 동일하면 턴인 플레이어
        {
            GameWord = "테스트";
            gametext.text = GameWord; // 인게임에 보이게해할 텍스트
            InGameUI.SetActive(true);
            DrawingTool.SetActive(true); // 팔레트
            InputField.SetActive(false); // 인풋필드 닫기
        }
        else // 동일하지 않으면 현재 그림그리는 턴이 아닌 플레이어들
        {
            InGameUI.SetActive(true);
            DrawingTool.SetActive(false); // 팔레트
            InputField.SetActive(true); // 인풋필드 닫기
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
            players_Q.Enqueue(currentPlayer); // 턴이 끝난 현재 플레이어를 다시 Queue에 저장

            photonView.RPC("StartTurn", RpcTarget.MasterClient); // 다음 턴을 시작 마스터 클라만 작동
        }
    }

}
