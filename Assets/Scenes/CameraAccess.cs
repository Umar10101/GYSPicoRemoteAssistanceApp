using UnityEngine;
using TMPro;
using Unity.XR.PXR;
using FMSolution.FMETP;
using Unity.XR.PICO.TOBSupport;

public class CameraAccess : MonoBehaviour
{
    public TextureEncoder textureEncoder; // Reference to the TextureEncoder component
    public int videoWidth = 2328;
    public int videoHeight = 1748;
    public int frameRate = 24;
    // public TMP_Text debugText; // Reference to the TMP component in the Inspector

    private float currentTime = 0.0f;
    private Texture2D texture2D;

    private bool isCameraOpened = false;
    private bool isEnterpriseServiceBound = false;

    private void Start()
    {
        InitEnterpriseService();
        // PXR_MixedReality.EnableVideoSeeThrough(true);
        // PXR_Boundary.EnableSeeThroughManual(true);
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            // PXR_MixedReality.EnableVideoSeeThrough(true);
            // PXR_Boundary.EnableSeeThroughManual(true);
        }
    }

    private void InitEnterpriseService()
    {
        PXR_Enterprise.InitEnterpriseService();
        BindEnterpriseService((success) =>
        {
            if (success)
            {
                isEnterpriseServiceBound = true;
                // LogToTMP("Enterprise service bind success");
                TryOpenCamera();
            }
            else
            {
                // LogToTMP("Enterprise service bind failed");
            }
        });
    }

    private void BindEnterpriseService(System.Action<bool> callback)
    {
        PXR_Enterprise.BindEnterpriseService((success) =>
        {
            // LogToTMP("Enterprise service bind callback: " + success.ToString());
            callback(success);
        });
    }

    private void TryOpenCamera()
    {
        if (isEnterpriseServiceBound)
        {
            isCameraOpened = PXR_Enterprise.OpenVSTCamera();
            if (isCameraOpened)
            {
                // LogToTMP("VST camera opened successfully");
            }
            else
            {
                // LogToTMP("Failed to open VST camera");
            }
        }
        else
        {
            // LogToTMP("Cannot open camera: Enterprise service not bound yet");
        }
    }

    private void AcquireFrames()
    {
        Frame frame;
        int result = PXR_Enterprise.AcquireVSTCameraFrameAntiDistortion(videoWidth, videoHeight, out frame);

        if (result == 0)
        {
            if (texture2D == null)
            {
                texture2D = new Texture2D(videoWidth, videoHeight, TextureFormat.RGB24, false, false);
            }

            texture2D.LoadRawTextureData(frame.data, (int)frame.datasize);

            // Flip the texture vertically
            Color[] pixels = texture2D.GetPixels();
            Color[] flippedPixels = FlipTextureVertically(pixels, videoWidth, videoHeight);
            texture2D.SetPixels(flippedPixels);
            texture2D.Apply();

            // LogToTMP("Frame acquired successfully");

            // Pass the acquired frame to the TextureEncoder for encoding
            if (textureEncoder != null)
            {
                textureEncoder.Action_StreamTexture(texture2D);
                // LogToTMP("Frame Encoded Successfully");
            }
        }
        else
        {
            // LogToTMP("Failed to acquire frame. Result code: " + result);
        }
    }

    private Color[] FlipTextureVertically(Color[] original, int width, int height)
    {
        Color[] flipped = new Color[original.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                flipped[(height - 1 - y) * width + x] = original[y * width + x];
            }
        }
        return flipped;
    }

    private void Update()
    {
        if (isCameraOpened)
        {
            AcquireFrames();

            currentTime += Time.deltaTime;
            if (currentTime >= 1.0f / frameRate)
            {
                currentTime = 0.0f;
                // LogToTMP(currentTime + " Frame Acquired successfully");
            }
        }
    }

    // private void LogToTMP(string message)
    // {
    //     if (debugText != null)
    //     {
    //         debugText.text += message + "\n";
    //     }
    //     Debug.Log(message);
    // }
}
