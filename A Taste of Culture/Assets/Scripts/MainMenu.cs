using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject Buttons;
    [SerializeField] private GameObject Credits;
    [SerializeField] private GameObject Options;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreen;

    [SerializeField] private AudioMixer mixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            sfxSlider.value = PlayerPrefs.GetFloat("SoundFXVolume");
        }

        Cursor.visible = true;
    }

    public void OptionsSwitch()
    {
        SavePrefs();
        Options.SetActive(!Options.activeSelf);
        Buttons.SetActive(!Buttons.activeSelf);
    }

    public void CreditsSwitch()
    {
        Credits.SetActive(!Credits.activeSelf);
        Buttons.SetActive(!Buttons.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadIndex(int index)
    {
        Transitions.LoadWithTransition?.Invoke(index, -1);
    }

    /// <summary>
    /// Takes in the master slider float to set the volume
    /// from the audiomixer group Master
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        //Debug.Log("Did things");
        mixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// Takes in the music slider float to set the volume
    /// from the audiomixer group Music
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    /// <summary>
    /// Takes in the FX slider float to set the volume
    /// from the audiomixer group Music
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SoundFX", Mathf.Log10(volume) * 20);
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundFXVolume", sfxSlider.value);
    }

}