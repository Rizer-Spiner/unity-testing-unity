using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUXTesting.EndregasWarriors.Common;
using UnityUXTesting.EndregasWarriors.GameRecording;

namespace Tests
{
    public class AudioCaptureToolTests
    {
        private AudioCaptureTool subject;
        private bool condition;
        private AudioSource _audioSource;
        private GameRecordingBrain brain;

        [SetUp]
        public void SetUp()
        {
            var gameObjectAudio = new GameObject("Audio Source");
            gameObjectAudio.AddComponent<AudioSource>();
            gameObjectAudio.GetComponent<AudioSource>().clip =
                new WWW("Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Rock menu track.mp3").GetAudioClip(false,
                    true, AudioType.WAV);
            
            AudioCaptureTool.eventDelegate.onReady += MethodCalled;
            
            var gameObject = new GameObject("AudioTest");
            gameObject.AddComponent<AudioListener>();
            gameObject.AddComponent<GameRecordingBrain>();
            gameObject.AddComponent<AudioCaptureTool>();

            subject = gameObject.GetComponent<AudioCaptureTool>();
            _audioSource = gameObjectAudio.GetComponent<AudioSource>();
            brain = gameObject.GetComponent<GameRecordingBrain>();

        }

        private void MethodCalled(string type)
        {
            Debug.Log("type");
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator OnReadyFired()
        {
            Assert.True(condition);
            yield return null;
        }

        [UnityTest]
        public IEnumerator VerifyThatAudioFileExists()
        {
            // Wait for initialization 3 frames
            yield return null;
            yield return null;
            yield return null;
            
            _audioSource.Play();

            brain.status = CaptureSettings.StatusType.STARTED;
            yield return null;
            yield return null;

            var type = brain.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo audioPathInfo = type.GetField("audioFilePath", bindingAttr);
            
            Assert.True(File.Exists((string) audioPathInfo.GetValue(brain)));
        }

    }
}
