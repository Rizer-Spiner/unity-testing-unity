using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUXTesting.EndregasWarriors.BugReporting.Monobehaviour;
using UnityUXTesting.EndregasWarriors.Common.Model;

namespace UnityUXTesting.EndregasWarriors.PlayRunReporting.Checkpoints
{
    public class LevelDataManager : UXTool
    {
        public static LevelDataManager _instance;
        public List<LevelData> levelData;
        private float _levelStartTimeStamp;

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

            _levelStartTimeStamp = BugReportingManager._instance.countedTime;
            base.Awake();
        }

        protected override void Start()
        {
            CheckPointsManager._eventDelegate.SaveCheckPointsForLevel += SaveLevelData;
            base.Start();
        }

        public void SaveLevelData(CheckPointResult[] results)
        {
            string levelName = SceneManager.GetActiveScene().name;
            TimeInterval playInterval = new TimeInterval
            {
                start = _levelStartTimeStamp,
                end = BugReportingManager._instance.countedTime
            };

            LevelData levelD = new LevelData
            {
                levelName = levelName,
                playTimeInterval = playInterval,
                checkpoints = results
            };

            levelData.Add(levelD);
        }
    }
}