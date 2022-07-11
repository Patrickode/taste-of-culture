using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaladSeasoner : MonoBehaviour
{
    [SerializeField] private SpriteRenderer idleSpriteObj;
    [SerializeField] private SpriteRenderer heldSpriteObj;
    [SerializeField] private SpriteRenderer useSpriteObj;
    [SerializeField] private float useSpriteTime;
    [Space(5)]
    [SerializeField] private MoveWithMousePos heldManager;
    [SerializeField] private SaladSeasoning seasonPrefab;
    [SerializeField] private Collider2D seasonZone;
    [SerializeField] private Transform seasonContainer;
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField] [VectorLabels(0.5f, 5, "Min", "Max")] private Vector2 spawnRotRange;

    private SpriteRenderer currentSpriteObj;
    private SpriteRenderer newSpriteObj;
    private Coroutine useTimer;

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        spawnRotRange.x = Mathf.Min(spawnRotRange.x, spawnRotRange.y);
        spawnRotRange.y = Mathf.Max(spawnRotRange.y, spawnRotRange.x);
    });

    private void Start()
    {
        UtilFunctions.SafeSetActive(heldSpriteObj, false);
        UtilFunctions.SafeSetActive(useSpriteObj, false);
        SwitchToSpriteObj(SeasonerState.Idle);
    }

    private void Update()
    {
        if (!heldManager.Held)
        {
            SwitchToSpriteObj(SeasonerState.Idle);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Vector3 targetPos = transform.position + (Vector3)spawnOffset;

            if (seasonZone.OverlapPoint(targetPos))
            {
                var spawnedSeasoning = Instantiate(
                    seasonPrefab,
                    transform.position + (Vector3)spawnOffset,
                    transform.rotation * Quaternion.AngleAxis(
                        Random.Range(spawnRotRange.x, spawnRotRange.y), Vector3.forward)
                    );
                spawnedSeasoning.transform.parent = seasonContainer;

                useTimer = Coroutilities.DoAfterDelay(this, () => useTimer = null, 0.1f);
                SwitchToSpriteObj(SeasonerState.Use);
            }
        }

        if (useTimer == null)
            SwitchToSpriteObj(SeasonerState.Held);
    }

    enum SeasonerState { Idle, Held, Use }
    private void SwitchToSpriteObj(SeasonerState state)
    {
        newSpriteObj = state switch
        {
            SeasonerState.Held => heldSpriteObj,
            SeasonerState.Use => useSpriteObj,
            _ => idleSpriteObj,
        };

        if (!newSpriteObj || currentSpriteObj == newSpriteObj) return;

        UtilFunctions.SafeSetActive(currentSpriteObj, false);
        newSpriteObj.gameObject.SetActive(true);
        currentSpriteObj = newSpriteObj;
    }
}