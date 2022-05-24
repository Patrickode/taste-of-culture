using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfile : MonoBehaviour
{
    // [SerializeField] GameObject spiceBreakdown;
    // [SerializeField] GameObject flavorBreakdown;
    
    // [SerializeField] bool displaySpices = true;
    // [SerializeField] bool displayFlavors = true;

    [SerializeField] int bitterness;
    [SerializeField] int spiciness;
    [SerializeField] int sweetness;
    [SerializeField] int saltiness;

    int[] flavors = new int[4];
    FlavorVisualizer[] flavorVisualizers = new FlavorVisualizer[4];

    [SerializeField] GameObject flavorVisualizerPrefab;
    
    private void Start() 
    {
        // if(displaySpices && spiceBreakdown != null) { spiceBreakdown.SetActive(true); }
        // else { spiceBreakdown.SetActive(false); } 

        // if(displayFlavors && flavorBreakdown != null) { flavorBreakdown.SetActive(true); }
        // else { flavorBreakdown.SetActive(false); } 

        flavors[0] = bitterness;
        flavors[1] = spiciness;
        flavors[2] = sweetness;
        flavors[3] = saltiness;

        VisualizeFlavors();
    }

    private void VisualizeFlavors()
    {
        // TODO: Get flavor values from save...
        int totalFlavors = bitterness + spiciness + sweetness + saltiness;

        foreach(int flavor in flavors)
        {
            float flavorFraction = flavor / totalFlavors;
            int segments = Mathf.RoundToInt(360 * flavorFraction);

            Debug.Log("Flavor Value: " + flavor + " Total Flavor: " + totalFlavors + " Fraction: " + flavorFraction);
            // Debug.Log("Segments: " + segments);

            GameObject flavorVisualizer = Instantiate(flavorVisualizerPrefab, gameObject.transform);
            // flavorVisualizer.GetComponent<FlavorVisualizer>().DrawCircle(segments);
        }

    }
}
