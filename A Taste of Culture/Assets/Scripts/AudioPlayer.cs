using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioMode
{
    Sequential,
    SingleLoop,
    Randomized
}

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource bgSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private List<AudioClip> clips;
    private int clipIndex = 0;

    [SerializeField] AudioMode mode;

    [SerializeField] private AudioClip bgClip;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool loopBackground;
    [SerializeField] private bool loopMusic;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
            mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
            mixer.SetFloat("SoundFX", Mathf.Log10(PlayerPrefs.GetFloat("SoundFXVolume")) * 20);
        }

        bgSource.clip = bgClip;
        bgSource.loop = loopBackground;
        bgSource.Play();

        //musicSource.clip = musicClip;
        //musicSource.loop = loopMusic;
        //musicSource.Play();
    }

    public void TriggerSFX()
    {
        switch (mode)
        {
            case AudioMode.SingleLoop:
                source.clip = clips[0];
                source.loop = true;
                break;

            case AudioMode.Sequential:
                source.clip = clips[clipIndex];
                clipIndex = (clipIndex + 1) % clips.Count;
#if UNITY_EDITOR
                Debug.Log($"<color=#88D>Played sequential audio, new/next index is {clipIndex}</color>");
#endif

                break;

            case AudioMode.Randomized:
                int roll = Random.Range(0, clips.Count);
                source.clip = clips[roll];
#if UNITY_EDITOR
                Debug.Log($"<color=#88D>Played randomized audio; rolled index was {roll}</color>");
#endif
                break;
        }

        source.Play();
        //StartCoroutine("StartSound");
    }
}
