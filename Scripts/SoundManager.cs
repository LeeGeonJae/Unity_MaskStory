using UnityEngine;
using UnityEngine.Audio;

public enum SoundGroupType
{
    Master,
    BGM,
    SFX
}

public class SoundManager : MonoBehaviour
{
    [Header("AudioMixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup bgmGroup;
    public AudioMixerGroup sfxGroup;

    public void SoundVolumControl(SoundGroupType soundGroupType, float volum)
    {
        switch (soundGroupType)
        {
            case SoundGroupType.Master:
                audioMixer.SetFloat("Mater", volum);
                break;
            case SoundGroupType.BGM:
                audioMixer.SetFloat("BGM", volum);
                break;
            case SoundGroupType.SFX:
                audioMixer.SetFloat("SFX", volum);
                break;
        }
    }
}
