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
        private bool finishFlag = false;

        public VideoEncoder(IntPtr videoLibAPI)
        {
            this.videoLibAPI = videoLibAPI;
            frameQueue = new Queue<FrameData>();
            encodeThread = new Thread(FrameEncodeThreadFunction);
            encodeThread.Priority = ThreadPriority.Lowest;
            encodeThread.IsBackground = true;
            encodeThread.Start();
        }

        private void FrameEncodeThreadFunction()
        {
            while (true)
            {
                while (GameRecordingBrain._instance.status == CaptureSettings.StatusType.STARTED || 
                       GameRecordingBrain._instance.status == CaptureSettings.StatusType.PAUSED )
                {
                    finishFlag = false;
                    if (frameQueue.Count > 0)
                    {
                        EncodeFrame();
                    }
                    else
                    {
                        // Wait 1 second for captured frame.S
                        Thread.Sleep(1000);
                    }
                }

                while (GameRecordingBrain._instance.status == CaptureSettings.StatusType.STOPPED && frameQueue.Count > 0)
                {
                    finishFlag = false;
                    EncodeFrame();
                }

                if (GameRecordingBrain._instance.status != CaptureSettings.StatusType.NOT_START)
                {
                    finishFlag = true;
                }
            }
        }
        
        private void EncodeFrame()
        {
            FrameData frame;
            lock (frameQueue) {
                frame = frameQueue.Dequeue();
            }
            VideoCaptureLib_WriteFrames(videoLibAPI, frame.pixels, frame.count);
        }

        public bool FinishEncoding()
        {
            return finishFlag;
        }

        public void EnqueueFrame(FrameData newFrame)
        {
            lock (frameQueue)
            {
                frameQueue.Enqueue(newFrame);
            }
        }

        public void Abort()
        {
            encodeThread.Abort();
        }


        [DllImport("VideoCaptureLib")]
        static extern void VideoCaptureLib_WriteFrames(System.IntPtr api, byte[] data, int count);
    
    }
}