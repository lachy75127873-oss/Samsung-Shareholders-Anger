using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    [Header("UserAudioMixers")]
    [SerializeField] AudioMixer master;
    [SerializeField] AudioMixer bgm;
    [SerializeField] AudioMixer sfx;

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

    public void Init()
    {
        bgmAudioSource = GetComponents<AudioSource>()[0];
        interfaceAudioSource = GetComponents<AudioSource>()[1];
        inGameAudioSource = GetComponents<AudioSource>()[2];
    }

    #region UserUIAdjust
    public void SetMasterVolume(float volume)
        => master.SetFloat("Master", volume);
    public void SetBGMVolume(float volume)
    => bgm.SetFloat("BGM", volume);
    public void SetSFXVolume(float volume)
    => sfx.SetFloat("SFX", volume);

    public float GetMasterVolume()
    {
        if(master.GetFloat("Master", out var value))
            return value;
        else
            return 0f;
    }
    public float GetBGMVolume()
    {
        if (bgm.GetFloat("BGM", out var value))
            return value;
        else
            return 0f;
    }
    public float GetSFXVolume()
    {
        if (sfx.GetFloat("SFX", out var value))
            return value;
        else
            return 0f;
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
