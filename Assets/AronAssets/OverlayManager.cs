using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public GameObject OverlayUI;

    // Start is called before the first frame update
    void Start()
    {
        OverlayUI.SetActive(false);
    }

    public void DataReceivedToggle()
    {
        OverlayUI.SetActive(!OverlayUI.activeSelf);
    }
}
