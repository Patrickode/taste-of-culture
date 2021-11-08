using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GifManager : MonoBehaviour
{
    [SerializeField] GameObject ingredient1Instruction;
    [SerializeField] GameObject ingredient2Instruction;
    public CookingSceneManager sceneManager;
    
    RawImage demoVideo;

    VideoPlayer demoPlayer;
    Button closeButton;

    void Awake() 
    {
        demoVideo = GameObject.Find("VideoTexture").GetComponent<RawImage>();
        closeButton = GameObject.Find("CloseButton").GetComponent<Button>();

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(ButtonClicked);
        }

        demoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
        if (sceneManager.GetComponent<CookingSceneManager>().knife.name.Equals("Chopping Knife"))
        {
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Chopping Video.mp4");
        }
        else
        {
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Slicing Video.mp4");
        }

        demoPlayer.gameObject.SetActive(false);
        demoVideo.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    public void StartVideo()
    {
        if(demoVideo != null) 
        { 
            Cursor.visible = true;

            demoPlayer.gameObject.SetActive(true);
            demoVideo.gameObject.SetActive(true); 
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(demoPlayer != null) 
        {
            if(closeButton != null && closeButton.gameObject.activeInHierarchy == false && demoVideo.gameObject.activeInHierarchy == true) 
            {
                closeButton.gameObject.SetActive(true);
            }
        }
    }

    void ButtonClicked()
    {
        if(demoVideo == null) { return; }

        demoVideo.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        demoPlayer.gameObject.SetActive(false);
        sceneManager.GifEnded();
    }
}
