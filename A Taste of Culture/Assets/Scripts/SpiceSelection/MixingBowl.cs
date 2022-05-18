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
        if (spice == null) { return; }

        // Toggle reset button
        if (!firstSpiceAdded && other.gameObject.tag == "Spice")
        {
            firstSpiceAdded = true;
            if (resetButton != null) { resetButton.gameObject.SetActive(true); }
            if (doneButton != null) { doneButton.gameObject.SetActive(true); }
        }

        // Add to flavor profile
        BitternessValue += spice.Bitterness;
        SpicinessValue += spice.Spiciness;
        SweetnessValue += spice.Sweetness;

        Debug.Log($"{name}: Added spice \"{spice.name}\" with flavor profile " +
            $"(Bit: {BitternessValue}, Sp: {SpicinessValue} Sw: {SweetnessValue})");
    }

    public void ResetValues()
    {
        GameObject[] spices = GameObject.FindGameObjectsWithTag("Spice");

        foreach (GameObject spice in spices)
        {
            Destroy(spice);
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

        if (resetButton != null) { resetButton.gameObject.SetActive(false); }
        if (doneButton != null) { doneButton.gameObject.SetActive(false); }

        SceneController sceneController = FindObjectOfType<SceneController>();
        if (sceneController != null) { sceneController.TaskComplete(); }
    }
}
