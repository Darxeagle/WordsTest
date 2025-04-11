using System;
using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Saves
{
    public class SaveManager : IInitializable, ITickable
    {
        private const string PREFS_GAME_MODEL = "GameModel";
        private const string PREFS_LEVEL_MODEL = "LevelModel";
        
        [Inject] private EventManager eventManager;
        [Inject] private GameModel gameModel;
        [Inject] private LevelModel levelModel;
        
        private bool modelChanged = false;


        public void Initialize()
        {
            eventManager.EventBus.Where(e => e == EventId.ModelUpdated).Subscribe(ModelChanged);
        }
        
        public void LoadGame()
        {
            if (PlayerPrefs.HasKey(PREFS_GAME_MODEL))
            {
                var gameModelJson = PlayerPrefs.GetString(PREFS_GAME_MODEL);
                gameModel.Apply(JsonUtility.FromJson<GameModel>(gameModelJson));
            }
            else
            {
                gameModel.CurrentLevel = 0;
            }

            if (PlayerPrefs.HasKey(PREFS_LEVEL_MODEL))
            {
                var levelModelJson = PlayerPrefs.GetString(PREFS_LEVEL_MODEL);
                levelModel.Apply(JsonUtility.FromJson<LevelModel>(levelModelJson));
            }
        }
        
        private void ModelChanged(EventId e)
        {
            modelChanged = true;
        }

        public void Tick()
        {
            if (modelChanged)
            {
                SaveGame();
                modelChanged = false;
            }
        }

        private void SaveGame()
        {
            PlayerPrefs.SetString(PREFS_GAME_MODEL, JsonUtility.ToJson(gameModel));
            PlayerPrefs.SetString(PREFS_LEVEL_MODEL, JsonUtility.ToJson(levelModel));
        }
    }
}