using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

namespace FMSolution.FMETP
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoToTextureEncoder : MonoBehaviour
    {
        public TextureEncoder textureEncoder;
        public RawImage rawImage; // Reference to the RawImage on the Canvas
        private VideoPlayer videoPlayer;
        private RenderTexture renderTexture;

        void Start()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.isLooping = true;
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            // Set the render texture size according to your video resolution
            renderTexture = new RenderTexture(1080, 720, 0); // Example resolution, adjust as necessary
            videoPlayer.targetTexture = renderTexture;

            // Assign the RenderTexture to the RawImage's texture
            if (rawImage != null)
            {
                rawImage.texture = renderTexture;
            }

            videoPlayer.prepareCompleted += PrepareCompleted;
            videoPlayer.Prepare();
        }

        private void PrepareCompleted(VideoPlayer source)
        {
            videoPlayer.Play();
            StartCoroutine(CaptureFrames());
        }

        private IEnumerator CaptureFrames()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                Texture2D frameTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                RenderTexture.active = renderTexture;
                frameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                frameTexture.Apply();
                RenderTexture.active = null;

                // Pass the frameTexture to the TextureEncoder for encoding
                textureEncoder.Action_StreamTexture(frameTexture);

                // Optionally, you can destroy the frameTexture after use to save memory
                Destroy(frameTexture);

                // Adjust the frame rate as needed (e.g., every frame, every other frame, etc.)
                yield return new WaitForSeconds(1.0f / (float)videoPlayer.frameRate);
            }
        }

        private void OnDestroy()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }
        }
    }
}
