using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingBowl : MonoBehaviour
{
    [SerializeField] private bool canAddSpicesOnStart = true;
    [SerializeField] private bool onlyAddOnControlsActivated;
    [SerializeField] private GameObject[] hideUntilSpiceAdded;

    private HashSet<Spice> addedSpices = new HashSet<Spice>();

    public bool CanAddSpice { get; private set; }

    // Flavor Profile values
    int BitternessValue;
    int SpicinessValue;
    int SweetnessValue;
    int SaltinessValue;

    // Used to toggle reset button after first spice added to bowl
    bool firstSpiceAdded;

    private void OnEnable()
    {
        DialogueController.ToggleControls += OnControlsToggle;
    }
    private void OnDisable()
    {
        DialogueController.ToggleControls -= OnControlsToggle;
    }
    private void OnControlsToggle(bool active)
    {
        if (!onlyAddOnControlsActivated) return;
        CanAddSpice = active;
    }

    void Start()
    {
        CanAddSpice = canAddSpicesOnStart;
        ToggleAllActive(false, hideUntilSpiceAdded);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //No need to continue if we can't add spice, the collision wasn't with a spice, or
        //we've already added this spice
        if (!CanAddSpice || !other.gameObject.TryGetComponent(out Spice spice) || addedSpices.Contains(spice))
            return;

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

        addedSpices.Add(spice);

        Debug.Log($"<color=#888>{name}: Added spice \"{spice.name}\" with flavor profile " +
            $"(Bi: {spice.Bitterness}, Sp: {spice.Spiciness} " +
            $"Sw: {spice.Sweetness}, Sa: {spice.Saltiness}).</color>\n" +
            $"\t<color=#777>Total profile is now (Bi: {BitternessValue}, Sp: {SpicinessValue}, " +
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
            obj.SafeSetActive(active);
    }

    private void SaveFavorProfile()
    {
        FlavorProfileData flavorData = FlavorProfileData.Instance;

        flavorData.AddFlavors(
            (FlavorType.Bitterness, BitternessValue), (FlavorType.Spiciness, SpicinessValue),
            (FlavorType.Sweetness, SweetnessValue), (FlavorType.Saltiness, SaltinessValue));

        Debug.Log($"Saved flavor profile: (" +
            $"Bi: {flavorData[FlavorType.Bitterness]}, Sp: {flavorData[FlavorType.Spiciness]}, " +
            $"Sw: {flavorData[FlavorType.Sweetness]}, Sa: {flavorData[FlavorType.Saltiness]})");
    }
}
