using System.Collections.Generic;
using CurrentGame.Gameplay.Models;
using UnityEngine;
using Zenject;

namespace CurrentGame.GameFlow
{
    public class LevelsDataManager : IInitializable
    {
        [Inject] private RemoteConfigManager remoteConfigManager;

        private Dictionary<int, LevelData> levelsData = new();
            if (jsonAsset != null)
            {
                var levelData = JsonUtility.FromJson<LevelData>(jsonAsset.text);
                levelsData.Add(levelData.id, levelData);
            }
        
        public void Initialize()
        {
            LoadFirstLevel();
            remoteConfigManager.OnRemoteDataLoaded += HandleRemoteDataLoaded;
        }

        private void LoadFirstLevel()
        {
            var jsonAsset = Resources.Load<TextAsset>("level0");
            else
            {
                Debug.LogError("Failed to load level0.json from Resources");
            }
        }

        private void HandleRemoteDataLoaded()
        {
            if (string.IsNullOrEmpty(remoteConfigManager.LevelsJson))
                return;

            var levelDatas = JsonUtility.FromJson<List<LevelData>>(remoteConfigManager.LevelsJson);
            foreach (var level in levelDatas)
            {
                if (!levelsData.ContainsKey(level.id))
                {
                    levelsData.Add(level.id, level);
                }
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