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
            if (remoteConfigManager == null)
            {
                Debug.LogError("RemoteConfigManager is null in LevelsDataManager.");
                return;
            }
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
                if (levelData == null)
                {
                    Debug.LogError("Deserialized LevelData is null from level0.");
                    return;
                }
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
            if (remoteConfigManager == null)
            {
                Debug.LogError("RemoteConfigManage is null in HandleRemoteDataLoaded.");
                return;
            }
            
            if (remoteConfigManager.LevelsConfig.levels == null)
            {
                Debug.LogError("LevelsConfig.levels is null in HandleRemoteDataLoaded.");
                return;
            }

            if (remoteConfigManager.LevelsConfig.levels.Count > 0)
            {
                levelsData = remoteConfigManager.LevelsConfig.levels;
            }
            else
            {
                Debug.LogError("LevelsConfig is empty.");
            }
        }

        public LevelData GetLevelByIndex(int index)
        {
            if (levelsData.Count == 0)
            {
                Debug.LogError("LevelsData is empty.");
                return null;
            }
            
            if (index < 0)
            {
                Debug.LogError($"Index {index} is negative.");
                index = 0;
            }
            
            return levelsData[index % levelsData.Count];
        }
    }
}
