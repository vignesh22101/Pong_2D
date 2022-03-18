using UnityEngine;

public enum Audios { Brick_Death, Ball_Hit, Powerup };

public class AudioPlayer : MonoBehaviour
{
    #region Variables
    internal static AudioPlayer instance;

    [SerializeField] private AudioSource audioSource_SFX, audioSource_Music;
    [SerializeField] private AudioData[] audioData;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    internal void PlayOneShot(Audios targetAudio)
    {
        AudioClip target_AudioClip = Get_AudioClip(targetAudio);

        if (target_AudioClip != null)
            audioSource_SFX.PlayOneShot(target_AudioClip);
    }

    internal void Change_SFX_State()
    {
        audioSource_SFX.mute = !audioSource_SFX.mute;
        PlayerPrefs.SetInt("SFX", audioSource_SFX.mute ? 0 : 1);
    }

    internal void Change_Music_State()
    {
        if (audioSource_Music.isPlaying)
            audioSource_Music.Pause();
        else
            audioSource_Music.UnPause();

        PlayerPrefs.SetInt("Music", audioSource_Music.isPlaying ? 1 : 0);
    }

    private AudioClip Get_AudioClip(Audios targetAudio)
    {
        foreach (var item in audioData)
        {
            if (item.audio == targetAudio)
                return item.audioClip;
        }

        return null;
    }
}

[System.Serializable]
public class AudioData
{
    public Audios audio;
    public AudioClip audioClip;
}
