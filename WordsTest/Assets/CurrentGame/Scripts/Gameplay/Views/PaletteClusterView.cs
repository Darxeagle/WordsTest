using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CurrentGame.Gameplay.Views
{
    public class PaletteClusterView : MonoBehaviour
    {
        private ClusterView clusterView;
        
        public ClusterView ClusterView => clusterView;


        private void Awake()
        {
            if (clusterView == null)
            {
                clusterView = GetComponentInChildren<ClusterView>();
            }
        }

        public void SetClusterView(ClusterView view)
        {
            clusterView = view;
            if (view != null)
            {
                view.transform.SetParent(transform, true);
            }
        }
        
        public async UniTask AnimateFromLeft(bool longer = false)
        {
            await clusterView.transform.DOLocalMoveX(0f, 0.2f).From(longer ? -1.5f : -0.5f).SetEase(Ease.OutCirc)
                .AsyncWaitForCompletion();
        }
        
        public async UniTask AnimateFromRight(bool longer = false)
        {
            await clusterView.transform.DOLocalMoveX(0f, 0.2f).From(longer ? 1.5f : 0.5f).SetEase(Ease.OutCirc)
                .AsyncWaitForCompletion();
        }
    }
}