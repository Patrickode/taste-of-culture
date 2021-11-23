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

    void Start() 
    {
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(false);
            closeButton.onClick.AddListener(ButtonClicked);
        }

        if (sceneManager.GetComponent<CookingSceneManager>().knife.name.Equals("Chopping Knife"))
        {
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Chopping.mp4");
        }
        else
        {
            demoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Slicing Video.mp4");
        }

        demoPlayer.gameObject.SetActive(false);
        demoVideo.gameObject.SetActive(false);

        demoPlayer.loopPointReached += LoopedOnce;
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

    void ButtonClicked()
    {
        if(demoVideo == null) { return; }

        demoVideo.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        demoPlayer.gameObject.SetActive(false);
        sceneManager.GifEnded();

        // Start timer for toggling instruction tooltips
        InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
        if(tooltips != null) { tooltips.PrepToToggle(); }
    }

    void LoopedOnce(VideoPlayer demoPlayer)
    {
        closeButton.gameObject.SetActive(true);
    }
}
