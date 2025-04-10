using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CurrentGame.Victory
{
    public class VictoryScreenWord : MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [SerializeField] private VictoryScreenLetter letterPrefab;
        [SerializeField] private float letterSpawnDelay = 0.1f;
        
        private List<VictoryScreenLetter> letters = new();

        public void SetWord(string word, float startDelay = 0f)
        {
            Clear();
            
            for (int i = 0; i < word.Length; i++)
            {
                var letterView = Instantiate(letterPrefab, transform);
                letters.Add(letterView);
                letterView.SetLetter(word[i], startDelay + i * letterSpawnDelay);
            }
        }

        private void Clear()
        {
            foreach (var letter in letters)
            {
                if (letter != null)
                    Destroy(letter.gameObject);
            }
            letters.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}