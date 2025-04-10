using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Controllers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CurrentGame.Gameplay.Views
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Button completeButton;
        [SerializeField] private Button optionsButton;
        
        [Inject] private LevelController levelController;
        [Inject] private GameController gameController;
        
        private void Awake()
        {
            completeButton.onClick.AddListener(OnCompleteButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }
        
        private void OnCompleteButtonClicked()
        {
            levelController.CheckCompleted();
        }
        
        private void OnOptionsButtonClicked()
        {
            gameController.ToOptions(false);
        }
    }
}