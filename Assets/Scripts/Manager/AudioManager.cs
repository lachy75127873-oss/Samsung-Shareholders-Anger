using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioMixerGroupName
{
    Master,
    BGM,
    SFX
}

public class AudioManager : MonoBehaviour
{
    [Header("UserAudioMixers")]
    [SerializeField] AudioMixerGroup master;
    [SerializeField] AudioMixerGroup bgm;
    [SerializeField] AudioMixerGroup sfx;

    [Header("AuioSources")]
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource interfaceAudioSource;
    [SerializeField] AudioSource inGameAudioSource;

    [Header("BGM Clips")]
    [SerializeField] AudioClip lobbyBGM;
    [SerializeField] AudioClip inGameBGM;

    [Header("SFX Clips")]
    [SerializeField] AudioClip clickSFX;
    [SerializeField] AudioClip confirmSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip getItemSFX;

    private Dictionary<AudioMixerGroupName, AudioMixerGroup> UserAudios;

    public void Init()
    {
        bgmAudioSource = GetComponents<AudioSource>()[0];
        interfaceAudioSource = GetComponents<AudioSource>()[1];
        inGameAudioSource = GetComponents<AudioSource>()[2];

        UserAudios = new()
        {
            { AudioMixerGroupName.Master, master},
            { AudioMixerGroupName.BGM, bgm},
            { AudioMixerGroupName.SFX, sfx},
        };
    }

    public void Start()
    {
        foreach (var key in UserAudios.Keys)
        {
            var s_key = key.ToString();

            // 키가 있다면
            if (PlayerPrefs.HasKey(s_key))
            {
                var value = PlayerPrefs.GetFloat(s_key);
                // 키에 저장된 값 확인
                //Debug.Log($"PlayerPrefs.GetFloat({s_key}): {value}");
                // 저장된 값을 오디오 믹서에 세팅
                UserAudios[key].audioMixer.SetFloat(s_key, value);
                //UserAudios[key].audioMixer.GetFloat(s_key, out var current);
                // 저장 확인
                //Debug.Log($"UserAudios[{key}].audioMixer.GetFloat: {current}");
            }
        }
    }

    #region UserUIAdjust
    public void SetVolume(AudioMixerGroupName key, float value)
    {
        var s_key = key.ToString();

        if (UserAudios.ContainsKey(key))
        {
            UserAudios[key].audioMixer.SetFloat(s_key, value);
            PlayerPrefs.SetFloat(s_key, value);
        }
        else
        {
            Debug.Log($"Can't Find AudioMixerGroup: {key}");
        }
    }

    public float GetVolume(AudioMixerGroupName key)
    {
        var s_key = key.ToString();

        if (UserAudios[key].audioMixer.GetFloat(s_key, out var volume))
            return volume;
        else
        {
            Debug.Log($"Can't Find AudioMixerGroup: {key}");
            return 0f;
        }
    }

    #endregion

    #region BGM
    public void PlayTitleBGM()
        => PlayBGM(lobbyBGM);
    public void PlayInGameBGM()
        => PlayBGM(inGameBGM);

    private void PlayBGM(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return;
        if (bgmAudioSource.clip == clip) return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = volume;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    #endregion

    #region Interface SFX
    public void PlayClick()
        => interfaceAudioSource.PlayOneShot(clickSFX);
    public void PlayConfirm()
        => interfaceAudioSource.PlayOneShot(confirmSFX);

    #endregion

    #region InGame SFX
    public void PlayJump()
       => inGameAudioSource.PlayOneShot(jumpSFX);
    public void PlayGetItem()
        => inGameAudioSource.PlayOneShot(getItemSFX);
    #endregion
}
