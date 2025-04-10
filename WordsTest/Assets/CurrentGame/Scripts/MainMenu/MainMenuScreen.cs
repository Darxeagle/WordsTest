using CurrentGame.GameFlow;
using CurrentGame.Options;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CurrentGame.MainMenu
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        
        [Inject] private GameController gameController;
        [Inject] private OptionsScreen optionsScreen;
        
        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }
        
        private void OnPlayButtonClicked()
        {
            gameController.StartLevel();
        }
        
        private void OnOptionsButtonClicked()
        {
            gameController.ToOptions(true);
        }
        
        private void OnDestroy()
        {
            if (playButton != null)
                playButton.onClick.RemoveListener(OnPlayButtonClicked);
            if (optionsButton != null)
                optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        }
    }
}