using System;

namespace UnityUXTesting.EndregasWarriors.Common.Model
{
    [Serializable]
    public class PlayRunReport
    {
        public string buildRef;
        public string gameRef;
        public string videoRef;

        public Bug[] bugReport;
        //public LevelData[] levelData { get; set; }
    }
}