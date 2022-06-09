using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingBowl : MonoBehaviour
{
    [SerializeField] private GameObject[] hideUntilSpiceAdded;

    // Flavor Profile values
    int BitternessValue;
    int SpicinessValue;
    int SweetnessValue;
    int SaltinessValue;

    // Used to toggle reset button after first spice added to bowl
    bool firstSpiceAdded;

    // Start is called before the first frame update
    void Start()
    {
        ToggleAllActive(false, hideUntilSpiceAdded);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Spice spice = other.gameObject.GetComponent<Spice>();
        if (spice == null) { return; }

        // Toggle reset button
        if (!firstSpiceAdded && other.gameObject.CompareTag("Spice"))
        {
            firstSpiceAdded = true;
            ToggleAllActive(true, hideUntilSpiceAdded);
        }

        // Add to flavor profile
        BitternessValue += spice.Bitterness;
        SpicinessValue += spice.Spiciness;
        SweetnessValue += spice.Sweetness;
        SaltinessValue += spice.Saltiness;

        Debug.Log($"<color=#888>{name}: Added spice \"{spice.name}\" with flavor profile " +
            $"(Bit: {spice.Bitterness}, Sp: {spice.Spiciness} " +
            $"Sw: {spice.Sweetness}, Sa: {spice.Saltiness}).</color>\n" +
            $"\t<color=#777>Total profile is now (Bit: {BitternessValue}, Sp: {SpicinessValue}, " +
            $"Sw: {SweetnessValue}, Sa: {SaltinessValue})</color>");
    }

    /// <summary>
    /// Called with <see cref="resetButton"/>'s OnClick via the inspector
    /// </summary>
    public void ResetValues()
    {
        GameObject[] spices = GameObject.FindGameObjectsWithTag("Spice");

        foreach (GameObject spice in spices)
            if (spice)
                Destroy(spice);

        ToggleAllActive(false, hideUntilSpiceAdded);
        firstSpiceAdded = false;

        // Ensures cursor isn't visible after clicking button
        Cursor.visible = false;

        BitternessValue = 0;
        SpicinessValue = 0;
        SweetnessValue = 0;
        SaltinessValue = 0;
    }

    /// <summary>
    /// Called with <see cref="doneButton"/>'s OnClick via the inspector
    /// </summary>
    public void FinishSelecting()
    {
        SaveFavorProfile();

        SpiceBowl.CanDisplayTooltip = false;
        ToggleAllActive(false, hideUntilSpiceAdded);

        SceneController sceneController = FindObjectOfType<SceneController>();
        if (sceneController)
            sceneController.TaskComplete();
    }

    private void ToggleAllActive(bool active, params GameObject[] objsToToggle)
    {
        foreach (var obj in objsToToggle)
            if (obj)
                obj.SetActive(active);
    }

    private void SaveFavorProfile()
    {              
        FlavorProfileData flavorData = FlavorProfileData.Instance;

        flavorData.Bitterness = BitternessValue;
        flavorData.Spiciness = SpicinessValue;
        flavorData.Sweetness = SweetnessValue;
        flavorData.Saltiness = SaltinessValue;

        Debug.Log("Saved flavor profile: " + $"(Bit: {flavorData.Bitterness}, Sp: {flavorData.Spiciness}, " +
            $"Sw: {flavorData.Sweetness}, Sa: {flavorData.Saltiness})");
    }
} 
