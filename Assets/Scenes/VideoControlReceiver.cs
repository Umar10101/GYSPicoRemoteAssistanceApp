using UnityEngine;
using UnityEngine.UI;
using FMSolution.FMWebSocket;
using UnityEngine.Video;

public class VideoControlReceiver : MonoBehaviour
{
    public RectTransform videoElement;    // Reference to the video element's RectTransform
    public Canvas vrCanvas;               // Reference to the Canvas in VR
    public FMWebSocketManager webSocketManager; // Reference to the WebSocket manager
    public VideoPlayer videoPlayer;       // Reference to the VideoPlayer component

    private RawImage videoImage;          // Reference to the video element's RawImage component

    void Awake()
    {
        videoImage = videoElement.GetComponent<RawImage>();

        // Set transparency to 0
        Color color = videoImage.color;
        color.a = 0;
        videoImage.color = color;

        // Pause the video on Awake
        videoPlayer.Pause();
    }

    void Start()
    {
        webSocketManager.OnReceivedStringDataEvent.AddListener(OnReceiveVideoControlData);
    }

    void OnReceiveVideoControlData(string message)
    {
        VideoControlData controlData = JsonUtility.FromJson<VideoControlData>(message);

        // Set the video time to synchronize both videos
        videoPlayer.time = controlData.videoTime;

        // Toggle play/pause
        if (controlData.isPlaying)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
        }

        // Adjust transparency
        Color color = videoImage.color;
        color.a = controlData.transparency;
        videoImage.color = color;
    }

    [System.Serializable]
    public class VideoControlData
    {
        public bool isPlaying;
        public float transparency;
        public double videoTime;
    }
}
