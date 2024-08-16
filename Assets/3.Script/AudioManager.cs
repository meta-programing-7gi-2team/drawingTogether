using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    [Header("Audio Scurce")]
    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Audio Clip Bgm")]
    public AudioClip Login_BGM;
    public AudioClip Lobby_BGM;
    public AudioClip InGame_BGM;

    [Header("Audio Clip Sfx")]
    public AudioClip StartGame_SFX;
    public AudioClip Timer_SFX;
    public AudioClip Click_SFX;
    public AudioClip CursorOn_SFX;
    public AudioClip Clear_SFX;
    public AudioClip CorrectAnswer_SFX;
    public AudioClip ResultWin_SFX;
    public AudioClip ResultLose_SFX;

    [Header("Audio Setting")]
    [SerializeField] private AudioMixer audiomixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;


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
        BGMSource.clip = Login_BGM;
        BGMSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    public void SetVolumeBgm()
    {
        BGMSource.volume = bgmSlider.value;
    }
    public void SetVolumeSfx()
    {
        SFXSource.volume = sfxSlider.value;
    }
}
