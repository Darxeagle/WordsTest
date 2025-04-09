using System;
using System.Collections.Generic;

namespace WordsTest.Gameplay.Models
{
    [Serializable]
    public class Word
    {
        public List<char> characters;
        public int length;

        public Word(string word)
        {
            characters = new List<char>(word.ToCharArray());
            length = characters.Count;
        }
    }
}