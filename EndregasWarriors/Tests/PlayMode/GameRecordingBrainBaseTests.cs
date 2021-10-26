using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameRecordingBrainBaseTests
    {
        private GameRecordingBrainBase subject;
        
        [SetUp]
        public void SetUp()
        {
            var gameObject = new GameObject("GameBrain");
            gameObject.AddComponent<GameRecordingBrain>();

            subject = gameObject.GetComponent<GameRecordingBrainBase>();
        }

        [UnityTest]
        public IEnumerator MixVideoWithAudio_Pass()
        {
            yield return null;
            var type = subject.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            MethodInfo mix = type.GetMethod("MixAudioWithVideo", bindingAttr);
            
            FieldInfo audioPathInfo = type.GetField("audioFilePath", bindingAttr);
            FieldInfo videoFilePath = type.GetField("videoFilePath", bindingAttr);
            
            audioPathInfo.SetValue(subject, "Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Audio_testData.wav");
            videoFilePath.SetValue(subject, "Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Video_Test_data.mp4");
            
            Assert.True((bool) mix.Invoke(subject, null));
            
            FieldInfo finalVideoFilePathInfo = type.GetField("finalVideoFilePath", bindingAttr);
           
            Assert.True(File.Exists( (string) finalVideoFilePathInfo.GetValue(subject)));
            yield return null;
        }

        [UnityTest]
        public IEnumerator MixVideoWithAudio_Fail()
        {
            yield return null;
            var type = subject.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            // MethodInfo mix = type.GetMethod("MixAudioWithVideo", bindingAttr);
            
            FieldInfo audioPathInfo = type.GetField("audioFilePath", bindingAttr);
            FieldInfo videoFilePath = type.GetField("videoFilePath", bindingAttr);
            
            audioPathInfo.SetValue(subject, "Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Audio_testData_that_doesnt_exists.wav");
            videoFilePath.SetValue(subject, "Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Video_Test_data_that_doesnt_exists.mp4");
            
            // Assert.False((bool) mix.Invoke(subject, null));
            Assert.False(subject.MixAudioWithVideo());
            FieldInfo finalVideoFilePathInfo = type.GetField("finalVideoFilePath", bindingAttr);
           
            Assert.False(File.Exists( (string) finalVideoFilePathInfo.GetValue(subject)));
            yield return null;

        }

        [UnityTest]
        public IEnumerator CloseLibAPIs()
        {
            yield return null;
            subject.CloseLibAPIs();
            yield return null;
            
            var type = subject._encoder.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo threadInfo = type.GetField("encodeThread", bindingAttr);

            Thread thread = (Thread) threadInfo.GetValue(subject);
            Assert.False(thread.IsAlive);
        }

        [UnityTest]
        public IEnumerator CleanLibAPIS()
        {
            yield return null;
            subject.CleanLibAPIs();
            yield return null;
            
            var type = subject.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            FieldInfo audioPathInfo = type.GetField("audioFilePath", bindingAttr);
            FieldInfo videoFilePath = type.GetField("videoFilePath", bindingAttr);
            
            Debug.Log( "Hellooo " + audioPathInfo.GetValue(subject));

            Assert.False(File.Exists((string) audioPathInfo.GetValue(subject)));
            Assert.False(File.Exists((string) videoFilePath.GetValue(subject)));
        }
        
    }
}
