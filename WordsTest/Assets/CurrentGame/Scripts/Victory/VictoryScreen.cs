using System.Collections.Generic;
using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Controllers;
using CurrentGame.Gameplay.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CurrentGame.Victory
{
    public class VictoryScreen : MonoBehaviour
    {
        [SerializeField] private Transform wordsContainer;
        [SerializeField] private VictoryScreenWord wordPrefab;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button nextLevelButton;
        
        [Inject] private LevelController levelController;
        [Inject] private TransitionManager transitionManager;
        [Inject] private LevelModel levelModel;
        [Inject] private GameController gameController;
        
        private List<VictoryScreenWord> wordViews = new();
        
        private void Awake()
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        public void SetWords(List<Word> words)
        {
            Clear();
            Appear(words);
        }

        private void Appear(List<Word> words)
        {
            for (int i = 0; i < words.Count; i++)
            {
                var wordView = Instantiate(wordPrefab, wordsContainer);
                wordViews.Add(wordView);
                wordView.SetWord(words[i], i * 0.2f);
            }
        }

        private void Clear()
        {
            foreach (var wordView in wordViews)
            {
                if (wordView != null)
                    Destroy(wordView.gameObject);
            }
            wordViews.Clear();
        }
        
        private void OnMainMenuButtonClicked()
        {
            gameController.ToMainMenu(true);
        }
        
        private void OnNextLevelButtonClicked()
        {
            gameController.StartLevel();
        }
        
        private void OnDestroy()
        {
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
            if (nextLevelButton != null)
                nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
        }
    }
}