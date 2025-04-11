using System;
using UnityEngine;
using Zenject;

namespace CurrentGame.GameFlow
{
    [Serializable]
    public class GameModel
    {
        [Inject] private EventManager eventManager;
        
        [SerializeField]private int currentLevel;
        [SerializeField] private bool musicEnabled = true;
        [SerializeField] private bool soundEnabled = true;
        
        public void Apply(GameModel model)
        {
            currentLevel = model.currentLevel;
            MusicEnabled = model.musicEnabled;
            SoundEnabled = model.soundEnabled;
            
            eventManager.TriggerEvent(EventId.ModelUpdated);
        }

        public int CurrentLevel
        {
            get => currentLevel;
            set
            {
                if (currentLevel != value)
                {
                    currentLevel = value;
                    eventManager.TriggerEvent(EventId.ModelUpdated);
                }
            }
        }
        
        public bool MusicEnabled
        {
            get => musicEnabled;
            set
            {
                if (musicEnabled != value)
                {
                    musicEnabled = value;
                    eventManager.TriggerEvent(EventId.ModelUpdated);
                }
            }
        }
        
        public bool SoundEnabled
        {
            get => soundEnabled;
            set
            {
                if (soundEnabled != value)
                {
                    soundEnabled = value;
                    eventManager.TriggerEvent(EventId.ModelUpdated);
                }
            }
        }
    }
}