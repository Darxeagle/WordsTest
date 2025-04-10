using System.Collections.Generic;
using System.Linq;
using CurrentGame.Gameplay.Controllers;
using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Saves;
using CurrentGame.Gameplay.Views;
using CurrentGame.MainMenu;
using CurrentGame.Options;
using CurrentGame.Victory;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CurrentGame.GameFlow
{
    public class GameController : IInitializable
    {
        [Inject] private TransitionManager transitionManager;
        [Inject] private LevelController levelController;
        [Inject] private VictoryScreen victoryScreen;
        [Inject] private LevelView levelView;
        [Inject] private GameplayUI gameplayUI;
        [Inject] private LevelModel levelModel;
        [Inject] private GameModel gameModel;
        [Inject] private MainMenuScreen mainMenuScreen;
        [Inject] private OptionsScreen optionsScreen;
        [Inject] private SaveManager saveManager;
        [Inject] private LevelsDataManager levelsDataManager;
        
        public void Initialize()
        {
            saveManager.LoadGame();
            StartGame();
        }
        
        private void StartGame()
        {
            transitionManager.OpeningTransitionAsync(() =>
            {
                mainMenuScreen.gameObject.SetActive(true);
                gameplayUI.gameObject.SetActive(false);
                levelView.gameObject.SetActive(false);
                victoryScreen.gameObject.SetActive(false);
                optionsScreen.gameObject.SetActive(false);
            }).Forget();
        }

        public void CheckFinish()
        {
            var placedClustersGroups = levelModel.PlacedClusters.GroupBy(pc => pc.position.wordIndex).ToList();
            var wordsLeft = levelModel.Words.ToList();
            var completedWords = new List<Word>();
            
            foreach (var placedClusterGroup in placedClustersGroups)
            {
                var orderedClusters = placedClusterGroup.OrderBy(pc => pc.position.charIndex).ToList();
                var completeChars = new char[orderedClusters.Last().position.charIndex + orderedClusters.Last().cluster.length];
                
                foreach (var placedCluster in orderedClusters)
                {
                    for (int i = 0; i < placedCluster.cluster.length; i++)
                    {
                        completeChars[placedCluster.position.charIndex + i] = placedCluster.cluster.characters[i];
                    }
                }
                
                var word = wordsLeft.FirstOrDefault(w => w.characters.SequenceEqual(completeChars));
                if (word != null)
                {
                    wordsLeft.Remove(word);
                    completedWords.Add(word);
                }
            }
            
            if (wordsLeft.Count == 0)
            {
                transitionManager.TransitionAsync(() =>
                {
                    victoryScreen.gameObject.SetActive(true);
                    victoryScreen.SetWords(completedWords);
                    levelView.gameObject.SetActive(false);
                    gameplayUI.gameObject.SetActive(false);
                    gameModel.CurrentLevel++;
                    levelModel.Clear();
                }).Forget();
            }
        }

        public void StartLevel()
        {
            if (!levelModel.Initialized)
            {
                levelModel.Apply(levelsDataManager.GetLevelByIndex(gameModel.CurrentLevel));
            }
            
            transitionManager.TransitionAsync(() =>
            {
                levelController.InitializeLevel();
                levelView.gameObject.SetActive(true);
                gameplayUI.gameObject.SetActive(true);
                mainMenuScreen.gameObject.SetActive(false);
                victoryScreen.gameObject.SetActive(false);
            }).Forget();
        }

        public void ToMainMenu(bool fromGame)
        {
            if (fromGame)
            {
                transitionManager.TransitionAsync(() =>
                {
                    levelView.gameObject.SetActive(false);
                    gameplayUI.gameObject.SetActive(false);
                    mainMenuScreen.gameObject.SetActive(true);
                    optionsScreen.gameObject.SetActive(false);
                }).Forget();
            }
            else
            {
                transitionManager.TransitionAsync(() =>
                {
                    optionsScreen.gameObject.SetActive(false);
                    mainMenuScreen.gameObject.SetActive(true);
                }).Forget();
            }
        }

        public void ToOptions(bool fromMenu)
        {
            if (fromMenu)
            {
                transitionManager.TransitionAsync(() =>
                {
                    optionsScreen.gameObject.SetActive(true);
                    optionsScreen.FromMenu(true);
                    mainMenuScreen.gameObject.SetActive(false);
                }).Forget();
            }
            else
            {
                optionsScreen.gameObject.SetActive(true);
            }
        }
    }
}