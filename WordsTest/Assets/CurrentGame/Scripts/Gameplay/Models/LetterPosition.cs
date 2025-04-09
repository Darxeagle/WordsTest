using System;

namespace WordsTest.Gameplay.Models
{
    [Serializable]
    public class LetterPosition
    {
        public int wordIndex;
        public int charIndex;
        
        public LetterPosition(int wordIndex, int charIndex)
        {
            this.wordIndex = wordIndex;
            this.charIndex = charIndex;
        }
    }
}