using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using RockVR.Common;
using RockVR.Video;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRecordingBrain : Singleton<MonoBehaviour>
{
    [SerializeField] private VideoCaptureBase[] _videoCapture;
    [SerializeField] private AudioCapture _audioCapture;
    
    public VideoCaptureCtrlBase.StatusType status { get; protected set; }
    public bool debug = false;
    public bool startOnAwake = false;
    public float captureTime = 10f;
    public bool quitAfterCapture = false;
    public EventDelegate eventDelegate = new EventDelegate();

    private bool hasBeenAwakeBefore = false;
    
    private int videoCaptureFinishCount;
    private int videoCaptureRequiredCount;
    private Thread videoMergeThread;
    private Thread garbageCollectionThread;
    private bool isCaptureAudio;
    private bool isOfflineRender;
    
    public AudioCapture audioCapture
    {
        get
        {
            return _audioCapture;
        }
        set
        {
            if (status == VideoCaptureCtrlBase.StatusType.STARTED)
            {
                Debug.LogWarning("[VideoCaptureCtrl::AudioCapture] Cannot " +
                                 " set aduio during capture session!");
                return;
            }
            _audioCapture = value;
        }
    }
    
    public VideoCaptureBase[] videoCaptures
    {
        get
        {
            return _videoCapture;
        }
        set
        {
            if (status == VideoCaptureCtrlBase.StatusType.STARTED)
            {
                Debug.LogWarning("[VideoCaptureCtrl::VideoCaptures] Cannot " +
                                 "set camera during capture session!");
                return;
            }
            _videoCapture = value;
        }
    }

    private void Start()
    {
        videoCaptures = FindObjectsOfType<VideoCapture>();
        audioCapture = FindObjectOfType<AudioCapture>();
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += ChangeSceneConfiguration;
        Debug.Log(status);
        StartCapture();
    }

    private void ChangeSceneConfiguration(Scene arg0, LoadSceneMode arg1)
    {
        if (hasBeenAwakeBefore)
        {
            StopCapture();
            DestroyCurrentComponents();
            FindNewComponents();
            StartCapture();
        }
       
    }

    private void DestroyCurrentComponents()
    {
        for (int i = 0; i < videoCaptures.Length; i++)
        {
            Destroy(videoCaptures[i].gameObject);
        }
        if(audioCapture != null) 
            Destroy(audioCapture.gameObject);
    }

    private void FindNewComponents()
    {
        videoCaptures = FindObjectsOfType<VideoCapture>();
        audioCapture = FindObjectOfType<AudioCapture>();
    }
    

    private void Awake()
    {
        if (!hasBeenAwakeBefore)
        {
            hasBeenAwakeBefore = true;
            if (!Directory.Exists((PathConfig.SaveFolder)))
            {
                Directory.CreateDirectory(PathConfig.SaveFolder);
            }
        }
        status = VideoCaptureCtrlBase.StatusType.NOT_START;

        base.Awake();
    }

    protected override void OnApplicationQuit()
    {
        StopCapture();
        base.OnApplicationQuit();
    }

    void StartCapture()
    {
        if (status == VideoCaptureCtrlBase.StatusType.NOT_START)
        {
            // Filter out disabled capture component.
            List<VideoCapture> validCaptures = new List<VideoCapture>();
            if (videoCaptures != null && videoCaptures.Length > 0)
            {
                foreach (VideoCapture videoCapture in videoCaptures)
                {
                    if (videoCapture != null && videoCapture.gameObject.activeSelf)
                    {
                        validCaptures.Add(videoCapture);
                    }
                }
            }

            videoCaptures = validCaptures.ToArray();
            // Cache those value, thread cannot access unity's object.
            isCaptureAudio = false;
            if (audioCapture != null && audioCapture.gameObject.activeSelf)
                isCaptureAudio = true;
            // Check if can start a capture session.
            if (!isCaptureAudio && videoCaptures.Length == 0)
            {
                Debug.LogError(
                    "[VideoCaptureCtrl::StartCapture] StartCapture called " +
                    "but no attached VideoRecorder or AudioRecorder were found!"
                );
                return;
            }

            if (!File.Exists(PathConfig.ffmpegPath))
            {
                Debug.LogError(
                    "[VideoCaptureCtrl::StartCapture] FFmpeg not found, please add " +
                    "ffmpeg executable before start capture!"
                );
                return;
            }

            // Loop through each of the video capture component, initialize 
            // and start recording session.
            videoCaptureRequiredCount = 0;
            for (int i = 0; i < videoCaptures.Length; i++)
            {
                VideoCapture videoCapture = (VideoCapture) videoCaptures[i];
                if (videoCapture == null || !videoCapture.gameObject.activeSelf)
                {
                    continue;
                }

                videoCaptureRequiredCount++;
                if (videoCapture.status != VideoCaptureCtrlBase.StatusType.NOT_START &&
                    videoCapture.status != VideoCaptureCtrlBase.StatusType.FINISH)
                {
                    return;
                }

                if (videoCapture.offlineRender)
                {
                    isOfflineRender = true;
                }

                videoCapture.StartCapture();
                videoCapture.eventDelegate.OnComplete += OnVideoCaptureComplete;
            }

            // Check if record audio.
            if (IsCaptureAudio())
            {
                audioCapture.StartCapture();
                audioCapture.eventDelegate.OnComplete += OnAudioCaptureComplete;
            }

            // Reset record session count.
            videoCaptureFinishCount = 0;
            // Start garbage collect thread.
            garbageCollectionThread = new Thread(GarbageCollectionThreadFunction);
            garbageCollectionThread.Priority = System.Threading.ThreadPriority.Lowest;
            garbageCollectionThread.IsBackground = true;
            garbageCollectionThread.Start();
            // Update current status.
            status = VideoCaptureCtrlBase.StatusType.STARTED;
        }
        else
        {
            {
                Debug.LogWarning("[VideoCaptureCtrl::StartCapture] Previous " +
                                 " capture not finish yet!");
                return;
            }
        }
    }
    
    private void OnVideoCaptureComplete()
    {
        videoCaptureFinishCount++;
        if (videoCaptureFinishCount == videoCaptureRequiredCount && // Finish all video capture.
            !isCaptureAudio)// No audio capture required.
        {
            status = VideoCaptureCtrlBase.StatusType.FINISH;
            if (eventDelegate.OnComplete != null)
                eventDelegate.OnComplete();
        }
    }
    
    private void OnAudioCaptureComplete()
    {
        // Start merging thread when we have videos captured.
        if (IsCaptureAudio())
        {
            videoMergeThread = new Thread(VideoMergeThreadFunction);
            videoMergeThread.Priority = System.Threading.ThreadPriority.Lowest;
            videoMergeThread.IsBackground = true;
            videoMergeThread.Start();
        }
    }
    
    private void VideoMergeThreadFunction()
    {
        // Wait for all video record finish.
        while (videoCaptureFinishCount < videoCaptureRequiredCount)
        {
            Thread.Sleep(1000);
        }
        foreach (VideoCapture videoCapture in videoCaptures)
        {
            // TODO, make audio live streaming work
            if (
                videoCapture.mode == VideoCapture.ModeType.LIVE_STREAMING ||
                // Dont merge audio when capture equirectangular, its not sync.
                videoCapture.format == VideoCapture.FormatType.PANORAMA)
            {
                continue;
            }
            VideoMuxing muxing = new VideoMuxing(videoCapture, audioCapture);
            if (!muxing.Muxing())
            {
                if (eventDelegate.OnError != null)
                    eventDelegate.OnError((int)VideoCaptureCtrlBase.ErrorCodeType.VIDEO_AUDIO_MERGE_TIMEOUT);
            }
            PathConfig.lastVideoFile = muxing.filePath;
        }
        status = VideoCaptureCtrlBase.StatusType.FINISH;
        if (eventDelegate.OnComplete != null)
            eventDelegate.OnComplete();
        Cleanup();
    }
    
    private void Cleanup()
    {
        foreach (VideoCapture videoCapture in videoCaptures)
        {
            // Dont clean panorama video, its not include in merge thread.
            if (videoCapture.format == VideoCapture.FormatType.PANORAMA)
            {
                continue;
            }
            videoCapture.eventDelegate.OnComplete -= OnVideoCaptureComplete;
            videoCapture.Cleanup();
        }
        if (isCaptureAudio)
        {
            audioCapture.eventDelegate.OnComplete -= OnAudioCaptureComplete;
            audioCapture.Cleanup();
        }
    }
    
    void GarbageCollectionThreadFunction()
    {
        while (status == VideoCaptureCtrlBase.StatusType.STARTED)
        {
            // TODO, adjust gc interval dynamic.
            Thread.Sleep(1000);
            System.GC.Collect();
        }
    }
    
    private bool IsCaptureAudio()
    {
        return isCaptureAudio && !isOfflineRender;
    }

    private void StopCapture()
    {
        if (status != VideoCaptureCtrlBase.StatusType.STARTED && status != VideoCaptureCtrlBase.StatusType.PAUSED)
        {
            Debug.LogWarning("[VideoCaptureCtrl::StopCapture] capture session " +
                             "not start yet!");
            return;
        }
        foreach (VideoCapture videoCapture in videoCaptures)
        {
            if (!videoCapture.gameObject.activeSelf)
            {
                continue;
            }
            if (videoCapture.status != VideoCaptureCtrlBase.StatusType.STARTED && status != VideoCaptureCtrlBase.StatusType.PAUSED)
            {
                if (IsCaptureAudio())
                {
                    audioCapture.eventDelegate.OnComplete -= OnAudioCaptureComplete;
                    audioCapture.StopCapture();
                }
                videoCapture.eventDelegate.OnComplete -= OnVideoCaptureComplete;
                status = VideoCaptureCtrlBase.StatusType.NOT_START;
                return;
            }
            videoCapture.StopCapture();
            PathConfig.lastVideoFile = videoCapture.filePath;
        }
        if (IsCaptureAudio())
        {
            audioCapture.StopCapture();
        }
        status = VideoCaptureCtrlBase.StatusType.STOPPED;
    }

    private void ToggleCapture()
    {
        foreach (VideoCapture videoCapture in videoCaptures)
        {
            videoCapture.ToggleCapture();
        }
        if (IsCaptureAudio())
        {
            audioCapture.PauseCapture();
        }
        if (status != VideoCaptureCtrlBase.StatusType.PAUSED)
        {
            status = VideoCaptureCtrlBase.StatusType.PAUSED;
        }
        else
        {
            status = VideoCaptureCtrlBase.StatusType.STARTED;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        ToggleCapture();
    }

    
}
