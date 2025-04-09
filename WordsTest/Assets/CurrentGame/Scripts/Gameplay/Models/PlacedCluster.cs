using System;

namespace WordsTest.Gameplay.Models
{
    [Serializable]
    public class PlacedCluster
    {
        public LetterPosition position;
        public Cluster cluster;
    }
}