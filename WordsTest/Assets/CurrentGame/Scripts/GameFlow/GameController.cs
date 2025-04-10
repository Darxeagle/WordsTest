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
            }).Forget();
        }

        public void CheckFinish()
        {
            var placedClustersGroups = levelModel.PlacedClusters.GroupBy(pc => pc.position.wordIndex).ToList();
            var completedWords = new List<string>();
            
            foreach (var placedClusterGroup in placedClustersGroups)
            {
                var word = levelModel.Words[placedClusterGroup.Key];
                var orderedClusters = placedClusterGroup.OrderBy(pc => pc.position.charIndex).ToList();
                
                var wordChars = new char[word.length];
                var isComplete = true;
                
                foreach (var placedCluster in orderedClusters)
                {
                    for (int i = 0; i < placedCluster.cluster.length; i++)
                    {
                        var charIndex = placedCluster.position.charIndex + i;
                        if (charIndex < word.length && word.characters[charIndex] == placedCluster.cluster.characters[i])
                        {
                            wordChars[charIndex] = placedCluster.cluster.characters[i];
                        }
                        else
                        {
                            isComplete = false;
                            break;
                        }
                    }
                    if (!isComplete) break;
                }
                
                if (isComplete)
                {
                    completedWords.Add(new string(wordChars));
                }
            }
            
            if (completedWords.Count == levelModel.Words.Count)
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