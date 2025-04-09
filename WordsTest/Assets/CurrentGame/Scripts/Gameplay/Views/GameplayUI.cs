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
            // Handle options button click
            Debug.Log("Options button clicked");
        }
    }
}