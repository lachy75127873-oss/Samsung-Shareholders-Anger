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
            { AudioMixerGroupName.BGM, master},
            { AudioMixerGroupName.SFX, master},
        };
    }

    #region UserUIAdjust
    public void SetVolume(AudioMixerGroupName name, float value)
    {
        UserAudios[name].audioMixer.SetFloat(name.ToString(), value);
    }

    public float GetVolume(AudioMixerGroupName name)
    {
        if (UserAudios[name].audioMixer.GetFloat(nameof(name), out var volume))
            return volume;
        else
        {
            Debug.Log($"Can't Find AudioMixerGroup: {name}");
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
