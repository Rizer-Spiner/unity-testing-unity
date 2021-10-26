using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUXTesting.EndregasWarriors.GameRecording;

namespace Tests
{
    public class AudioVideoMixerTest
    {
        
        // A Test behaves as an ordinary method
        [Test]
        public void mix_correct()
        {
            string videoPath = "C:/Users/spiri/Unity/unity-testing/Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Video_Test_data.mp4";
            string audioPath = "C:/Users/spiri/Unity/unity-testing/Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Audio_testData.wav";
            string finalVideo = "C:/Users/spiri/Unity/unity-testing/Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Final_video_data.mp4";
            var audioMixer = new AudioVideoMixer(videoPath, audioPath, finalVideo, 30);

            Assert.AreEqual(audioMixer.Muxing(), true);
            Assert.True(File.Exists(finalVideo));
        }

        [Test]
        public void mix_incorrect_file_does_not_exit()
        {
            string videoPath = "C:/Users/spiri/Unity/unity-testing/Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Video_Test_data_that_does_not_exits.mp4";
            string audioPath = "C:/Users/spiri/Unity/unity-testing/Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Audio_testData_that_does_not_exits.wav";
            string finalVideo = "C:/Users/spiri/Unity/unity-testing/Assets/UnityUXTesting/EndregasWarriors/Tests/TestData/Final_video_test_data_that_will_not_exist.mp4";
            var audioMixer = new AudioVideoMixer(videoPath, audioPath, finalVideo, 60);

            Assert.AreEqual(audioMixer.Muxing(), false);
            Assert.False(File.Exists(finalVideo));
        }
        
    }
}
