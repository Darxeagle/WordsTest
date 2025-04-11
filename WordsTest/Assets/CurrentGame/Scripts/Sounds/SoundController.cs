using System;
using CurrentGame.GameFlow;
using UniRx;
using UnityEngine;
using Zenject;

namespace CurrentGame.Sounds
{
    public class SoundController : MonoBehaviour, IInitializable
    {
        [SerializeField] private AudioSource audioSource;
        
        [Inject] private GameModel gameModel;
        [Inject] private EventManager eventManager;
        [Inject] private SoundsConfig soundsConfig;
        
        public void Initialize()
        {
            audioSource = GetComponent<AudioSource>();
            eventManager.EventBus.Subscribe(OnEvent).AddTo(this);
        }

        public void PlaySound(SoundId soundId)
        {
            if (!gameModel.SoundEnabled) return;
            
            var clip = soundsConfig.GetSound(soundId);
            audioSource.PlayOneShot(clip);
        }
        
        private void OnEvent(EventId eventId)
        {
            switch (eventId)
            {
                case EventId.ButtonClicked:
                    PlaySound(SoundId.Click);
                    break;  
                case EventId.SwitchClicked:
                    PlaySound(SoundId.Switch);
                    break;
                case EventId.ClusterDragBegin:
                    PlaySound(SoundId.Take);
                    break;
                case EventId.ClusterDragEnd:
                    PlaySound(SoundId.Put);
                    break;
                case EventId.CheckStarted:
                    PlaySound(SoundId.Check);
                    break;
                case EventId.CheckCompleted:
                    PlaySound(SoundId.CheckWin);
                    break;
                case EventId.CheckFailed:
                    PlaySound(SoundId.CheckFail);
                    break;
                case EventId.LevelStarted:
                    PlaySound(SoundId.LevelStart);
                    break;
            }
        }
    }
}