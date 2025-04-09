using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace CurrentGame.GameFlow
{
    public class RemoteConfigManager
    {
        private struct UserAttributes { }
        private struct AppAttributes { }

        public event Action OnRemoteDataLoaded;
        public string LevelsJson { get; private set; }
        
        public async Task Initialize()
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
                LevelsJson = RemoteConfigService.Instance.appConfig.GetJson("levels");
                OnRemoteDataLoaded?.Invoke();
            }
        }
    }
}