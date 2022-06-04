using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfile : MonoBehaviour
{
    [SerializeField] [Range(1f, 8f)] float maxRadius = 4f;
    [SerializeField] [Range(.01f, .5f)] float lineWidth = .05f;
    [SerializeField] [Range(.01f, .5f)] float lineSpacing = .05f;
    [SerializeField] float GradualDisplaySpeed = 0.05f;

    [SerializeField] Color bitternessColor;
    [SerializeField] Color spicinessColor;
    [SerializeField] Color sweetnessColor;
    [SerializeField] Color saltinessColor;

    [SerializeField] GameObject flavorVisualizerPrefab;

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

        VisualizeFlavors();
    }

    private void VisualizeFlavors()
    {
        int totalFlavors = bitterness + spiciness + sweetness + saltiness;

        float radius = maxRadius;

        foreach(KeyValuePair<int, Color> flavor in flavors)
        {
            if(flavor.Key == 0) { continue; }

            float flavorFraction = (float)flavor.Key / (float)totalFlavors;
            int segments = Mathf.RoundToInt(360 * flavorFraction);

            Vector3 position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.25f, gameObject.transform.position.z - 0.05f);;
            GameObject flavorVisualizer = Instantiate(flavorVisualizerPrefab, position, gameObject.transform.rotation);
            flavorVisualizer.transform.parent = gameObject.transform;

            FlavorVisualizer visualizer = flavorVisualizer.GetComponent<FlavorVisualizer>();
            visualizer.DisplayFlavorValue(radius, lineWidth, segments, flavor.Value, GradualDisplaySpeed);
            visualizer.labelText.text = GetFlavorName(flavor) + " " + Mathf.RoundToInt(flavorFraction * 100) + "%";

            radius -= lineWidth + lineSpacing;
        }
    }

    string GetFlavorName(KeyValuePair<int, Color> flavor)
    {
        string flavorName = "Saltiness";

        if(flavor.Key == bitterness) { flavorName = "Bitterness"; }
        else if(flavor.Key == spiciness) { flavorName = "Spiciness"; }
        else if(flavor.Key == sweetness) { flavorName = "Sweetness"; }

        return flavorName;
    }
}
