using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionTooltips : MonoBehaviour
{
    [SerializeField] float delay = 3f;

    GameObject movementControls;
    GameObject rotationControls;

    void Awake() 
    {
        movementControls = gameObject.transform.GetChild(0).gameObject;
        rotationControls = gameObject.transform.GetChild(1).gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        movementControls.SetActive(false);
        rotationControls.SetActive(false);
    }

    public void PrepToToggle()
    {
        StartCoroutine(ToggleMovementInstructions());
    }

    IEnumerator ToggleMovementInstructions()
    {
        yield return new WaitForSeconds(delay);
        movementControls.SetActive(true);
    }

    public void ToggleRotationInstructions()
    {
        movementControls.SetActive(false);
        rotationControls.SetActive(true);
    }

    public void ResetInstructions()
    {
        rotationControls.SetActive(false);

        StartCoroutine(ToggleMovementInstructions());
    }
}
