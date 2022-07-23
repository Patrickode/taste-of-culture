using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GIFManager : MonoBehaviour
{
    public bool startActive;
    public RawImage demoVideo;
    public Button closeButton;
    public VideoPlayer demoPlayer;
    public string videoFilename;

    //bool hasBeenPlayed = false;

    void Start()
    {
        if (closeButton != null)
            closeButton.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(videoFilename))
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFilename);
        demoPlayer.gameObject.SetActive(false);
        demoVideo.gameObject.SetActive(false);
        demoVideo.color = Color.clear;

#if !UNITY_EDITOR
        demoPlayer.loopPointReached += LoopedOnce; 
#endif
        //if (startActive)
        //    Coroutilities.DoAfterDelay(this, StartVideo, 1.5f);
    }

    public void StartVideo()
    {
        // if(hasBeenPlayed) { return; }               // Fixes bug where slicing scene restarts video.

        if (demoVideo != null)
        {
            Cursor.visible = true;

            demoPlayer.gameObject.SetActive(true);
            demoVideo.gameObject.SetActive(true);
            //TODO(?): Move this to before the dialogue is removed from the screen after continue is clicked, to
            //prevent flickering even better
            Coroutilities.DoWhen(this,
                () =>
                {
                    demoVideo.color = Color.white;
#if UNITY_EDITOR
                    closeButton.gameObject.SetActive(true);
#endif
                },
                () => demoPlayer.isPrepared);
        }

        // hasBeenPlayed = true;
    }

    public void CloseManager()
    {
        if (demoVideo == null) { return; }

        demoVideo.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        demoPlayer.gameObject.SetActive(false);

        // Start timer for toggling instruction tooltips
        InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
        if (tooltips != null) { tooltips.PrepToToggle(); }
    }

    void LoopedOnce(VideoPlayer demoPlayer)
    {
        closeButton.gameObject.SetActive(true);
        //demoPlayer.gameObject.SetActive(false);
        //demoVideo.gameObject.SetActive(false);
    }
}
