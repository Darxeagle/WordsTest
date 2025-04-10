using CurrentGame.GameFlow;
using CurrentGame.Sounds;
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
        [Inject] private SoundController soundController;
        [Inject] private MusicController musicController;
        [Inject] private GameController gameController;
        
        private bool fromMenu;
        
        
        public void Initialize()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            
            // Initialize toggle states
            soundToggle.isOn = gameModel.SoundEnabled;
            musicToggle.isOn = gameModel.MusicEnabled;
        }
        
        public void FromMenu(bool state)
        {
            fromMenu = state;
            mainMenuButton.gameObject.SetActive(state);
        }
        
        private void OnSoundToggleChanged(bool isOn)
        {
            soundController.SetSoundEnabled(isOn);
        }
        
        private void OnMusicToggleChanged(bool isOn)
        {
            musicController.SetMusicEnabled(isOn);
        }
        
        private void OnCloseButtonClicked()
        {
            if (fromMenu)
            {
                gameController.ToMainMenu(false);
            }
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