using System;
using RockVR.Video;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityUXTesting.EndregasWarriors.Prototypying
{
    public class RecordingBrain : MonoBehaviour
    {
        private RecordingBrain _instance;
        
        
        public VideoCapture _Video;
        public AudioCapture _Audio;

        void  Awake()
        {
            DontDestroyOnLoad(this);
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += (arg0, mode) =>
            {
                Debug.Log("Initializing removing old components");
                Destroy(_Video.gameObject);
                Destroy(_Audio.gameObject);
                Debug.Log("Old components deleted");
            };
        }

        private void Update()
        {
            if (!_Video)
            {
                Debug.Log("_capture not found");
            }
            else
            {
                Debug.Log("Found capture");
            }

            if (!_Audio)
            {
                Debug.Log("_audiocapture not found");
            }
            else
            {
                Debug.Log("Found audioCapture");
            }
        }
    }
}