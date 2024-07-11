using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasTest : MonoBehaviour

{
    private Canvas canvas;
    private RectTransform canvasRectTransform;

    void Awake()
    {
        // Ensure there's a Canvas component on the GameObject
        canvas = GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas component not found!");
            return;
        }

        // Get the RectTransform component
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        if (canvasRectTransform == null)
        {
            Debug.LogError("RectTransform component not found!");
            return;
        }
    }

    void Start()
    {
        // Configure the Canvas to stretch across the screen
        ConfigureCanvasToStretch();

        // Log the width and height of the Canvas
        LogCanvasSize();
    }

    void ConfigureCanvasToStretch()
    {
        // Set the anchor points to stretch
        canvasRectTransform.anchorMin = new Vector2(0, 0);
        canvasRectTransform.anchorMax = new Vector2(1, 1);

        // Set the pivot point to the center
        canvasRectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Reset the offset and position
        canvasRectTransform.offsetMin = Vector2.zero;
        canvasRectTransform.offsetMax = Vector2.zero;
        canvasRectTransform.localPosition = Vector3.zero;
    }

    void LogCanvasSize()
    {
        // Use the rect property to get the actual size
        Rect rect = canvasRectTransform.rect;
        Debug.Log("Canvas Width: " + rect.width + ", Canvas Height: " + rect.height);
    }
}

