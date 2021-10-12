using System;
using System.Linq;
using RockVR.Common;
using RockVR.Video;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FindObject : MonoBehaviour
{

    [SerializeField] 
    private VideoCapture _capture;

    [SerializeField] private AudioCapture _audioCapture;

    private void Start()
    {
        Debug.Log("I am in Start");
        _capture = FindObjectOfType<VideoCapture>();
        _audioCapture = FindObjectOfType<AudioCapture>();
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += FindComponents;
    }

    private void FindComponents(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("I am in FindComponents");
        _capture = FindObjectOfType<VideoCapture>();
        _audioCapture = FindObjectOfType<AudioCapture>();
    }


    private void Update()
    {
        if (!_capture)
        {
            Debug.Log("_capture not found");
        }
        else
        {
            Debug.Log("Found capture");
        }

        if (!_audioCapture)
        {
            Debug.Log("_audiocapture not found");
        }
        else
        {
            Debug.Log("Found audioCapture");
        }
    }
}
