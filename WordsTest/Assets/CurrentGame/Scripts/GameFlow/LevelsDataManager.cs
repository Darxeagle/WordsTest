using System.Collections.Generic;
using CurrentGame.Gameplay.Models;
using UnityEngine;
using Zenject;

namespace CurrentGame.GameFlow
{
    public class LevelsDataManager : IInitializable
    {
        [Inject] private RemoteConfigManager remoteConfigManager;

        private List<LevelData> levelsData = new();
        
        public void Initialize()
        {
            LoadFirstLevel();
            HandleRemoteDataLoaded();
            remoteConfigManager.OnRemoteDataLoaded += HandleRemoteDataLoaded;
        }

        private void LoadFirstLevel()
        {
            var jsonAsset = Resources.Load<TextAsset>("level0");
            if (jsonAsset != null)
            {
                var levelData = JsonUtility.FromJson<LevelData>(jsonAsset.text);
                if (levelsData.Count == 0)
                {
                    levelsData.Add(levelData);
                }
            }
            else
            {
                Debug.LogError("Failed to load level0.json from Resources");
            }
        }

        private void HandleRemoteDataLoaded()
        {
            if (remoteConfigManager.LevelsConfig == null)
            {
                return;
            }

            if (remoteConfigManager.LevelsConfig?.levels.Count > 0)
            {
                levelsData = remoteConfigManager.LevelsConfig.levels;
            }
        }

        public LevelData GetLevelByIndex(int index)
        {
            if (levelsData.Count == 0)
                return null;
            
            return levelsData[index % levelsData.Count];
        }
    }
}