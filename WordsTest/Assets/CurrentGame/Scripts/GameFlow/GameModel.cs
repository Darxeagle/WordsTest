﻿using System;
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
            musicEnabled = model.musicEnabled;
            soundEnabled = model.soundEnabled;
        }

        public int CurrentLevel
        {
            get => currentLevel;
            set
            {
                if (currentLevel != value)
                {
                    currentLevel = value;
                    eventManager.TriggerEvent(EventManager.modelUpdated);
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
                    eventManager.TriggerEvent(EventManager.modelUpdated);
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
                    eventManager.TriggerEvent(EventManager.modelUpdated);
                }
            }
        }
    }
}