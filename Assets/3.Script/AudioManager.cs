using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;

    [Header("Audio Scurce")]
    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip Lobby_Background;
    public AudioClip InGame_Background;
    public AudioClip StartGame_SFX;
    public AudioClip Timer_SFX;
    public AudioClip BtnClick_SFX;
    public AudioClip Clear_SFX;
    public AudioClip CorrectAnswer_SFX;
    public AudioClip ResultWin_SFX;
    public AudioClip ResultLose_SFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        BGMSource.clip = Lobby_Background;
        BGMSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
