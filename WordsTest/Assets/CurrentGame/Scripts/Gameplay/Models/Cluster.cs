using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CurrentGame.Gameplay.Models
{
    [Serializable]
    public class Cluster
    {
        public List<char> characters;
        public int length;
        public int color;

        public Cluster(string chars)
        {
            characters = new List<char>(chars.ToCharArray());
            length = characters.Count;
            color = Random.Range(0, int.MaxValue);
        }
    }
}