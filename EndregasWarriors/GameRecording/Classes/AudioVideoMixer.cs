using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;

namespace UnityUXTesting.EndregasWarriors.GameRecording
{
    public class AudioVideoMixer
    {
        private string finalFilePath;
        private string videoFilePath;
        private string audioFilePath;

        private int bitrate;

        public AudioVideoMixer(string videoPath, string audioPath, string finalPath, int bitRate)
        {
            videoFilePath = videoPath;
            audioFilePath = audioPath;
            finalFilePath = finalPath;
            bitrate = bitRate;
        }

        public bool Muxing()
        {
            System.IntPtr libAPI = MuxingLib_Get(
                bitrate,
                finalFilePath,
                videoFilePath,
                audioFilePath,
                PathConfig.ffmpegPath);
            if (libAPI == System.IntPtr.Zero)
            {
                Debug.LogWarning("[VideoMuxing::Muxing] Get native LibVideoMergeAPI failed!");
                return false;
            }
            MuxingLib_Muxing(libAPI);
            // Make sure generated the merge file.
            int waitCount = 0;
            while (!File.Exists(finalFilePath))
            {
                if (waitCount++ < 100)
                    Thread.Sleep(500);
                else
                {
                    Debug.LogWarning("[VideoMuxing::Muxing] Mux process failed!");
                    MuxingLib_Clean(libAPI);
                    return false;
                }
            }
            MuxingLib_Clean(libAPI);
            return true;
        }
        
        
        #region Dll Import
        [DllImport("VideoCaptureLib")]
        static extern System.IntPtr MuxingLib_Get(int rate, string path, string vpath, string apath, string ffpath);

        [DllImport("VideoCaptureLib")]
        static extern void MuxingLib_Muxing(System.IntPtr api);

        [DllImport("VideoCaptureLib")]
        static extern void MuxingLib_Clean(System.IntPtr api);
        #endregion // Dll Import
    }
}