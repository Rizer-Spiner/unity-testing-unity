using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.EndregasWarriors.GameRecording
{
    public class VideoEncoder
    {
        public struct FrameData
        {
            public byte[] pixels;
            public int count;

            public FrameData(byte[] p, int c)
            {
                pixels = p;
                count = c;
            }
        }

        private Queue<FrameData> frameQueue;
        private Thread encodeThread;
        private IntPtr videoLibAPI;

        public VideoEncoder(IntPtr videoLibAPI)
        {
            this.videoLibAPI = videoLibAPI;
            encodeThread = new Thread(FrameEncodeThreadFunction);
            encodeThread.Priority = ThreadPriority.Lowest;
            encodeThread.IsBackground = true;
            encodeThread.Start();
        }

        private void FrameEncodeThreadFunction()
        {
            while (GameRecordingBrain._instance.status == CaptureSettings.StatusType.STARTED || 
                   GameRecordingBrain._instance.status == CaptureSettings.StatusType.PAUSED ||
                   frameQueue.Count > 0)
            {
                if (frameQueue.Count > 0)
                {
                    FrameData frame;
                    lock (this)
                    {
                        frame = frameQueue.Dequeue();
                    }
                    VideoCaptureLib_WriteFrames(videoLibAPI, frame.pixels, frame.count);
                }
                else
                {
                    // Wait 1 second for captured frame.
                    Thread.Sleep(1000);
                }
            }
            
            VideoCaptureLib_Close(videoLibAPI);
        }

        public void EnqueueFrame(FrameData newFrame)
        {
            lock (this)
            {
                frameQueue.Enqueue(newFrame);
            }
        }


        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_WriteFrames(System.IntPtr api, byte[] data, int count);
        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_Close(System.IntPtr api);
    }
}