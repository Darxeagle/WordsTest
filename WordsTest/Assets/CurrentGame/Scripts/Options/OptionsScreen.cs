using System.Runtime.InteropServices;
using CurrentGame.GameFlow;
using CurrentGame.Sounds;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CurrentGame.Options
{
    public class OptionsScreen : MonoBehaviour, IInitializable
    {
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button mainMenuButton;
        
        [Inject] private GameModel gameModel;
        [Inject] private GameController gameController;
        [Inject] private EventManager eventManager;
        
        private bool fromMenu;
        
        
        public void Initialize()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            
            soundToggle.SetIsOnWithoutNotify(gameModel.SoundEnabled);
            musicToggle.SetIsOnWithoutNotify(gameModel.MusicEnabled);
            eventManager.EventBus.Where(e => e == EventId.ModelUpdated).Subscribe(OnModelUpdated);
        }

        private void OnModelUpdated(EventId e)
        {
            soundToggle.SetIsOnWithoutNotify(gameModel.SoundEnabled);
            musicToggle.SetIsOnWithoutNotify(gameModel.MusicEnabled);
        }

        public void FromMenu(bool state)
        {
            fromMenu = state;
            mainMenuButton.gameObject.SetActive(!state);
        }
        
        private void OnSoundToggleChanged(bool isOn)
        {
            gameModel.SoundEnabled = isOn;
        }
        
        private void OnMusicToggleChanged(bool isOn)
        {
            gameModel.MusicEnabled = isOn;
        }
        
        private void OnCloseButtonClicked()
        {
            gameController.CloseOptions(fromMenu);
        }
        
        private void OnMainMenuButtonClicked()
        {
            gameController.ToMainMenu(true);
        }
        
        private void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
            if (soundToggle != null)
                soundToggle.onValueChanged.RemoveListener(OnSoundToggleChanged);
            if (musicToggle != null)
                musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
        }
    }
}