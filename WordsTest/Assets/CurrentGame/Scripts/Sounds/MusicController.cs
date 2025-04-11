using System;
using System.Resources;
using CurrentGame.GameFlow;
using UniRx;
using UnityEngine;
using Zenject;

namespace CurrentGame.Sounds
{
    public class MusicController : MonoBehaviour, IInitializable
    {
        [SerializeField] private AudioSource audioSource;
        
        [Inject] private GameModel gameModel;
        [Inject] private EventManager eventManager;
        
        public void Initialize()
        {
            audioSource = GetComponent<AudioSource>();
            SetMusicEnabled(gameModel.MusicEnabled);
            eventManager.EventBus.Where(e => e == EventId.ModelUpdated).Subscribe(OnModelUpdated);
        }

        private void OnModelUpdated(EventId e)
        {
            SetMusicEnabled(gameModel.MusicEnabled);
        }

        private void SetMusicEnabled(bool enabled)
        {
            if (enabled && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else if (!enabled && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            gameModel.MusicEnabled = enabled;
        }
    }
}