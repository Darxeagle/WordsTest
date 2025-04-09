using System;
using System.Collections.Generic;

namespace CurrentGame.Gameplay.Models
{
    [Serializable]
    public class LevelData
    {
        public int id;
        public List<string> words;
        public List<string> clusters;
    }
}