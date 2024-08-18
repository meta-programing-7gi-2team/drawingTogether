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
    public Text point_txt; // �� �÷��̾�
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
        }
    }

    [PunRPC]
    private void PlayerTurnGame(int actorNumber)
    {
        NetworkManager.instance.Game_Check = true;

        Gametext.text = string.Empty;

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
        float duration = PhotonNetwork.CurrentRoom.PlayTime; ;  // �����̴��� 100���� 0���� �����ϴ� �� �ɸ��� �ð�
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
        if (!photonView.IsMine) return;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        players_Q.Enqueue(currentPlayer); // ���� ���� ���� �÷��̾ �ٽ� Queue�� ����
        photonView.RPC("StartTurn", RpcTarget.MasterClient); // ���� ���� ���� ������ Ŭ�� �۵�
    }

    public void OnEvent(EventData photonEvent)
    {
        // �̺�Ʈ �ڵ尡 6�� ��쿡�� ó��
        if (photonEvent.Code == 6)
        {
            if (photonView.IsMine)
            {
                string[] Word = (string[])photonEvent.CustomData;
                if (gameWord.Equals(Word[0]))
                {
                    photonView.RPC("EndTurn", RpcTarget.MasterClient); // ���� ���� ���� ������ Ŭ�� �۵�

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
