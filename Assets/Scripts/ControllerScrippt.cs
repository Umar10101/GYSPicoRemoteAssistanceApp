using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ControllerScript : MonoBehaviour
{
    public GameObject micEncoder;
    public GameObject audioDecoder;
    public Image micIndicator; // UI Image component to hold the mic indicator PNG
    public float blinkSpeed = 1.0f; // Speed of the blinking effect

    private InputDevice leftController;
    private InputDevice rightController;
    private Coroutine blinkCoroutine;

    void Start()
    {
        // Initialize controllers
        InitializeControllers();
    }

    void InitializeControllers()
    {
        // Get all XR input devices
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);

        // Find left and right controllers
        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                leftController = device;
            }
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                rightController = device;
            }
        }
    }

    void Update()
    {
        // Check if controllers are valid, if not, reinitialize them
        if (!leftController.isValid || !rightController.isValid)
        {
            InitializeControllers();
        }

        // Check for A button press on the right controller to toggle micEncoder
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButtonPressed) && aButtonPressed)
        {
            if (micEncoder != null)
            {
                micEncoder.SetActive(!micEncoder.activeSelf);
                Debug.Log($"Mic Encoder {(micEncoder.activeSelf ? "Enabled" : "Disabled")}");

                // Start or stop the blinking effect based on the micEncoder's active state
                if (micEncoder.activeSelf)
                {
                    if (blinkCoroutine == null)
                    {
                        blinkCoroutine = StartCoroutine(BlinkMicIndicator());
                    }
                }
                else
                {
                    if (blinkCoroutine != null)
                    {
                        StopCoroutine(blinkCoroutine);
                        blinkCoroutine = null;
                        SetMicIndicatorAlpha(1.0f); // Ensure the indicator is fully visible when stopping
                    }
                }
            }
        }

        // Check for B button press on the right controller to toggle audioDecoder
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bButtonPressed) && bButtonPressed)
        {
            if (audioDecoder != null)
            {
                audioDecoder.SetActive(!audioDecoder.activeSelf);
                Debug.Log($"Audio Decoder {(audioDecoder.activeSelf ? "Enabled" : "Disabled")}");
            }
        }
    }

    private IEnumerator BlinkMicIndicator()
    {
        float alpha = 1.0f;
        bool fadingOut = true;

        while (true)
        {
            if (fadingOut)
            {
                alpha -= Time.deltaTime * blinkSpeed;
                if (alpha <= 0.0f)
                {
                    alpha = 0.0f;
                    fadingOut = false;
                }
            }
            else
            {
                alpha += Time.deltaTime * blinkSpeed;
                if (alpha >= 1.0f)
                {
                    alpha = 1.0f;
                    fadingOut = true;
                }
            }

            SetMicIndicatorAlpha(alpha);
            yield return null;
        }
    }

    private void SetMicIndicatorAlpha(float alpha)
    {
        if (micIndicator != null)
        {
            Color color = micIndicator.color;
            color.a = alpha;
            micIndicator.color = color;
        }
    }
}
