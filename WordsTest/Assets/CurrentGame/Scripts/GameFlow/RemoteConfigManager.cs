using System;
using System.Collections.Generic;
using CurrentGame.Gameplay.Models;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
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
                
                if (AuthenticationService.Instance == null)
                {
                    Debug.LogError("AuthenticationService.Instance is null");
                    return;
                }
                
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                
                if (RemoteConfigService.Instance == null)
                {
                    Debug.LogError("RemoteConfigService.Instance is null");
                    return;
                }
                
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
            if (RemoteConfigService.Instance == null)
            {
                Debug.LogError("RemoteConfigService.Instance is null during fetch");
                return;
            }
            
            if (configResponse.status != ConfigRequestStatus.Success)
            {
                Debug.LogError($"Remote Config fetch failed: {configResponse.requestOrigin}");
                return;
            }
            
            if (configResponse.requestOrigin == ConfigOrigin.Default)
            {
                Debug.Log("Remote Config fetched from default origin");
                return;
            }
            
            if (configResponse.requestOrigin == ConfigOrigin.Remote)
            {
                if (RemoteConfigService.Instance.appConfig == null)
                {
                    Debug.LogError("Remote Config appConfig is null");
                    return;
                }
                
                var levelsJson = RemoteConfigService.Instance.appConfig.GetJson("levels");
                if (string.IsNullOrEmpty(levelsJson))
                {
                    Debug.LogError("Remote Config levels JSON is empty");
                    return;
                }

                LevelsConfig levelsConfig;
                try
                {
                    levelsConfig = JsonUtility.FromJson<LevelsConfig>(levelsJson);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse levels JSON: {e.Message}");
                    throw;
                }
                
                if (levelsConfig == null || levelsConfig.levels == null || levelsConfig.levels.Count == 0)
                {
                    Debug.LogError("Remote Config levels JSON is invalid");
                    return;
                }
                
                LevelsConfig = levelsConfig;
                OnRemoteDataLoaded?.Invoke();
            }
        }
    }
    
    public class LevelsConfig
    {
        public List<LevelData> levels;
    }
}
