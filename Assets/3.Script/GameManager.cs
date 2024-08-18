using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private Button Start_Btu;
    [SerializeField] private GameObject DrawingTool;
    [SerializeField] private GameObject InGameUI;
    [SerializeField] private GameObject InputField;
    public Text Gametext { get; private set; }
    public Slider timeSlider { get; private set; }
    private Queue<Player> players_Q = new Queue<Player>();
    private Player currentPlayer;
    private string gameWord = string.Empty;
    private GameRoundSetting roundSetting;
    private Coroutine currentCoroutine;
    public Text point_txt; // 각 플레이어
    public int point;

    private void Start()
    {
        if (photonView.IsMine && PhotonNetwork.IsMasterClient)
        {
            GameObject Btu = GameObject.FindGameObjectWithTag("GameStart");
            Start_Btu = Btu.GetComponent<Button>();
            Start_Btu.gameObject.SetActive(true);
            Start_Btu.onClick.AddListener(GameStart);
        }

        DrawingTool = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("DrawingTool"));
        InGameUI = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("GameUI"));
        InputField = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("Input"));
        Gametext = InGameUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        timeSlider = InGameUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Slider>();
        roundSetting = FindObjectOfType<GameRoundSetting>();

        DrawingTool.SetActive(false);
        InGameUI.SetActive(false);

        Invoke("Setting", 0.7f);
    }
    private void Setting()
    {
        point = 0;
        GameObject parentObject = gameObject.transform.parent.gameObject;
        point_txt = parentObject.transform.GetChild(2).transform.GetComponent<Text>();
        point_txt.text = point.ToString();
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
        }
    }

    [PunRPC]
    private void PlayerTurnGame(int actorNumber)
    {
        NetworkManager.instance.Game_Check = true;

        Gametext.text = string.Empty;

        if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(actorNumber)) // 두개의 넘버가 동일하면 턴인 플레이어
        {
            currentCoroutine = StartCoroutine(StartPlayerTurn());
            gameWord = roundSetting.ChangeText(Gametext.text);
            SendWord(gameWord);
            Gametext.text = gameWord; // 인게임에 보이게 할 텍스트
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

    private IEnumerator StartPlayerTurn()
    {
        float elapsedTime = 0f;
        float duration = PhotonNetwork.CurrentRoom.PlayTime; ;  // 슬라이더가 100에서 0으로 감소하는 데 걸리는 시간
        float curValue = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;  // 경과 시간 증가
            curValue = Mathf.Lerp(1, 0, elapsedTime / duration);  // Lerp를 사용하여 값을 부드럽게 감소
            SendTimeSliderValue(curValue);
            yield return null;  // 다음 프레임까지 대기
        }

        curValue = 0;  // 최종적으로 슬라이더를 0으로 설정
        SendTimeSliderValue(curValue);

        EndTurn();
    }

    private void SendTimeSliderValue(float time)
    {
        timeSlider.value = time;

        // 이벤트 코드 설정
        byte eventCode = 4;

        // Photon을 통해 이벤트로 전송
        PhotonNetwork.RaiseEvent(eventCode, time, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    private void SendWord(string word)
    {
        // 이벤트 코드 설정
        byte eventCode = 5;

        // Photon을 통해 이벤트로 전송
        PhotonNetwork.RaiseEvent(eventCode, word, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    [PunRPC]
    public void EndTurn()
    {
        if (!photonView.IsMine) return;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        players_Q.Enqueue(currentPlayer); // 턴이 끝난 현재 플레이어를 다시 Queue에 저장
        photonView.RPC("StartTurn", RpcTarget.MasterClient); // 다음 턴을 시작 마스터 클라만 작동
    }

    public void OnEvent(EventData photonEvent)
    {
        // 이벤트 코드가 6인 경우에만 처리
        if (photonEvent.Code == 6)
        {
            if (photonView.IsMine)
            {
                string[] Word = (string[])photonEvent.CustomData;
                if (gameWord.Equals(Word[0]))
                {
                    photonView.RPC("EndTurn", RpcTarget.MasterClient); // 다음 턴을 시작 마스터 클라만 작동

                    int viewID = int.Parse(Word[1]);
                    photonView.RPC("AddPoint", RpcTarget.All, viewID);
                }
            }
        }
    }
    [PunRPC]
    private void AddPoint(int viewID)
    {
        PhotonView foundView = PhotonNetwork.GetPhotonView(viewID);

        if (foundView.IsMine)
        {
            Debug.Log(viewID);
            point += 10;
            point_txt.text = point.ToString();
        }
    }
}
