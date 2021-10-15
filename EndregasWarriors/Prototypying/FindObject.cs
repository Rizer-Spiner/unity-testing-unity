using System;
using System.Collections;
using System.Linq;
using System.Threading;
using RockVR.Common;
using RockVR.Video;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUXTesting.EndregasWarriors.Prototypying;


public class FindObject : MonoBehaviour
{
    public VideoCapture _capture;

    private bool _lock;
    public AudioCapture _audioCapture;

    private void Start()
    {
        Debug.Log("I am in Start");
        _capture = FindObjectOfType<VideoCapture>();
        Debug.Log(_capture);
        _audioCapture = FindObjectOfType<AudioCapture>();
        Debug.Log(_audioCapture);
        if(!FindObjectOfType<RecordingBrain>()) Debug.Log("Nooooooo brain???+");
        FindObjectOfType<RecordingBrain>()._Audio = _audioCapture;
        FindObjectOfType<RecordingBrain>()._Video = _capture;
    }


    
}