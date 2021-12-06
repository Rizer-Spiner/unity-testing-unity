using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityUXTesting.EndregasWarriors.BugReporting.Monobehaviour;
using UnityUXTesting.EndregasWarriors.Common.Model;
using UnityUXTesting.EndregasWarriors.DataSending;
using UnityUXTesting.EndregasWarriors.PlayRunReporting.Checkpoints;

namespace UnityUXTesting.EndregasWarriors.PlayRunReporting
{
    public class PlayRunManager : UXTool
    {
        private PlayRunManager _instance;
        private IPlayRunService _service;

        public PathConfigScriptable configuration;

        protected override void Awake()
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

            base.Awake();
        }

        protected override void Start()
        {
            _service = new PlayRunServiceImpl(configuration);
            base.Start();
#if !UNITY_EDITOR
            Application.wantsToQuit += WantsToQuit;
#else
            EditorApplication.playModeStateChanged += change => ExitPlayMode(change);
#endif
        }


        private bool WantsToQuit()
        {
            PlayRunReport report = CreateReport();
            StartCoroutine(PostPlayReport(report));

#if !UNITY_EDITOR
            Application.wantsToQuit -= WantsToQuit;
#else
            EditorApplication.playModeStateChanged -= change => ExitPlayMode(change);
#endif
            return true;
        }

        private IEnumerator PostPlayReport(PlayRunReport report)
        {
            yield return _service.PostPlayRunReport(report);
        }

        private PlayRunReport CreateReport()
        {
            List<Bug> bugReport = BugReportingManager._instance.bugs;
            List<LevelData> levelDatas = LevelDataManager._instance.levelData;

            return new PlayRunReport()
            {
                bugReport = bugReport.ToArray(),
                levelData = levelDatas.ToArray(),
                buildRef = configuration.currentBuildID,
                gameRef = configuration.gameName,
                videoRef = new FileInfo(GameRecordingBrain._instance.finalVideoFilePath).Name
            };
        }

        private void ExitPlayMode(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode) WantsToQuit();
        }
    }
}