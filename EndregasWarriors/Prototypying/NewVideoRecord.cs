using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using RockVR.Video;
using UnityEditor;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.GameRecording;
using PathConfig = UnityUXTesting.EndregasWarriors.Common.PathConfig;

namespace UnityUXTesting.EndregasWarriors.Prototypying
{
    [RequireComponent(typeof(Camera))]
    public class NewVideoRecord : MonoBehaviour
    {
        public bool started;

        private string filePath;
        private Camera captureCamera;
        private RenderTexture frameRenderTexture;
        private bool isCreateRenderTexture;

        private int frameWidth = 1280;
        private int frameHeight = 720;
        private int antiAliasing = 4;
        private RenderTexture finalTargetTexture;
        private Texture2D frameTexture;
        private float capturingTime;
        private Queue<VideoEncoder.FrameData> frameQueue;
        private int capturedFrameCount;
        private int encodedFrameCount;
        private IntPtr libAPI;
        private Material blitMaterial;
        private Thread encodeThread;
        private bool isCapturingFrame;
        private float deltaFrameTime;
        private bool isFinished;

        private void Awake()
        {
            filePath = PathConfig.saveFolder + Utils.StringUtils.GetMp4FileName(Utils.StringUtils.GetRandomString(5));
            captureCamera = GetComponentInChildren<Camera>();

            if (captureCamera.targetTexture != null)
            {
                // Use binded rendertexture will ignore antiAliasing config.
                frameRenderTexture = captureCamera.targetTexture;
                isCreateRenderTexture = false;
            }
            else
            {
                // Create a rendertexture for video capture.
                // Size it according to the desired video frame size.
                frameRenderTexture = new RenderTexture(frameWidth, frameHeight, 24);
                frameRenderTexture.antiAliasing = antiAliasing;
                frameRenderTexture.wrapMode = TextureWrapMode.Clamp;
                frameRenderTexture.filterMode = FilterMode.Trilinear;
                frameRenderTexture.hideFlags = HideFlags.HideAndDontSave;
                // Make sure the rendertexture is created.
                frameRenderTexture.Create();
                isCreateRenderTexture = true;

                captureCamera.targetTexture = frameRenderTexture;
            }
            
            captureCamera.aspect = frameWidth / ((float) frameHeight);
            captureCamera.targetTexture = frameRenderTexture;

            finalTargetTexture = new RenderTexture(frameWidth, frameHeight, 24);
            finalTargetTexture.isPowerOfTwo = true;
            finalTargetTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            finalTargetTexture.useMipMap = false;
            finalTargetTexture.antiAliasing = antiAliasing;
            finalTargetTexture.wrapMode = TextureWrapMode.Clamp;
            finalTargetTexture.filterMode = FilterMode.Trilinear;
            finalTargetTexture.autoGenerateMips = false;

            frameTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, false);
            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.wrapMode = TextureWrapMode.Clamp;
            frameTexture.filterMode = FilterMode.Trilinear;
            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.anisoLevel = 0;

            capturingTime = 0f;
            capturedFrameCount = 0;
            encodedFrameCount = 0;
            frameQueue = new Queue<VideoEncoder.FrameData>();

            libAPI = VideoCaptureLib_Get(
                frameWidth,
                frameHeight,
                30,
                0,
                filePath,
                PathConfig.ffmpegPath);

            deltaFrameTime = 1f / 30;

            blitMaterial = new Material(Shader.Find("Hidden/BlitCopy"));
            blitMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        private void Start()
        {
            
#if !UNITY_EDITOR
        Application.wantsToQuit += WantsToQuit;
#else
            EditorApplication.playModeStateChanged += change => WantsToQuit();
#endif
            
            if (encodeThread != null)
            {
                encodeThread.Abort();
            }

            // Start encoding thread.
            started = true;
            encodeThread = new Thread(FrameEncodeThreadFunction);
            encodeThread.Priority = System.Threading.ThreadPriority.Lowest;
            encodeThread.IsBackground = true;
            encodeThread.Start();
        }

        private void LateUpdate()
        {
            if (started)
            {
                capturingTime += Time.deltaTime;
                if (!isCapturingFrame)
                {
                    int totalRequiredFrameCount =
                        (int) (capturingTime / deltaFrameTime);
                    // Skip frames if we already got enough.
                    if (totalRequiredFrameCount > capturedFrameCount)
                    {
                        StartCoroutine(CaptureFrameAsync());
                    }
                }
            }
        }
        
        private bool WantsToQuit()
        {
            
            while (!isFinished)
            {
                Thread.Sleep(1000);
            }
            encodeThread.Abort();
            VideoCaptureLib_Close(libAPI);
            VideoCaptureLib_Clean(libAPI);
            //todo: send Video
            return true;
        }

        private IEnumerator CaptureFrameAsync()
        {
            isCapturingFrame = true;

            yield return new WaitForEndOfFrame();
            CopyFrameTexture();
            EnqueueFrameTexture();

            isCapturingFrame = false;
        }

        private void CopyFrameTexture()
        {
            RenderTexture.active = frameRenderTexture;
            
            frameTexture.ReadPixels(new Rect(0, 0, frameWidth, frameHeight), 0, 0, false);
            frameTexture.Apply();
            // Restore RenderTexture states.
            RenderTexture.active = null;
        }
        
        void EnqueueFrameTexture()
        {
            int totalRequiredFrameCount = (int)(capturingTime / deltaFrameTime);
            int requiredFrameCount = totalRequiredFrameCount - capturedFrameCount;
            lock (this)
            {
                frameQueue.Enqueue(
                    new VideoEncoder.FrameData(frameTexture.GetRawTextureData(), requiredFrameCount));
            }
            capturedFrameCount = totalRequiredFrameCount;
        }

        private void FrameEncodeThreadFunction()
        {
           Debug.Log("Hallo"); 
            while (frameQueue.Count > 0 || started)
            {
                Debug.Log("Hallo1"); 
                if (frameQueue.Count > 0)
                {
                    isFinished = false;
                    Debug.Log("Hallo2"); 
                    VideoEncoder.FrameData frame;
                    lock (this)
                    {
                        frame = frameQueue.Dequeue();
                    }

                    VideoCaptureLib_WriteFrames(libAPI, frame.pixels, frame.count);
                    encodedFrameCount++;
                }
             
            }

            isFinished = true;
        }

        [DllImport("VideoCaptureLib")]
        static extern System.IntPtr VideoCaptureLib_Get(int width, int height, int rate, int proj, string path,
            string ffpath);


        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_WriteFrames(System.IntPtr api, byte[] data, int count);


        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_Close(System.IntPtr api);
        
        
        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_Clean(System.IntPtr api);
    }
}