using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

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
        SetSfxVolume();

        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetBgmVolume();
            SetSfxVolume();
        }
    }

    private void SetBgmVolume()
    {
        float volume = bgmSlider.value;
        audiomixer.SetFloat("bgm", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("bgmVolume", volume);
    }

    private void SetSfxVolume()
    {
        float volume = sfxSlider.value;
        audiomixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetBgmVolume();
        SetSfxVolume();
    }
}
