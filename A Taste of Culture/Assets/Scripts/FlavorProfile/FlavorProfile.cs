using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfile : MonoBehaviour
{
    [SerializeField] FlavorVisualizer flavorVisualizerPrefab;
    [SerializeField] TMPro.TextMeshProUGUI labelPrefab;
    [Space(10)]
    [SerializeField] Vector3 visualizerOffset;
    [SerializeField] [Range(1, 360)] float maxAngle = 360;
    [SerializeField] [Range(1f, 8f)] float maxRadius = 4f;
    [SerializeField] [Range(.01f, .5f)] float lineWidth = .05f;
    [SerializeField] [Range(.01f, .5f)] float lineSpacing = .05f;
    [Tooltip("How long it'll take for the visualizers to fill, in seconds.")]
    [SerializeField] [Min(0)] float displayDuration = 1;
    [Tooltip("The minimum number of segments to add/remove per second. Overrides displayDuration.")]
    [SerializeField] [Min(0)] float minSpeed = 0;
    [SerializeField] bool visualizeOnStart;
    [Space(5)]
    [SerializeField] Color bitternessColor;
    [SerializeField] Color spicinessColor;
    [SerializeField] Color sweetnessColor;
    [SerializeField] Color saltinessColor;

    private Dictionary<FlavorType, FlavorVisualizer> visualizers
        = new Dictionary<FlavorType, FlavorVisualizer>();
    private const string Separator = "<color=#00000000>X</color>";

    private void Start()
    {
        if (visualizeOnStart)
            VisualizeFlavors();
    }

    public void VisualizeFlavors()
    {
        FlavorProfileData fData = FlavorProfileData.Instance;
        var flavors = fData.FlavorDict;
        int totalFlavors = fData.FlavorSum;

        float iteratedRadius = maxRadius;

        foreach (var flavor in flavors)
        {
            //Create a properly named container object with a rect transform and move it to the right spot.
            Transform container = new GameObject
            (
                Enum.GetName(typeof(FlavorType), flavor.Key),
                typeof(RectTransform)
            ).transform;

            //Then parent it to this for organization's sake.
            //  NOTE: "worldPositionStays" doesn't just affect position (at least for RectTransform objs).
            //  Passing false prevents the object from having a scale of ~100.
            container.position = transform.position + visualizerOffset;
            container.SetParent(transform, false);

            FlavorVisualizer visualizer = Instantiate(
                flavorVisualizerPrefab,
                container.position,
                container.rotation);
            visualizer.transform.parent = container;
            visualizer.name = "Visualizer";

            visualizer.labelText = Instantiate(labelPrefab, container);
            visualizer.labelText.name = "Label";
            visualizer.minimumSpeed = minSpeed;
            visualizers.Add(flavor.Key, visualizer);

            //We ensure total flavors is at least some exceedingly small non-zero val to prevent division by zero
            float flavPercent = flavor.Value / Mathf.Max(totalFlavors, Mathf.Epsilon);

            //Label text will be further positioned by the visualizer.
            int angleSegments = Mathf.RoundToInt(maxAngle * flavPercent);
            visualizer.DisplayFlavorValue(
                iteratedRadius, lineWidth,
                Mathf.RoundToInt(maxAngle * flavPercent),
                GetFlavorColor(flavor.Key),
                displayDuration);

            int roundPercent = Mathf.RoundToInt(flavPercent * 100);

            //Text = "Name ##%". The separator (zero alpha) acts as a letter-width space, accounting for
            //non-monospaced fonts.
            visualizer.labelText.text = $"{Enum.GetName(typeof(FlavorType), flavor.Key)}{Separator}" +
                $"{(roundPercent < 10 ? Separator + roundPercent : roundPercent.ToString())}%";

            iteratedRadius -= lineWidth + lineSpacing;
        }
    }

    public void UpdateFlavors()
    {
        if (visualizers.Count < 1)
        {
            VisualizeFlavors();
            return;
        }

        FlavorProfileData fData = FlavorProfileData.Instance;
        var newFlavs = fData.FlavorDict;
        int flavSum = fData.FlavorSum;

        foreach (var flav in newFlavs)
        {
            float flavPercent = flav.Value / Mathf.Max(flavSum, Mathf.Epsilon);

            visualizers[flav.Key].UpdateDisplay(Mathf.RoundToInt(maxAngle * flavPercent), displayDuration);

            int roundPercent = Mathf.RoundToInt(flavPercent * 100);
            visualizers[flav.Key].labelText.text = $"{Enum.GetName(typeof(FlavorType), flav.Key)}{Separator}" +
                $"{(roundPercent < 10 ? Separator + roundPercent : roundPercent.ToString())}%";
        }
    }

    private Color GetFlavorColor(FlavorType flavType) => flavType switch
    {
        FlavorType.Bitterness => bitternessColor,
        FlavorType.Spiciness => spicinessColor,
        FlavorType.Sweetness => sweetnessColor,
        _ => saltinessColor,
    };
}