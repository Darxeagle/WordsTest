using System.Collections.Generic;
using CurrentGame.Gameplay.Models;
using CurrentGame.Helpers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CurrentGame.Gameplay.Views
{
    public class PaletteView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private LevelView levelView;
        [SerializeField] private Transform scrollContainer;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private Transform clustersContainer;
        [SerializeField] private PaletteClusterView paletteClusterPrefab;
        
        [Inject] private ScreenAdjuster screenAdjuster;

        private Dictionary<Cluster, PaletteClusterView> paletteClusters = new();
        private Vector2 lastDragPosition;
        private bool isDragging = false;
        private LevelModel levelModel;
        private float xMax;
        private Rect screenBounds;

        public IReadOnlyDictionary<Cluster, PaletteClusterView> PaletteClusters => paletteClusters;
        
        public void Initialize(LevelModel model)
        {
            levelModel = model;
            screenBounds = PositionHelper.GetScreenBounds();
            CreateClusters(model.PaletteClusters);
        }
        
        private void CreateClusters(IReadOnlyList<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                var clusterView = levelView.CreateClusterView(cluster);
                var paletteClusterView = Instantiate(paletteClusterPrefab, clustersContainer);
                paletteClusterView.SetClusterView(clusterView);
                clusterView.transform.localPosition = Vector3.zero;
                paletteClusters.Add(cluster, paletteClusterView);
            }

            UpdateClusterPositions();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            lastDragPosition = PositionHelper.ScreenToWorld(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            Vector2 currentPosition = PositionHelper.ScreenToWorld(eventData.position);
            float delta = currentPosition.x - lastDragPosition.x;
            scrollContainer.localPosition += Vector3.right * delta;
            lastDragPosition = currentPosition;
            
            ClampScrollPosition();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
        }

        private void UpdateClusterPositions()
        {
            float currentX = 0.5f;
            
            for (int i = 0; i < levelModel.PaletteClusters.Count; i++) 
            {
                var cluster = levelModel.PaletteClusters[i];
                var clusterView = paletteClusters[cluster].ClusterView;
                if (clusterView == null) continue;
                
                float clusterWidth = clusterView.GetWidth();
                paletteClusters[cluster].transform.localPosition = new Vector3(currentX + LevelView.LETTER_SIZE/2f + LevelView.FRAME_PADDING, 0f, 0f);

                currentX += clusterWidth + LevelView.FRAME_PADDING * 2f + LevelView.CLUSTER_SPACING;
            }

            xMax = currentX - LevelView.CLUSTER_SPACING + 0.5f;

            ClampScrollPosition();
        }

        private void ClampScrollPosition()
        {
            if (xMax < screenAdjuster.ScreenWidth)
            {
                scrollContainer.localPosition = new Vector3(
                    -xMax/2f, 0f, 0f);
                return;
            }
            
            scrollContainer.localPosition = new Vector3(
                Mathf.Clamp(scrollContainer.localPosition.x, -xMax+screenAdjuster.ScreenWidth/2f, -screenAdjuster.ScreenWidth/2f), 
                0f, 0f);
        }
        
        public void AddCluster(Cluster cluster, ClusterView clusterView, int insertIndex)
        {
            if (paletteClusters.ContainsKey(cluster)) return;

            Vector3 clusterPosition = clusterView.transform.position;
            scrollContainer.localPosition += Vector3.left * (clusterView.GetWidth()/2f + LevelView.CLUSTER_SPACING);
            
            var paletteClusterView = Instantiate(paletteClusterPrefab, clustersContainer);
            paletteClusterView.SetClusterView(clusterView);
            paletteClusters.Add(cluster, paletteClusterView);
            UpdateClusterPositions();
            
            AnimateClusterMoveToPanel(cluster, clusterPosition).Forget();
            AnimateClustersPushOut(insertIndex).Forget();
        }

        public void RemoveCluster(Cluster cluster, int removeIndex)
        {
            paletteClusters.TryGetValue(cluster, out var paletteClusterView);
            if (paletteClusterView != null)
            {
                Destroy(paletteClusterView.gameObject);
                paletteClusters.Remove(cluster);
                
                scrollContainer.localPosition += Vector3.right * (paletteClusterView.ClusterView.GetWidth()/2f + LevelView.CLUSTER_SPACING);
                
                UpdateClusterPositions();
                
                AnimateClustersPushIn(removeIndex).Forget();
            }
        }
        
        public int GetInsertIndex(Vector3 position)
        {
            int i = 0;
            float lastX = -10f;

            foreach (var paletteCluster in levelModel.PaletteClusters)
            {
                var view = paletteClusters[paletteCluster];
                var clusterPosition = view.transform.localPosition + scrollContainer.localPosition;
                if (position.x > lastX && position.x < clusterPosition.x)
                {
                    break;
                }
                
                lastX = clusterPosition.x;
                i++;
            }

            return i;
        }

        public void Clear()
        {
            foreach (var clusterView in paletteClusters.Values)
            {
                Destroy(clusterView.gameObject);
            }
            paletteClusters.Clear();
        }
        
        public async UniTask AnimateClusterReturnToPanel(Cluster cluster)
        {
            if (!paletteClusters.ContainsKey(cluster)) return;
            var clusterView = paletteClusters[cluster].ClusterView;
            await clusterView.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutSine).AsyncWaitForCompletion();
        }
        
        private async UniTask AnimateClusterMoveToPanel(Cluster cluster, Vector3 fromPosition)
        {
            if (!paletteClusters.ContainsKey(cluster)) return;
            var clusterView = paletteClusters[cluster].ClusterView;
            await clusterView.transform.DOMove(paletteClusters[cluster].transform.position, 0.2f).From(fromPosition)
                .SetEase(Ease.OutCirc).AsyncWaitForCompletion();
        }
        
        private async UniTask AnimateClustersPushOut(int insertIndex)
        {
            var concurrentUnitask = new List<UniTask>();
            
            for (int i = 0; i < levelModel.PaletteClusters.Count; i++)
            {
                if (i < insertIndex) concurrentUnitask.Add(paletteClusters[levelModel.PaletteClusters[i]].AnimateFromRight());
                else if (i > insertIndex) concurrentUnitask.Add(paletteClusters[levelModel.PaletteClusters[i]].AnimateFromLeft());
            }
            
            await UniTask.WhenAll(concurrentUnitask);
        }

        private async UniTask AnimateClustersPushIn(int removeIndex)
        {
            var concurrentUnitask = new List<UniTask>();

            for (int i = 0; i < levelModel.PaletteClusters.Count; i++)
            {
                if (i < removeIndex) concurrentUnitask.Add(paletteClusters[levelModel.PaletteClusters[i]].AnimateFromLeft(true));
                else if (i >= removeIndex) concurrentUnitask.Add(paletteClusters[levelModel.PaletteClusters[i]].AnimateFromRight(true));
            }

            await UniTask.WhenAll(concurrentUnitask);
        }
        
    }
}