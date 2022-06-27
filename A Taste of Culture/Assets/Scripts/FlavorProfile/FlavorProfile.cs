using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfile : MonoBehaviour
{
    [SerializeField] FlavorVisualizer flavorVisualizerPrefab;
    [SerializeField] TMPro.TextMeshProUGUI labelPrefab;
    [Space(5)]
    [SerializeField] Vector3 visualizerOffset;
    [SerializeField] [Range(1, 360)] float maxAngle = 360;
    [SerializeField] [Range(1f, 8f)] float maxRadius = 4f;
    [SerializeField] [Range(.01f, .5f)] float lineWidth = .05f;
    [SerializeField] [Range(.01f, .5f)] float lineSpacing = .05f;
    [SerializeField] [Min(0)] float gradualDisplaySpeed = 0.05f;
    [SerializeField] bool visualizeOnStart;
    [Space(10)]
    [SerializeField] Color bitternessColor;
    [SerializeField] Color spicinessColor;
    [SerializeField] Color sweetnessColor;
    [SerializeField] Color saltinessColor;

    private Dictionary<FlavorType, FlavorVisualizer> visualizers = new Dictionary<FlavorType, FlavorVisualizer>();
    private const string Separator = "<color=#00000000>X</color>";

    private void Start()
    {
        if (visualizeOnStart)
            VisualizeFlavors();
    }

    public void VisualizeFlavors()
    {
        FlavorProfileData fData = FlavorProfileData.Instance;
        int totalFlavors = fData.FlavorSum;
        var flavors = fData.FlavorDict;

        float iteratedRadius = maxRadius;

        foreach (var flavor in flavors)
        {
            float flavorFraction = flavor.Value > 0
                ? (float)flavor.Value / totalFlavors
                : 0.005f;
            int segments = Mathf.RoundToInt(maxAngle * flavorFraction);

            //Create a properly named container object with a rect transform and move it to the right spot.
            Transform container = new GameObject
            (
                Enum.GetName(typeof(FlavorType), flavor.Key),
                typeof(RectTransform)
            ).transform;

            //Then parent it to this for organization's sake.
            //  NOTE: "worldPositionStays" doesn't just affect position (at least for RectTransform objs). Passing false
            //  prevents the object from having a scale of ~100.
            container.position = transform.position + visualizerOffset;
            container.SetParent(transform, false);

            FlavorVisualizer visualizer = Instantiate(flavorVisualizerPrefab, container.position, container.rotation);
            visualizer.transform.parent = container;
            visualizer.name = "Visualizer";
            visualizers.Add(flavor.Key, visualizer);

            //Label text will be further positioned by the visualizer.
            visualizer.labelText = Instantiate(labelPrefab, container);
            visualizer.labelText.name = "Label";
            visualizer.DisplayFlavorValue(iteratedRadius, lineWidth, segments, GetFlavorColor(flavor.Key), gradualDisplaySpeed);

            int roundPercent = Mathf.RoundToInt(flavor.Value / Mathf.Max(totalFlavors, Mathf.Epsilon) * 100);

            //Text = "Name ##%". The separator (zero alpha) acts as a letter-width space, accounting for non-monospaced fonts.
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

        foreach (var flav in newFlavs)
        {
            visualizers[flav.Key].UpdateDisplay(flav.Value, gradualDisplaySpeed);

            int roundPercent = Mathf.RoundToInt(flav.Value / Mathf.Max(fData.FlavorSum, Mathf.Epsilon) * 100);

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