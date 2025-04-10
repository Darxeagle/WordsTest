using System;
using System.Collections.Generic;
using CurrentGame.Gameplay.Models;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Zenject;

namespace CurrentGame.GameFlow
{
    public class RemoteConfigManager : IInitializable
    {
        private struct UserAttributes { }
        private struct AppAttributes { }

        public event Action OnRemoteDataLoaded;
        public LevelsConfig LevelsConfig { get; private set; }
        
        public void Initialize()
        {
            InitializeAsync().Forget();
        }
        
        private async UniTask InitializeAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                
                RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetched;
                await RemoteConfigService.Instance.FetchConfigsAsync(
                    new UserAttributes(), 
                    new AppAttributes()
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Remote Config initialization failed: {e.Message}");
            }
        }

        private void OnRemoteConfigFetched(ConfigResponse configResponse)
        {
            if (configResponse.requestOrigin == ConfigOrigin.Remote)
            {
                LevelsConfig = JsonUtility.FromJson<LevelsConfig>(RemoteConfigService.Instance.appConfig.GetJson("levels"));
                OnRemoteDataLoaded?.Invoke();
            }
        }
    }
    
    public class LevelsConfig
    {
        public List<LevelData> levels;
    }
}