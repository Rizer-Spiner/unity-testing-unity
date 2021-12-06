using System;

namespace UnityUXTesting.EndregasWarriors.Common.Model
{
    [Serializable]
    public class PlayRunReport
    {
        public string buildRef { get; set; }
        public string gameRef { get; set; }
        public string videoRef { get; set; }

        public Bug[] bugReport { get; set; }
        public LevelData[] levelData { get; set; }
    }
}