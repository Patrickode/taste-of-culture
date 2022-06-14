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
    [SerializeField] float GradualDisplaySpeed = 0.05f;
    [Space(10)]
    [SerializeField] Color bitternessColor;
    [SerializeField] Color spicinessColor;
    [SerializeField] Color sweetnessColor;
    [SerializeField] Color saltinessColor;

    List<KeyValuePair<int, Color>> flavors = new List<KeyValuePair<int, Color>>();

    int bitterness;
    int spiciness;
    int sweetness;
    int saltiness;

    private void Start()
    {
        FlavorProfileData flavorData = FlavorProfileData.Instance;

        bitterness = flavorData.Bitterness;
        spiciness = flavorData.Spiciness;
        sweetness = flavorData.Sweetness;
        saltiness = flavorData.Saltiness;

        flavors.Add(new KeyValuePair<int, Color>(bitterness, bitternessColor));
        flavors.Add(new KeyValuePair<int, Color>(spiciness, spicinessColor));
        flavors.Add(new KeyValuePair<int, Color>(sweetness, sweetnessColor));
        flavors.Add(new KeyValuePair<int, Color>(saltiness, saltinessColor));
    }

    public void VisualizeFlavors()
    {
        int totalFlavors = bitterness + spiciness + sweetness + saltiness;

        float radius = maxRadius;

        foreach (KeyValuePair<int, Color> flavor in flavors)
        {
            float flavorFraction = flavor.Key > 0
                ? (float)flavor.Key / totalFlavors
                : 0.005f;
            int segments = Mathf.RoundToInt(maxAngle * flavorFraction);

            //Create a container object and move it to the right spot, then parent it to this for organization's sake.
            //  NOTE: "worldPositionStays" doesn't just affect position, at least for RectTransform objs. Passing false
            //  prevents the object from having a scale of ~100.
            Transform container = new GameObject(GetFlavorName(flavor), typeof(RectTransform)).transform;
            container.position = transform.position + visualizerOffset;
            container.SetParent(transform, false);

            FlavorVisualizer visualizer = Instantiate(flavorVisualizerPrefab, container.position, container.rotation);
            visualizer.transform.parent = container;
            visualizer.name = "Visualizer";

            //Label text will be further positioned by the visualizer.
            visualizer.labelText = Instantiate(labelPrefab, container);
            visualizer.labelText.name = "Label";
            visualizer.DisplayFlavorValue(radius, lineWidth, segments, flavor.Value, GradualDisplaySpeed);

            int roundPercent = Mathf.RoundToInt((float)flavor.Key / totalFlavors * 100);
            string separator = "<color=#00000000>X</color>";
            visualizer.labelText.text = $"{GetFlavorName(flavor)}{separator}" +
                $"{(roundPercent < 10 ? separator + roundPercent : roundPercent.ToString())}%";

            radius -= lineWidth + lineSpacing;
        }
    }

    string GetFlavorName(KeyValuePair<int, Color> flavor)
    {
        string flavorName = "Saltiness";

        if (flavor.Key == bitterness) { flavorName = "Bitterness"; }
        else if (flavor.Key == spiciness) { flavorName = "Spiciness"; }
        else if (flavor.Key == sweetness) { flavorName = "Sweetness"; }

        return flavorName;
    }
}
