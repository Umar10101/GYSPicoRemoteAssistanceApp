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

    private float targetHeightPercentage = 1.0f; // 72.5% of the total height
    private float targetWidthPercentage = 1.0f;   // 58% of the total width

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

        // Fixed receiver canvas size
        Vector2 receiverCanvasSize = new Vector2(4320, 2160);

        // Calculate the specific area based on the given percentages
        float targetHeight = receiverCanvasSize.y * targetHeightPercentage;
        float targetWidth = receiverCanvasSize.x * targetWidthPercentage;

        // Calculate the offset from the edges to position the area
        float offsetY = (receiverCanvasSize.y - targetHeight) / 2;
        float offsetX = (receiverCanvasSize.x - targetWidth) / 2;

        // Denormalize the position based on the adjusted receiver canvas size
        float adjustedX = clickData.x * targetWidth - (receiverCanvasSize.x / 2 - offsetX);
        float adjustedY = clickData.y * targetHeight - (receiverCanvasSize.y / 2 - offsetY);

        // Set the UI element's anchored position within the canvas
        uiElement.anchoredPosition = new Vector2(adjustedX, adjustedY);

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
        public float canvasWidth;
        public float canvasHeight;
    }
}
