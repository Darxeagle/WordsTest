using System;
using System.Collections.Generic;

namespace WordsTest.Gameplay.Models
{
    [Serializable]
    public class LevelData
    {
        public List<string> words;
        public List<string> clusters;
    }
}