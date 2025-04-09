using System;
using System.Collections.Generic;

namespace WordsTest.Gameplay.Models
{
    [Serializable]
    public class LevelModel
    {
        public List<Word> words = new List<Word>();
        public List<PlacedCluster> placedClusters = new List<PlacedCluster>();
        public List<Cluster> paletteClusters = new List<Cluster>();
    }
}