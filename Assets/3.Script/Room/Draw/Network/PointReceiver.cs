using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PointReceiver : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private Text[] pointList_text = new Text[8];
    private int[] pointList = new int[8];

    private int[] sendPoint = new int[2];
    public bool InTrun { get; private set; }
    
    public void TrunStart()
    {
        InTrun = true;
    }
    public void TrunReset()
    {
        InTrun = false;
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

        for (int i = 0; i < 8; i++)
        {
            pointList[i] = 0;
        }
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        // 이벤트 코드가 7인 경우에만 처리
        if (photonEvent.Code == 7)
        {
            string[] word = (string[])photonEvent.CustomData;
            int idx = int.Parse(word[1]) - 1;
            Debug.Log(idx);
            pointList[idx] += 10;

            for (int i = 0; i < 8; i++)
            {
                if(pointList[i] != 0)
                {
                    pointList_text[i].text = pointList[i].ToString();
                }
            }
            InTrun = false;
        }
    }

}
