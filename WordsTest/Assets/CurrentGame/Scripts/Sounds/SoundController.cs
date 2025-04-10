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
        
        public bool SoundEnabled { get; private set; } = true;
        
        
        public void Initialize()
        {
            audioSource = GetComponent<AudioSource>();
            
            eventManager.EventBus.Where(e => e == EventManager.buttonClicked).Subscribe(OnButtonClicked).AddTo(this);
            
            SetSoundEnabled(gameModel.MusicEnabled);
        }
        
        public void SetSoundEnabled(bool enabled)
        {
            SoundEnabled = enabled;
            gameModel.SoundEnabled = enabled;
        }

        public void PlaySound(SoundId soundId)
        {
            if (!SoundEnabled) return;
            
            var clip = soundsConfig.GetSound(soundId);
            audioSource.PlayOneShot(clip);
        }
        
        private void OnButtonClicked(string e)
        {
            PlaySound(SoundId.Click);
        }
    }
}