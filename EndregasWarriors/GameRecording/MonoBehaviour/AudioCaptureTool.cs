using System.Runtime.InteropServices;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.EndregasWarriors.GameRecording
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioCaptureTool : UXTool
    {
        public static EventDelegate eventDelegate = new EventDelegate();
        private System.IntPtr libAPI;
        private System.IntPtr audioPointer;
        private System.Byte[] audioByteBuffer;
        

        protected override void Awake()
        {
            audioByteBuffer = new System.Byte[8192];
            GCHandle audioHandle = GCHandle.Alloc(audioByteBuffer, GCHandleType.Pinned);
            audioPointer = audioHandle.AddrOfPinnedObject();
            base.Awake();
        }

        protected override void Start()
        {
            libAPI = GameRecordingBrain._instance.audioLibAPI;
            eventDelegate.onReady?.Invoke("audio");
            base.Start();
        }
        
        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (GameRecordingBrain._instance.status == CaptureSettings.StatusType.STARTED)
            {
                Marshal.Copy(data, 0, audioPointer, 2048);
                AudioCaptureLib_WriteFrame(libAPI, audioByteBuffer);
            }
        }
        
        [DllImport("VideoCaptureLib")]
        static extern void AudioCaptureLib_WriteFrame(System.IntPtr api, byte[] data);
    }
}