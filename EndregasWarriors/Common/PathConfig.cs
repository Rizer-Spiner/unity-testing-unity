using System;
using UnityEngine;

namespace UnityUXTesting.EndregasWarriors.Common
{
    public class PathConfig
    {
        public static string persistentDataPath = Application.persistentDataPath;
        public static string streamingAssetsPath = Application.streamingAssetsPath;
        public static string myDocumentsPath = Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments);
        public static string saveFolder="";
        public static string lastVideoFile = "";
        public static string serverAddress = "http://localhost:8080";
        public static string gameName = "TestGame";
        public static string buildID = "BuildNr1";


        public static string GameName
        {
            get => gameName;
            set => gameName = value;
        }

        public static string BuildID
        {
            get => buildID;
            set => buildID = value;
        }

        public static string ServerAddress
        {
            get => serverAddress;
            set => serverAddress = value;
        }

        public static string SaveFolder
        {
            get
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                if (saveFolder == "")
                {
                    saveFolder = persistentDataPath + "/RockVR/Video/";
                }
                return saveFolder;
#else
                if (saveFolder == "")
                {
                    saveFolder = myDocumentsPath + "/RockVR/Video/";
                }
                return saveFolder;
#endif
            }
            set
            {
                saveFolder = value;
            }
        }
        /// <summary>
        /// The ffmpeg path.
        /// </summary>
        public static string ffmpegPath
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return streamingAssetsPath + "/RockVR/FFmpeg/Windows/ffmpeg.exe";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                return streamingAssetsPath + "/RockVR/FFmpeg/OSX/ffmpeg";
#else
                return "";
#endif
            }
        }

        /// <summary>
        /// The Spatial Media Metadata Injector path.
        /// </summary>
        public static string injectorPath
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return streamingAssetsPath + "/RockVR/Spatial Media Metadata Injector/Windows/Spatial Media Metadata Injector.exe";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                return streamingAssetsPath + "/RockVR/Spatial Media Metadata Injector/OSX/Spatial Media Metadata Injector";
#else
                return "";
#endif
            }
        }
        
        
    }
}