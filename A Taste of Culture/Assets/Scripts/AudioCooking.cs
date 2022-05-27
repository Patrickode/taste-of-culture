using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioCooking : MonoBehaviour
{
    [SerializeField] private AudioSource sizzle1;
    [SerializeField] private AudioSource sizzle2;
    [SerializeField] private AudioSource sizzleCombined;

    [SerializeField] private AudioSource lowBoil;
    [SerializeField] private AudioSource highBoil;

    [SerializeField] private AudioMixer mixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume")) * 20);
            mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
            mixer.SetFloat("SoundFX", Mathf.Log10(PlayerPrefs.GetFloat("SoundFXVolume")) * 20);
        }
    }

    public void AddOnions()
    {
        sizzle1.Play();
    }

    public void SauteOnions()
    {
        sizzle2.Play();
    }

    public void AddTomatos()
    {
        StartCoroutine(AudioFade.FadeOut(sizzle1, 0.5f));
        StartCoroutine(AudioFade.FadeOut(sizzle2, 0.75f));

        highBoil.Play();
        StartCoroutine(AudioFade.FadeIn(highBoil, 1.5f));
    }

    public void Simmer()
    {
        StartCoroutine(AudioFade.FadeOut(highBoil, 1.5f));

        lowBoil.Play();
        StartCoroutine(AudioFade.FadeDown(lowBoil, 5.0f, 0.25f));
    }

    public void StopBoil()
    {
        StartCoroutine(AudioFade.FadeOut(lowBoil, 0.75f));
    }
}

// FadeOut Method taken from https://forum.unity.com/threads/fade-out-audio-source.335031/
// Special thanks to that!!
public static class AudioFade
{

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public static IEnumerator FadeDown(AudioSource audioSource, float FadeTime, float TargetVolume)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > TargetVolume)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

    }

    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        while (audioSource.volume < 1)
        {
            audioSource.volume += 1.0f * Time.deltaTime / FadeTime;

            yield return null;
        }
    }

}
