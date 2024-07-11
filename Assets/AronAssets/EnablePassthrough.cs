using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class EnablePassthrough : MonoBehaviour
{
    [SerializeField] Camera mainCamera; 

    [SerializeField] float enablePassthroughAfter = 1.0f; 

    void Awake()
    {
        if (mainCamera == null) mainCamera = GetComponent<Camera>();

        if (mainCamera) {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0,0,0,0);
            StartCoroutine(TogglePassthrough(true));
        } else {
            Debug.LogError("No Camera Found!");
        }
    }
    
    IEnumerator TogglePassthrough(bool enable) {
        yield return new WaitForSeconds(enablePassthroughAfter);

        PXR_Boundary.EnableSeeThroughManual(enable);
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause) {
            PXR_Boundary.EnableSeeThroughManual(true);
            PXR_MixedReality.EnableVideoSeeThrough(true);
        }
    }
}
