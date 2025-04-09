using System;
using System.Collections.Generic;

namespace CurrentGame.Gameplay.Models
{
    [Serializable]
    public class Cluster
    {
        public List<char> characters;
        public int length;

        public Cluster(string chars)
        {
            characters = new List<char>(chars.ToCharArray());
            length = characters.Count;
        }
    }
}