using System;
using System.Collections.Generic;
using CurrentGame.Gameplay.Models;
using CurrentGame.Helpers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CurrentGame.Gameplay.Views
{
    public class PaletteView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const float DRAG_THRESHOLD = 0.1f;
        
        
        [SerializeField] private LevelView levelView;
        [SerializeField] private Transform scrollContainer;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private Transform clustersContainer;
        [SerializeField] private PaletteClusterView paletteClusterPrefab;

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
            
            if (Mathf.Abs(delta) > DRAG_THRESHOLD)
            {
                scrollContainer.localPosition += Vector3.right * delta;
                lastDragPosition = currentPosition;
            }
            
            ClampScrollPosition();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
        }

        /*
        public void ClusterInserted(int index)
        {
            if (index < 0 || index > paletteClusters.Count) return;

            float currentX = 0f;
            // Calculate positions for all clusters
            for (int i = 0; i < paletteClusters.Count; i++)
            {
                var clusterView = paletteClusters[i].GetClusterView();
                if (clusterView == null) continue;

                float clusterWidth = clusterView.GetWidth();
                float targetX = currentX + clusterWidth / 2f;

                if (i >= index)
                {
                    // For clusters after insertion point, add offset that diminishes with distance
                    float insertionShift = insertionOffset * Mathf.Max(0, 1f - (i - index) * 0.5f);
                    
                    // Current position moves aside immediately
                    paletteClusters[i].SetOffset(new Vector3(targetX + insertionShift, 0f, 0f));
                    
                    // Target position is a bit further for inertia feeling
                    float targetShift = insertionShift * insertionInertiaMultiplier;
                    paletteClusters[i].SetTargetOffset(new Vector3(targetX, 0f, 0f));
                }
                else
                {
                    // Clusters before insertion point keep their positions
                    paletteClusters[i].transform.localPosition = new Vector3(targetX, 0f, 0f);
                }
                
                currentX += clusterWidth + clusterSpacing;
            }
        }
        */

        /*
        public void ClusterRemoved(int index)
        {
            if (index < 0 || index >= paletteClusters.Count) return;

            float currentX = 0f;
            // Calculate positions for all clusters
            for (int i = 0; i < paletteClusters.Count; i++)
            {
                var clusterView = paletteClusters[i].GetClusterView();
                if (clusterView == null) continue;

                float clusterWidth = clusterView.GetWidth();
                float targetX = currentX + clusterWidth / 2f;

                if (i >= index)
                {
                    // For clusters after removal point, add offset that diminishes with distance
                    float removalShift = -insertionOffset * Mathf.Max(0, 1f - (i - index) * 0.5f);
                    
                    // Current position moves aside immediately
                    paletteClusters[i].SetOffset(new Vector3(targetX + removalShift, 0f, 0f));
                    
                    // Target position is the new normal position
                    paletteClusters[i].SetTargetOffset(new Vector3(targetX, 0f, 0f));
                }
                else
                {
                    // Clusters before removal point keep their positions
                    paletteClusters[i].transform.localPosition = new Vector3(targetX, 0f, 0f);
                }
                
                currentX += clusterWidth + clusterSpacing;
            }
        }
        */

        private void UpdateClusterPositions()
        {
            float currentX = 0.5f;
            
            for (int i = 0; i < levelModel.PaletteClusters.Count; i++) 
            {
                var cluster = levelModel.PaletteClusters[i];
                var clusterView = paletteClusters[cluster].GetClusterView();
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
            scrollContainer.localPosition = new Vector3(
                Mathf.Clamp(scrollContainer.localPosition.x, -xMax+screenBounds.width/2f, -screenBounds.width/2f), 
                0f, 0f);
        }
        
        public void AddCluster(Cluster cluster, ClusterView clusterView)
        {
            if (paletteClusters.ContainsKey(cluster)) return;

            Vector3 clusterPosition = clusterView.transform.position;
            scrollContainer.localPosition += Vector3.left * (clusterView.GetWidth()/2f + LevelView.CLUSTER_SPACING);
            
            var paletteClusterView = Instantiate(paletteClusterPrefab, clustersContainer);
            paletteClusterView.SetClusterView(clusterView);
            paletteClusters.Add(cluster, paletteClusterView);
            UpdateClusterPositions();
            
            MoveClusterToPanel(cluster, clusterPosition);
        }

        public void RemoveCluster(Cluster cluster)
        {
            paletteClusters.TryGetValue(cluster, out var paletteClusterView);
            if (paletteClusterView != null)
            {
                Destroy(paletteClusterView.gameObject);
                paletteClusters.Remove(cluster);
                
                scrollContainer.localPosition += Vector3.right * (paletteClusterView.GetClusterView().GetWidth()/2f + LevelView.CLUSTER_SPACING);
            }
            
            UpdateClusterPositions();
        }
        
        public int GetInsertIndex(Vector3 position)
        {
            int i = 0;
            float lastX = -10f;

            foreach (var paletteCluster in paletteClusters.Values)
            {
                var clusterPosition = paletteCluster.transform.localPosition + scrollContainer.localPosition;
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
        
        public void ReturnClusterToPanel(Cluster cluster)
        {
            if (!paletteClusters.ContainsKey(cluster)) return;
            var clusterView = paletteClusters[cluster].GetClusterView();
            clusterView.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutCirc);
        }
        
        private void MoveClusterToPanel(Cluster cluster, Vector3 fromPosition)
        {
            if (!paletteClusters.ContainsKey(cluster)) return;
            var clusterView = paletteClusters[cluster].GetClusterView();
            clusterView.transform.DOMove(paletteClusters[cluster].transform.position, 0.2f).From(fromPosition).SetEase(Ease.OutCirc);
        }
    }
}