using System;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;
using System.Runtime.InteropServices;

namespace UnityUXTesting.EndregasWarriors.GameRecording
{
    public class CameraRenderer
    {
        public Camera captureCamera;
        public float interPupillaryDistance = 0.0635f;


        protected RenderTexture stereoTargetTexture;
        protected RenderTexture frameRenderTexture;
        protected RenderTexture finalTargetTexture;

        protected Material stereoPackMaterial;
        protected Material copyReverseMaterial;
        protected Material blitMaterial;

        private IntPtr libAPI; 


        public CameraRenderer(Camera captureCamera, int frameHeight, int frameWidth, int antiAliasing, int targetFramerate, string filePath)
        {
            this.captureCamera = captureCamera;
            
            blitMaterial = new Material(Shader.Find("Hidden/BlitCopy"));
            blitMaterial.hideFlags = HideFlags.HideAndDontSave;
            
            frameRenderTexture = new RenderTexture(frameWidth, frameHeight, 24);
            frameRenderTexture.antiAliasing = antiAliasing;
            frameRenderTexture.wrapMode = TextureWrapMode.Clamp;
            frameRenderTexture.filterMode = FilterMode.Trilinear;
            frameRenderTexture.hideFlags = HideFlags.HideAndDontSave;
            // Make sure the rendertexture is created.
            frameRenderTexture.Create();
            
            // Init the final stereo texture.
            finalTargetTexture = new RenderTexture(frameWidth, frameHeight, 24);
            finalTargetTexture.isPowerOfTwo = true;
            finalTargetTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            finalTargetTexture.useMipMap = false;
            finalTargetTexture.antiAliasing = antiAliasing;
            finalTargetTexture.wrapMode = TextureWrapMode.Clamp;
            finalTargetTexture.filterMode = FilterMode.Trilinear;
            finalTargetTexture.autoGenerateMips = false;
            
            libAPI = VideoCaptureLib_Get(
                frameWidth,
                frameHeight,
                targetFramerate,
                0,
                filePath,
                PathConfig.ffmpegPath);
           
            if (libAPI == IntPtr.Zero)
            {
                Debug.LogWarning("[VideoCapture::StartCapture] Get native " +
                                 "capture api failed!");
                return;
            }
        }

        public void SetStereoVideoFormat(RenderTexture frameRenderTexture)
        {
            Vector3 cameraPosition = captureCamera.transform.localPosition;
            // Left eye
            captureCamera.transform.Translate(new Vector3(-interPupillaryDistance, 0, 0), Space.Self);
            RenderCameraToRenderTexture(captureCamera, frameRenderTexture, stereoTargetTexture);

            Graphics.Blit(stereoTargetTexture, finalTargetTexture, stereoPackMaterial);
            // Right eye
            captureCamera.transform.localPosition = cameraPosition;
            captureCamera.transform.Translate(new Vector3(interPupillaryDistance, 0f, 0f), Space.Self);
            RenderCameraToRenderTexture(captureCamera, frameRenderTexture, stereoTargetTexture);

            Graphics.Blit(stereoTargetTexture, finalTargetTexture, stereoPackMaterial);
            // Restore camera state
            captureCamera.transform.localPosition = cameraPosition;
        }

        private void RenderCameraToRenderTexture(Camera camera, RenderTexture frameRenderTexture, RenderTexture stereoTarget)
        {
            camera.CopyFrom(captureCamera);

            Graphics.SetRenderTarget(stereoTarget);
            Graphics.Blit(frameRenderTexture, blitMaterial);
            Graphics.SetRenderTarget(null);
        }
        
        
        
        #region Dll Import
        [DllImport("VideoCaptureLib")]
        static extern IntPtr VideoCaptureLib_Get(int width, int height, int rate, int proj, string path, string ffpath);

        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_WriteFrames(System.IntPtr api, byte[] data, int count);

        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_Close(System.IntPtr api);

        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_Clean(System.IntPtr api);

        [DllImport("VideoCaptureLib")]
        static extern System.IntPtr VideoStreamingLib_Get(int width, int height, int rate, int proj, string address, string ffpath);

        [DllImport("VideoCaptureLib")]
        static extern void VideoStreamingLib_WriteFrames(System.IntPtr api, byte[] data, int count);

        [DllImport("VideoCaptureLib")]
        static extern void VideoStreamingLib_Close(System.IntPtr api);

        [DllImport("VideoCaptureLib")]
        static extern void VideoStreamingLib_Clean(System.IntPtr api);
        #endregion // Dll Import
    }
    
    
  
}