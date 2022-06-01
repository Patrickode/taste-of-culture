using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfile : MonoBehaviour
{
    [SerializeField] [Range(1f, 8f)] float maxRadius = 4f;
    [SerializeField] [Range(.01f, .5f)] float lineWidth = .05f;
    [SerializeField] [Range(.01f, .5f)] float lineSpacing = .05f;

    [SerializeField] int bitterness;
    [SerializeField] int spiciness;
    [SerializeField] int sweetness;
    [SerializeField] int saltiness;

    [SerializeField] Color bitternessColor;
    [SerializeField] Color spicinessColor;
    [SerializeField] Color sweetnessColor;
    [SerializeField] Color saltinessColor;

    Dictionary<int, Color> flavors = new Dictionary<int, Color>();

    [SerializeField] GameObject flavorVisualizerPrefab;
    
    private void Start() 
    {
        flavors.Add(bitterness, bitternessColor);
        flavors.Add(spiciness, spicinessColor);
        flavors.Add(sweetness, sweetnessColor);
        flavors.Add(saltiness, saltinessColor);

        VisualizeFlavors();
    }

    private void VisualizeFlavors()
    {
        // TODO: Get flavor values from save...
        int totalFlavors = bitterness + spiciness + sweetness + saltiness;

        float radius = maxRadius;

        foreach(int flavor in flavors.Keys)
        {
            float flavorFraction = (float)flavor / (float)totalFlavors;
            int segments = Mathf.RoundToInt(360 * flavorFraction);

            Debug.Log("Flavor Value: " + flavor + " Total Flavor: " + totalFlavors + " Fraction: " + flavorFraction);
            Debug.Log("Segments: " + segments);

            GameObject flavorVisualizer = Instantiate(flavorVisualizerPrefab);
            flavorVisualizer.transform.parent = gameObject.transform;
            flavorVisualizer.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 0.05f);
            flavorVisualizer.transform.rotation = gameObject.transform.rotation;

            flavorVisualizer.GetComponent<FlavorVisualizer>().DrawCircle(radius, lineWidth, segments, flavors[flavor]);
        
            radius -= lineWidth + lineSpacing;
        }
    }
}
