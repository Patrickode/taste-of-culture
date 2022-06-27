using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStirIngredients : MonoBehaviour
{
    [SerializeField] private bool spawnOnStartup;
    [SerializeField] [Min(1)] private int numIngredients;
    [SerializeField] private Transform[] possibleIngredients;
    [Space(5)]
    [SerializeField] [Min(0)] private float separationRadius;
    [SerializeField] [Min(0)] private int maxFailedSeparations = 250;

    private void Start()
    {
        if (spawnOnStartup)
            Spawn();
    }

    public void Spawn()
    {
        if (possibleIngredients.Length < 1 || numIngredients < 0) return;

        float xRadius = transform.localScale.x / 2;
        float yRadius = transform.localScale.y / 2;

        Transform container = new GameObject("Stir Ingredients").transform;
        container.SetPositionAndRotation(transform.position, transform.rotation);

        //Init values for the upcoming loop; the spawn positions of the ingredients, and the number of
        //times such a position has been generated
        Vector3[] startPosns = new Vector3[numIngredients];
        startPosns[0] = Vector3.zero;
        int seprAttempts = 0;

        for (int i = 0; i < numIngredients; i++)
        {
            var spawnedIng = Instantiate(
                possibleIngredients[Random.Range(0, possibleIngredients.Length)],
                container);

            //Set a spawn pos in a random position within the radii.
            Vector3 spawnPos;
            do
            {
                spawnPos = Random.insideUnitCircle;
                spawnPos.x *= xRadius;
                spawnPos.y *= yRadius;
                seprAttempts++;
            }
            //If the separation radius is non-zero, AND we haven't used up all our seperation attempts,
            //AND the position we generated is too close to another, try again.
            while (separationRadius > 0 && seprAttempts <= maxFailedSeparations + numIngredients
                && System.Array.Exists(startPosns, (elem)
                    => (spawnPos - elem).sqrMagnitude <= separationRadius * separationRadius));

            spawnedIng.localPosition = spawnPos;
            startPosns[i] = spawnPos;
        }
    }
}
