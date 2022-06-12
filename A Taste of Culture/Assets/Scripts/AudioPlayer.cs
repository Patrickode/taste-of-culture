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
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private List<AudioClip> clips;
    private int clipIndex = 0;

    [SerializeField] AudioMode mode;


    public void Play()
    {
        switch (mode)
        {
            case AudioMode.SingleLoop:
                source.clip = clips[0];
                source.loop = true;
                break;

            case AudioMode.Sequential:
                source.clip = clips[clipIndex];
                clipIndex = clipIndex <= clips.Count - 1 ? 0 : clipIndex + 1;
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

        StartCoroutine("StartSound");
    }

    private void StartSound()
    {
        source.Play();
    }
}
