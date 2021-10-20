using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.GameRecording;
using static UnityUXTesting.EndregasWarriors.Common.CaptureSettings;


public class GameRecordingBrainBase : UXTool
{

    #region Variables
    
    public  string finalVideoFilePath;
    private string videoFilePath;
    private string audioFilePath;
    
    [NonSerialized]
    public VideoEncoder _encoder;
    [NonSerialized]
    public IntPtr videoLibAPI;
    [NonSerialized]
    public IntPtr audioLibAPI;
    
    public StereoFormatType stereoFormat = StereoFormatType.HALF;
    public FrameSizeType frameSize = FrameSizeType._1280x720;
    public AntiAliasingType _antiAliasing = AntiAliasingType._4;
    public EncodeQualityType encodeQuality = EncodeQualityType.Medium;
    public TargetFramerateType _targetFramerate = TargetFramerateType._30;

    public EventDelegate eventDelegate;


    private int frameWidth
    {
        get
        {
            int width = 0;
            if (frameSize == FrameSizeType._640x480) { width = 640; }
            if (frameSize == FrameSizeType._720x480) { width = 720; }
            if (frameSize == FrameSizeType._960x540) { width = 960; }
            if (frameSize == FrameSizeType._1280x720) { width = 1280; }
            if (frameSize == FrameSizeType._1920x1080) { width = 1920; }
            if (frameSize == FrameSizeType._2048x1080) { width = 2048; }
            if (frameSize == FrameSizeType._3840x2160) { width = 3840; }
            if (frameSize == FrameSizeType._4096x2160) { width = 4096; }
            if (frameSize == FrameSizeType._7680x4320) { width = 7680; }

            return width;
        }
    }
    
    private int frameHeight
    {
        get
        {
            int height = 0;
            if (frameSize == FrameSizeType._640x480 ||
                frameSize == FrameSizeType._720x480) { height = 480; }
            if (frameSize == FrameSizeType._960x540) { height = 540; }
            if (frameSize == FrameSizeType._1280x720) { height = 720; }
            if (frameSize == FrameSizeType._1920x1080 ||
                frameSize == FrameSizeType._2048x1080) { height = 1080; }
            if (frameSize == FrameSizeType._3840x2160 ||
                frameSize == FrameSizeType._4096x2160) { height = 2160; }
            if (frameSize == FrameSizeType._7680x4320) { height = 4320; }
            return height;
        }
    }
    
    private int antiAliasing
    {
        get
        {
            if (_antiAliasing == AntiAliasingType._1) { return 1; }
            if (_antiAliasing == AntiAliasingType._2) { return 2; }
            if (_antiAliasing == AntiAliasingType._4) { return 4; }
            if (_antiAliasing == AntiAliasingType._8) { return 8; }
            return 0;
        }
    }
    
    private int bitrate
    {
        get
        {
            if (encodeQuality == EncodeQualityType.Low) { return 1000; }
            if (encodeQuality == EncodeQualityType.Medium) { return 2500; }
            if (encodeQuality == EncodeQualityType.High) { return 5000; }
            return 0;
        }
    }
    
    private int targetFramerate
    {
        get
        {
            if (_targetFramerate == TargetFramerateType._18) { return 18; }
            if (_targetFramerate == TargetFramerateType._24) { return 24; }
            if (_targetFramerate == TargetFramerateType._30) { return 30; }
            if (_targetFramerate == TargetFramerateType._45) { return 45; }
            if (_targetFramerate == TargetFramerateType._60) { return 60; }
            return 0;
        }
    }
    public int GetFrameRate()
    {
        return targetFramerate;
    }

    public int GetFrameWidth()
    {
        return frameWidth;
    }

    public int GetFrameHeight()
    {
        return frameHeight;
    }

    public int GetAntiAliasing()
    {
        return antiAliasing;
    }
    #endregion
    
    
    protected override void Awake()
    {
        
        videoFilePath = PathConfig.SaveFolder + Utils.StringUtils.GetMp4FileName(Utils.StringUtils.GetRandomString(5)); 
        audioFilePath = PathConfig.SaveFolder + Utils.StringUtils.GetWavFileName(Utils.StringUtils.GetRandomString(5));
        
        Debug.Log("Step1");
        videoLibAPI = VideoCaptureLib_Get(
            frameWidth,
            frameHeight,
            targetFramerate,
            0,
            videoFilePath,
            PathConfig.ffmpegPath);
           
        if (videoLibAPI == IntPtr.Zero)
        {
            Debug.LogWarning("[VideoCapture::StartCapture] Get native " +
                             "capture api failed!");
            return;
        }
        Debug.Log("Step2");
        audioLibAPI = AudioCaptureLib_Get(
            AudioSettings.outputSampleRate,
            audioFilePath,
            PathConfig.ffmpegPath);
        
        if (audioLibAPI == IntPtr.Zero)
        {
            Debug.LogWarning("[AudioCapture::StartCapture] Get native " +
                             "LibAudioCaptureAPI failed!");
            return;
        }
        Debug.Log("Step3");
        _encoder = new VideoEncoder(videoLibAPI);
        Debug.Log("Step4");
        base.Awake();
    }

    public bool MixAudioWithVideo()
    {
        finalVideoFilePath = PathConfig.saveFolder + Utils.StringUtils.GetMp4FileName(Utils.StringUtils.GetRandomString(5));

        AudioVideoMixer mixer = new AudioVideoMixer(videoFilePath, audioFilePath, finalVideoFilePath, bitrate);
        return mixer.Muxing();
    }

    public void CloseLibAPIs()
    {
        VideoCaptureLib_Close(videoLibAPI);
        AudioCaptureLib_Close(audioLibAPI);
    }

    public void CleanLibAPIs()
    {
        VideoCaptureLib_Clean(videoLibAPI);
        AudioCaptureLib_Clean(audioLibAPI);
        
        if (File.Exists(videoFilePath)) File.Delete(videoFilePath);
        if (File.Exists(audioFilePath)) File.Delete(audioFilePath);
    }
    
 
    
    
    #region Dll Import
    [DllImport("VideoCaptureLib")]
    static extern IntPtr VideoCaptureLib_Get(int width, int height, int rate, int proj, string path, string ffpath);
    
    [DllImport("VideoCaptureLib")]
    static extern void VideoCaptureLib_Close(System.IntPtr api);
    
    [DllImport("VideoCaptureLib")]
    static extern void VideoCaptureLib_Clean(System.IntPtr api);
    
    [DllImport("VideoCaptureLib")]
    static extern IntPtr AudioCaptureLib_Get(int rate, string path, string ffpath);
    
    [DllImport("VideoCaptureLib")]
    static extern void AudioCaptureLib_Close(System.IntPtr api);
    
    [DllImport("VideoCaptureLib")]
    static extern void AudioCaptureLib_Clean(System.IntPtr api);

    #endregion // Dll Import
}
