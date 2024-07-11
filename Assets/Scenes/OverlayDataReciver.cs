using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using FMSolution.FMWebSocket;

public class OverlayDataReceiver : MonoBehaviour
{
    public RectTransform uiElement;    // Reference to the UI element's RectTransform
    public Canvas vrCanvas;             // Reference to the Canvas in VR
    public FMWebSocketManager webSocketManager; // Reference to the WebSocket manager if needed
    private Image uiElementImage;       // Reference to the UI element's Image component
    private Coroutine fadeCoroutine;    // Reference to the current fade coroutine

    void Start()
    {
        uiElementImage = uiElement.GetComponent<Image>();

        // Subscribe to the WebSocket manager's event for receiving UI position data
        webSocketManager.OnReceivedStringDataEvent.AddListener(OnReceiveUIPosition);
    }

    void OnReceiveUIPosition(string message)
    {
        // Deserialize the received JSON message into a ClickData object
        ClickData clickData = JsonUtility.FromJson<ClickData>(message);

        // Get the size of the VR Canvas
        Vector2 vrCanvasSize = vrCanvas.pixelRect.size;

        // Denormalize the position based on the VR Canvas size
        float adjustedX = (clickData.x * vrCanvasSize.x) - (vrCanvasSize.x / 2);
        float adjustedY = (clickData.y * vrCanvasSize.y) - (vrCanvasSize.y / 2);

        // Set the UI element's local position within the canvas
        uiElement.localPosition = new Vector2(adjustedX, adjustedY);

        // Change the UI element's opacity to 1 and restart the fade coroutine
        SetUIElementOpacity(1);
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOpacity());
    }

    void SetUIElementOpacity(float opacity)
    {
        Color color = uiElementImage.color;
        color.a = opacity;
        uiElementImage.color = color;
    }

    IEnumerator FadeOpacity()
    {
        float duration = 2f;
        float startOpacity = uiElementImage.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startOpacity, 0, time / duration);
            SetUIElementOpacity(newOpacity);
            yield return null;
        }

        SetUIElementOpacity(0);
    }

    [System.Serializable]
    public class ClickData
    {
        public float x;
        public float y;
    }
}
