using System;

using UnityEngine;
using UnityUXTesting.EndregasWarriors;
using UnityUXTesting.EndregasWarriors.Common;
using static UnityUXTesting.EndregasWarriors.Common.CaptureSettings;

namespace UnityUXTesting.EndregasWarriors.GameRecording
{
    [RequireComponent(typeof(Camera))]
    public class VideoCaptureTool : UXTool
    {
        #region Variables
        
        private Camera captureCamera;
       
        private float deltaFrameTime;
        private Texture2D frameTexture;
        private CameraRenderer _cameraRenderer;
        private VideoEncoder _videoEncoder;
        #endregion
           
        #region VideoCapture Core

        public void StartCapture()
        {
            // filePath = filePath + Utils.StringUtils.GetMp4FileName(Utils.StringUtils.GetRandomString(5));
            // //todo: Get camera and rest of variables
            // _cameraRenderer = new CameraRenderer(captureCamera, frameHeight, frameWidth, antiAliasing, targetFramerate,
            //     filePath);
            //
            // frameTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, false);
            // frameTexture.hideFlags = HideFlags.HideAndDontSave;
            // frameTexture.wrapMode = TextureWrapMode.Clamp;
            // frameTexture.filterMode = FilterMode.Trilinear;
            // frameTexture.hideFlags = HideFlags.HideAndDontSave;
            // frameTexture.anisoLevel = 0;
            // // Reset tempory variables.
            // capturingTime = 0f;
            // capturedFrameCount = 0;
            // encodedFrameCount = 0;
            
            // todo create VideoEncoder variable from brain and connect here
            
            // if (encodeThread != null)
            // {
            //     encodeThread.Abort();
            // }
            // // Start encoding thread.
            // encodeThread = new Thread(FrameEncodeThreadFunction);
            // encodeThread.Priority = System.Threading.ThreadPriority.Lowest;
            // encodeThread.IsBackground = true;
            // encodeThread.Start();
        }

        #endregion

        #region UnityLifeCycle
        private void Awake()
        {
            captureCamera = GetComponent<Camera>();
            // deltaFrameTime = 1f / targetFramerate;
           
            base.Awake();
        }

        #endregion


       
        
        
       
    }
}