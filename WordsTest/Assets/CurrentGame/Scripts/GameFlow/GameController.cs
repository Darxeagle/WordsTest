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
        [Inject] private EventManager eventManager;
        
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

        public async UniTask CheckFinish()
        {
            var completedWords = levelController.CheckCompleted();
            
            if (completedWords != null)
            {
                gameplayUI.SetInputEnabled(false);
                eventManager.TriggerEvent(EventId.CheckStarted);
                await levelView.BlinkCheckAllClusters();
                eventManager.TriggerEvent(EventId.CheckCompleted);
                await levelView.BlinkCorrectAllClusters();
                await transitionManager.TransitionAsync(() =>
                {
                    victoryScreen.gameObject.SetActive(true);
                    victoryScreen.SetWords(completedWords);
                    levelView.gameObject.SetActive(false);
                    gameplayUI.gameObject.SetActive(false);
                    gameModel.CurrentLevel++;
                    levelModel.Clear();
                });
                gameplayUI.SetInputEnabled(true);
            }
            else
            {
                gameplayUI.SetInputEnabled(false);
                eventManager.TriggerEvent(EventId.CheckStarted);
                await levelView.BlinkCheckAllClusters();
                eventManager.TriggerEvent(EventId.CheckFailed);
                await levelView.BlinkWrongAllClusters();
                gameplayUI.SetInputEnabled(true);
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
                eventManager.TriggerEvent(EventId.LevelStarted);
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
                    victoryScreen.gameObject.SetActive(false);
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
                transitionManager.TransitionAsync(() =>
                {
                    optionsScreen.gameObject.SetActive(true);
                    optionsScreen.FromMenu(false);
                    levelView.gameObject.SetActive(false);
                    gameplayUI.gameObject.SetActive(false);
                }).Forget();
            }
        }

        public void CloseOptions(bool fromMenu)
        {
            if (fromMenu)
            {
                ToMainMenu(false);
            }
            else
            {
                transitionManager.TransitionAsync(() =>
                {
                    optionsScreen.gameObject.SetActive(false);
                    levelView.gameObject.SetActive(true);
                    gameplayUI.gameObject.SetActive(true);
                }).Forget();
            }
        }
    }
}