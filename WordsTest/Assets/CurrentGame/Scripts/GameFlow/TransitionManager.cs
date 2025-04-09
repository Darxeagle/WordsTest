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

        public async UniTask TransitionAsync(Action action)
        {
            await FadeCurtainIn();
            
            action?.Invoke();
            
            await FadeCurtainOut();
        }

        private async UniTask FadeCurtainIn()
        {
            curtainGroup.blocksRaycasts = true;
            await curtainGroup.DOFade(1f, fadeDuration)
                .SetEase(fadeEase)
                .AsyncWaitForCompletion();
        }

        private async UniTask FadeCurtainOut()
        {
            await curtainGroup.DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .AsyncWaitForCompletion();
            curtainGroup.blocksRaycasts = false;
        }
    }
}