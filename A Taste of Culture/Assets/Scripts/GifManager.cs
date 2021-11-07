using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GifManager : MonoBehaviour
{
    [SerializeField] GameObject ingredient1Instruction;
    [SerializeField] GameObject ingredient2Instruction;
    
    RawImage demoVideo;

    VideoPlayer demoPlayer;
    Button closeButton;

    void Start() 
    {
        demoVideo = GameObject.Find("VideoTexture").GetComponent<RawImage>();
        closeButton = GameObject.Find("CloseButton").GetComponent<Button>();

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(ButtonClicked);
        }

        demoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();

        demoPlayer.gameObject.SetActive(false);
        demoVideo.gameObject.SetActive(false);

        StartVideo();
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
    void Update()
    {
        if(demoPlayer != null) 
        {
            if((ulong)demoPlayer.frame == demoPlayer.frameCount - 1)
            {
                if(closeButton != null && closeButton.gameObject.activeInHierarchy == false) 
                {
                    closeButton.gameObject.SetActive(true); 
                }
            }
        }
    }

    void ButtonClicked()
    {
        if(demoVideo == null) { return; }

        demoVideo.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        demoPlayer.gameObject.SetActive(false);
    }
}
