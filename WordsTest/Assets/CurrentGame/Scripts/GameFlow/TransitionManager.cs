using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

namespace CurrentGame.GameFlow
{
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup curtainGroup;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Ease fadeEase = Ease.InOutSine;

        private void Awake()
        {
            curtainGroup.alpha = 0f;
            curtainGroup.blocksRaycasts = false;
            curtainGroup.gameObject.SetActive(true);
        }
        
        public async UniTask OpeningTransitionAsync(Action action = null)
        {
            action?.Invoke();
            await FadeCurtainOut(500);
        }

        public async UniTask TransitionAsync(Action action = null)
        {
            await FadeCurtainIn();
            action?.Invoke();
            await FadeCurtainOut();
        }

        private async UniTask FadeCurtainIn(int delay = 0)
        {
            curtainGroup.alpha = 0f;
            curtainGroup.blocksRaycasts = true;
            await UniTask.Delay(delay);
            await curtainGroup.DOFade(1f, fadeDuration)
                .SetEase(fadeEase)
                .AsyncWaitForCompletion();
        }

        private async UniTask FadeCurtainOut(int delay = 0)
        {
            curtainGroup.blocksRaycasts = true;
            curtainGroup.alpha = 1f;
            await UniTask.Delay(delay);
            await curtainGroup.DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .AsyncWaitForCompletion();
            curtainGroup.blocksRaycasts = false;
        }
    }
}