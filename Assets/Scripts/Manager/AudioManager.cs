using UnityEngine;

public class AudioManager
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource sfxAudioSource;

    [Header("BGM Clips")]
    [SerializeField] float bgmVolume;
    [SerializeField] AudioClip LobbyBGM;
    [SerializeField] AudioClip InGameBGM;

    [Header("SFX Clips")]
    [SerializeField] AudioClip ClickSFX;
    [SerializeField] AudioClip ConfirmSFX;
    [SerializeField] AudioClip JumpSFX;
    [SerializeField] AudioClip GetItemSFX;


    #region BGM
    public void PlayLobbyBGM()
        => PlayBGM(LobbyBGM, bgmVolume);
    public void PlayInGameBGM()
        => PlayBGM(InGameBGM, bgmVolume);

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
        => sfxAudioSource.PlayOneShot(ClickSFX);
    public void PlayConfirm(float volume)
        => sfxAudioSource.PlayOneShot(ConfirmSFX);

    #endregion

    #region InGame SFX
    public void PlayJump()
        => sfxAudioSource.PlayOneShot(JumpSFX);
    public void PlayGetItem()
        => sfxAudioSource.PlayOneShot(GetItemSFX);

    #endregion
}
