using System;

namespace CurrentGame.Gameplay.Models
{
    [Serializable]
    public class PlacedCluster
    {
        public LetterPosition position;
        public Cluster cluster;
    }
}