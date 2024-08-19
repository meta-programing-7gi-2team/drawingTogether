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
    [SerializeField] private PointReceiver pointReceiver;
    [SerializeField] private Text TurnNickname;
    public Text Gametext { get; private set; }
    public Slider timeSlider { get; private set; }
    private Player currentPlayer;
    private Queue<Player> players_Q = new Queue<Player>();
    private string gameWord = string.Empty;
    private GameRoundSetting roundSetting;
    private Coroutine currentCoroutine;

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
        TurnNickname = InGameUI.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>();
        timeSlider = InGameUI.transform.GetChild(0).transform.GetChild(2).GetComponent<Slider>();
        roundSetting = FindObjectOfType<GameRoundSetting>();
        pointReceiver = FindObjectOfType<PointReceiver>();

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
        if (PhotonNetwork.IsMasterClient && players_Q.Count > 0)// && !pointReceiver.InTrun)
        {
            pointReceiver.TrunStart();
            currentPlayer = players_Q.Dequeue();
            players_Q.Enqueue(currentPlayer);

            photonView.RPC("PlayerTurnGame", RpcTarget.All, currentPlayer.ActorNumber, currentPlayer.NickName);
        }
    }

    [PunRPC]
    private void PlayerTurnGame(int actorNumber, string NickName)
    {
        NetworkManager.instance.Game_Check = true;

        Gametext.text = string.Empty;
        TurnNickname.text = NickName;

        if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(actorNumber)) // �ΰ��� �ѹ��� �����ϸ� ���� �÷��̾�
        {
            currentCoroutine = StartCoroutine(StartPlayerTurn());
            gameWord = roundSetting.ChangeText(Gametext.text);
            SendWord(gameWord);
            Gametext.text = gameWord; // �ΰ��ӿ� ���̰� �� �ؽ�Ʈ
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

    private IEnumerator StartPlayerTurn()
    {
        float elapsedTime = 0f;
        float duration = (int)PhotonNetwork.CurrentRoom.CustomProperties["PlayTime"];  // �����̴��� 100���� 0���� �����ϴ� �� �ɸ��� �ð�
        float curValue = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;  // ��� �ð� ����
            curValue = Mathf.Lerp(1, 0, elapsedTime / duration);  // Lerp�� ����Ͽ� ���� �ε巴�� ����
            SendTimeSliderValue(curValue);
            yield return null;  // ���� �����ӱ��� ���
        }

        curValue = 0;  // ���������� �����̴��� 0���� ����
        SendTimeSliderValue(curValue);
        pointReceiver.TrunReset();
        EndTurn();
    }

    private void SendTimeSliderValue(float time)
    {
        timeSlider.value = time;

        // �̺�Ʈ �ڵ� ����
        byte eventCode = 4;

        // Photon�� ���� �̺�Ʈ�� ����
        PhotonNetwork.RaiseEvent(eventCode, time, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    private void SendWord(string word)
    {
        // �̺�Ʈ �ڵ� ����
        byte eventCode = 5;

        // Photon�� ���� �̺�Ʈ�� ����
        PhotonNetwork.RaiseEvent(eventCode, word, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    [PunRPC]
    public void EndTurn()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            TurnNickname.text = string.Empty;
        }
        photonView.RPC("StartTurn", RpcTarget.MasterClient); // ���� ���� ���� ������ Ŭ�� �۵�
    }

    public void OnEvent(EventData photonEvent)
    {
        // �̺�Ʈ �ڵ尡 5�� ��쿡�� ó��
        if (photonEvent.Code == 5)
        {
            gameWord = (string)photonEvent.CustomData;
        }
        // �̺�Ʈ �ڵ尡 6�� ��쿡�� ó��
        if (photonEvent.Code == 6)
        {
            string[] Word = (string[])photonEvent.CustomData;
            if (gameWord.Equals(Word[0]))
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                    currentCoroutine = null;
                }

                if (PhotonNetwork.IsMasterClient && photonView.IsMine)
                {
                    // �̺�Ʈ �ڵ� ����
                    byte eventCode = 7;

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                    // Photon�� ���� �̺�Ʈ�� ����
                    PhotonNetwork.RaiseEvent(eventCode, Word, raiseEventOptions, SendOptions.SendUnreliable);

                    photonView.RPC("EndTurn", RpcTarget.MasterClient); // ���� ���� ���� ������ Ŭ�� �۵�
                }
            }
        }
    }
}
