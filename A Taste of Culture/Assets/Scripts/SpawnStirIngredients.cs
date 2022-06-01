using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStirIngredients : MonoBehaviour
{
    [SerializeField] [Min(0)] private int numIngredients;
    [SerializeField] private Transform[] possibleIngredients;

    private void Start()
    {
        if (possibleIngredients.Length < 1 || numIngredients < 0) return;

        float xRadius = transform.localScale.x / 2;
        float yRadius = transform.localScale.y / 2;
        for (int i = 0; i < numIngredients; i++)
        {
            var spawnedIng = Instantiate(possibleIngredients[Random.Range(0, possibleIngredients.Length)]);

            Vector3 spawnPos = Vector3.zero;
            while (spawnPos.normalized == Vector3.zero)
                spawnPos = Random.insideUnitCircle;

            spawnPos.x *= xRadius;
            spawnPos.y *= yRadius;
            spawnedIng.position = transform.position + spawnPos;
        }
    }
}
