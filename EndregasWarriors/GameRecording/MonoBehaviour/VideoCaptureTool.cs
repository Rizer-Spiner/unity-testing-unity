using System;
using System.Collections;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;
using static UnityUXTesting.EndregasWarriors.Common.CaptureSettings;

namespace UnityUXTesting.EndregasWarriors.GameRecording
{
    [RequireComponent(typeof(Camera))]
    public class VideoCaptureTool : UXTool
    {
        #region Variables

        private Camera captureCamera;
        private VideoEncoder _videoEncoder;

        private Texture2D frameTexture;
        private RenderTexture frameRenderTexture;

        private float deltaFrameTime;
        private bool isCreateRenderTexture;
        private float capturingTime = 0f;
        private bool isCapturingFrame = false;
        private int capturedFrameCount;

        public static EventDelegate eventDelegate = new EventDelegate();
        private Material blitMaterial;

        #endregion


        #region UnityLifeCycle

        protected override void Awake()
        {
            captureCamera = GetComponent<Camera>();
            blitMaterial = new Material(Shader.Find("Hidden/BlitCopy"));
            blitMaterial.hideFlags = HideFlags.HideAndDontSave;
            base.Awake();
        }

        protected override void Start()
        {
            deltaFrameTime = 1f / GameRecordingBrain._instance.GetFrameRate();

            if (captureCamera.targetTexture != null)
            {
                frameRenderTexture = captureCamera.targetTexture;
                isCreateRenderTexture = false;
            }
            else
            {
                frameRenderTexture = new RenderTexture(GameRecordingBrain._instance.GetFrameWidth(),
                    GameRecordingBrain._instance.GetFrameHeight(), 24);
                frameRenderTexture.antiAliasing = GameRecordingBrain._instance.GetAntiAliasing();
                frameRenderTexture.wrapMode = TextureWrapMode.Clamp;
                frameRenderTexture.filterMode = FilterMode.Trilinear;
                frameRenderTexture.hideFlags = HideFlags.HideAndDontSave;
                // Make sure the rendertexture is created.
                frameRenderTexture.Create();
                isCreateRenderTexture = true;
            }

            frameTexture = new Texture2D(GameRecordingBrain._instance.GetFrameWidth(),
                GameRecordingBrain._instance.GetFrameHeight(),
                TextureFormat.RGB24,
                false);

            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.wrapMode = TextureWrapMode.Clamp;
            frameTexture.filterMode = FilterMode.Trilinear;
            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.anisoLevel = 0;

            _videoEncoder = GameRecordingBrain._instance._encoder;
            eventDelegate.onReady?.Invoke("video");
            base.Start();
        }

        protected override void LateUpdate()
        {
            if (GameRecordingBrain._instance.status == StatusType.STARTED)
            {
                capturingTime += Time.deltaTime;
                if (!isCapturingFrame)
                {
                    int totalRequiredFrameCount =
                        (int) (capturingTime / deltaFrameTime);

                    if (totalRequiredFrameCount > capturedFrameCount)
                    {
                        StartCoroutine(CaptureFrameAsync());
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            if (frameRenderTexture != null && isCreateRenderTexture)
            {
                Destroy(frameRenderTexture);
            }
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest);// Render the screen.
            if (GameRecordingBrain._instance.status == StatusType.STARTED)
            {
                frameRenderTexture.DiscardContents();
                Graphics.SetRenderTarget(frameRenderTexture);
                Graphics.Blit(src, blitMaterial);
                Graphics.SetRenderTarget(null);
            }
         
        }

        #endregion


        private IEnumerator CaptureFrameAsync()
        {
            isCapturingFrame = true;
            if (GameRecordingBrain._instance.status == StatusType.STARTED)
            {
                yield return new WaitForEndOfFrame();
                CopyFrameTexture();
                EnqueueFrameTexture();
            }
            isCapturingFrame = false;
        }

        private void CopyFrameTexture()
        {
            frameTexture.ReadPixels(new Rect(0, 0,
                Screen.width,
                Screen.height), 0, 0, false);

            frameTexture.Apply();
            // Restore RenderTexture states.
            RenderTexture.active = null;
        }

        void EnqueueFrameTexture()
        {
            int totalRequiredFrameCount = (int) (capturingTime / deltaFrameTime);
            int requiredFrameCount = totalRequiredFrameCount - capturedFrameCount;

            _videoEncoder.EnqueueFrame(
                new VideoEncoder.FrameData(frameTexture.GetRawTextureData(), requiredFrameCount));
            capturedFrameCount = totalRequiredFrameCount;
        }
    }
}