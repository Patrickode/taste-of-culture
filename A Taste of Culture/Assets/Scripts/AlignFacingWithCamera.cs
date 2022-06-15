using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignFacingWithCamera : MonoBehaviour
{
    [SerializeField] private Camera _targetCam;
    [SerializeField] private bool invert;
    private Camera TargetCam
    {
        get
        {
            if (!_targetCam) _targetCam = Camera.main;
            return _targetCam;
        }
    }

    private void Update()
    {
        transform.forward = TargetCam.transform.forward * (invert ? -1 : 1);
    }
}
