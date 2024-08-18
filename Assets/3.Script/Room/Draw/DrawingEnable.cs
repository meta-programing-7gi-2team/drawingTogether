using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingEnable : MonoBehaviour
{
    [SerializeField] private Drawable drawable;
    private void OnEnable()
    {
        drawable.ResetCanvas();
    }
}
