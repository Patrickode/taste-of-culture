using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingBowl : MonoBehaviour
{
    public Button resetButton;
    public Button doneButton;

    // Flavor Profile values
    int BitternessValue;
    int SpicinessValue;
    int SweetnessValue;

    // Used to toggle reset button after first spice added to bowl
    bool firstSpiceAdded;                

    // Start is called before the first frame update
    void Start()
    {
        if (resetButton != null)
        {
            resetButton.gameObject.SetActive(false);
            resetButton.onClick.AddListener(ResetValues);
        }

        if (doneButton != null)
        {
            doneButton.gameObject.SetActive(false);
            doneButton.onClick.AddListener(FinishSelecting);
        }
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        Spice spice = other.gameObject.GetComponent<Spice>();
        if(spice == null) { return; }

        // Toggle reset button
        if(!firstSpiceAdded) 
        { 
            firstSpiceAdded = true; 
            resetButton.gameObject.SetActive(true);
            doneButton.gameObject.SetActive(true);
        }
        
        // Add to flavor profile
        BitternessValue += spice.Bitterness;
        SpicinessValue += spice.Spiciness;
        SweetnessValue += spice.Sweetness;

        Debug.Log("Bitterness: " + BitternessValue + " Spiciness: " + SpicinessValue + " Sweetness: " + SweetnessValue);
    }

    public void ResetValues()
    {
        Spice[] spices = FindObjectsOfType<Spice>();

        foreach(Spice spice in spices) 
        {
            Destroy(spice.gameObject);
        }

        resetButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);
        firstSpiceAdded = false;

        // Ensures cursor isn't visible after clicking button
        Cursor.visible = false;                         

        BitternessValue = 0;
        SpicinessValue = 0;
        SweetnessValue = 0;
    }

    public void FinishSelecting()
    {
        /// TODO: save flavor profile...

        SceneController sceneController = FindObjectOfType<SceneController>();
        if(sceneController != null) { sceneController.TaskComplete(); }
    }
}
