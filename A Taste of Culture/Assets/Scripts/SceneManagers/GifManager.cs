using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GifManager : MonoBehaviour
{
    //[SerializeField] GameObject ingredient1Instruction;
    //[SerializeField] GameObject ingredient2Instruction;
    public CookingSceneManager sceneManager;
    public RawImage demoVideo;
    public Button closeButton;
    public VideoPlayer demoPlayer;

    bool hasBeenPlayed = false;

    void Start() 
    {
        //demoVideo = GameObject.Find("VideoTexture").GetComponent<RawImage>();
        //closeButton = GameObject.Find("CloseButton").GetComponent<Button>();

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(ButtonClicked);
        }

        //demoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        if (sceneManager.GetComponent<CookingSceneManager>().knife.name.Equals("Chopping Knife"))
        {
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Chopping.mp4");
        }
        else if(sceneManager.GetComponent<CookingSceneManager>().knife.name.Equals("Cutting Knife"))
        {
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Slicing Video.mp4");
        }
        else
        {
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Spice Picking.mp4");
        }

        demoPlayer.gameObject.SetActive(false);
        demoVideo.gameObject.SetActive(false);

        demoPlayer.loopPointReached += LoopedOnce;
    }

    // Start is called before the first frame update
    public void StartVideo()
    {
        // if(hasBeenPlayed) { return; }               // Fixes bug where slicing scene restarts video.

        if(demoVideo != null) 
        { 
            Cursor.visible = true;

            demoPlayer.gameObject.SetActive(true);
            demoVideo.gameObject.SetActive(true); 
        }

        // hasBeenPlayed = true;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (closeButton.gameObject.activeInHierarchy == false && demoVideo.gameObject.activeInHierarchy == true)
    //    {
    //        closeButton.gameObject.SetActive(true);
    //    }
    //}

    void ButtonClicked()
    {
        if(demoVideo == null) { return; }

        demoVideo.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        demoPlayer.gameObject.SetActive(false);
        sceneManager.GifEnded();
    }

    void LoopedOnce(VideoPlayer demoPlayer)
    {
        closeButton.gameObject.SetActive(true);
        //demoPlayer.gameObject.SetActive(false);
        //demoVideo.gameObject.SetActive(false);
    }
}
