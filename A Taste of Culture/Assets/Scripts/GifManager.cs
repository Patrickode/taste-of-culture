using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GifManager : MonoBehaviour
{
    [SerializeField] GameObject ingredient1Instruction;
    [SerializeField] GameObject ingredient2Instruction;
    
    GameObject demoVideo;
    GameObject button;

    VideoPlayer demoPlayer;
    Button closeButton;

    void Awake() 
    {
        demoVideo = gameObject.transform.GetChild(0).gameObject;
        closeButton = gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Button>();
        
        if(closeButton != null) 
        { 
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(ButtonClicked); 
        }

        if(demoVideo != null) { demoPlayer = demoVideo.GetComponent<VideoPlayer>(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(demoVideo != null) 
        { 
            Cursor.visible = true;

            demoVideo.SetActive(true); 
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

        demoVideo.SetActive(false);
        closeButton.gameObject.SetActive(false);
        
        Cursor.visible = false;

        // TODO: enable instruction gif
    }
}
