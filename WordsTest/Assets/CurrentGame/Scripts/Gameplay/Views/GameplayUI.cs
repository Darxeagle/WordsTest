﻿using System;
using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Controllers;
using CurrentGame.Gameplay.Models;
using UniRx;
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
        [Inject] private LevelModel levelModel;
        [Inject] private EventManager eventManager;
        
        private void Awake()
        {
            completeButton.onClick.AddListener(OnCompleteButtonClicked);
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
            eventManager.EventBus.Where(e => e == EventManager.modelUpdated).Subscribe(LevelModelUpdated).AddTo(this);
            LevelModelUpdated();
        }

        private void OnCompleteButtonClicked()
        {
            levelController.CheckCompleted();
        }
        
        private void OnOptionsButtonClicked()
        {
            gameController.ToOptions(false);
        }

        private void LevelModelUpdated(string e = null)
        {
            completeButton.gameObject.SetActive(levelModel.Initialized && levelModel.PaletteClusters.Count == 0);
        }

        private void Update()
        {
            if (completeButton.IsActive())
            {
                completeButton.transform.localScale = Vector3.one * (1 + Mathf.PingPong(Time.time, 0.3f) * 0.7f);
            }
        }
    }
}