using CurrentGame.GameFlow;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Button = UnityEngine.UI.Button;

namespace CurrentGame.Helpers
{
    public class ClickEventDispatcher : MonoBehaviour
    {
        [SerializeField] private EventId eventId;
        
        [Inject] private EventManager eventManager;

        private Button button;
        private Toggle toggle;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
            
            toggle = GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        }

        private void OnToggleValueChanged(bool arg0)
        {
            eventManager.TriggerEvent(eventId);
        }

        private void OnButtonClicked()
        {
            eventManager.TriggerEvent(eventId);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClicked);
            }
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }
    }
}