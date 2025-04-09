using System.Collections.Generic;
using CurrentGame.Gameplay.Models;
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
        [SerializeField] private float height = 4f;
        [SerializeField] private float clusterSpacing = 2f;
        [SerializeField] private float scrollSpeed = 1f;
        [SerializeField] private float insertionOffset = 1f; // How far clusters move aside on insertion
        [SerializeField] private float insertionInertiaMultiplier = 1.5f; // Makes target position a bit further for inertia
        [SerializeField] private PaletteClusterView paletteClusterPrefab;

        private Dictionary<Cluster, PaletteClusterView> paletteClusters = new();
        private Vector2 lastDragPosition;
        private float scrollOffset = 0f;
        private bool isDragging = false;
        private Camera mainCamera;
        private LevelModel levelModel;

        public IReadOnlyDictionary<Cluster, PaletteClusterView> PaletteClusters => paletteClusters;
        

        private void Awake()
        {
            mainCamera = Camera.main;
            if (scrollContainer == null)
            {
                scrollContainer = transform.Find("ScrollContainer");
                if (scrollContainer == null)
                {
                    GameObject container = new GameObject("ScrollContainer");
                    scrollContainer = container.transform;
                    scrollContainer.SetParent(transform, false);
                }
            }
        }
        
        public void Initialize(LevelModel model)
        {
            levelModel = model;
            CreateClusters(model.paletteClusters);
        }
        
        private void CreateClusters(List<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                var clusterView = levelView.CreateClusterView(cluster);
                var paletteClusterView = Instantiate(paletteClusterPrefab, clustersContainer);
                paletteClusterView.SetClusterView(clusterView);
                paletteClusters.Add(cluster, paletteClusterView);
            }

            UpdateClusterPositions();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            lastDragPosition = GetPointerWorldPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            Vector2 currentPosition = GetPointerWorldPosition(eventData);
            float delta = currentPosition.x - lastDragPosition.x;
            
            if (Mathf.Abs(delta) > DRAG_THRESHOLD)
            {
                scrollOffset += delta * scrollSpeed;
                scrollContainer.localPosition = new Vector3(scrollOffset, 0f, 0f);
                lastDragPosition = currentPosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
        }

        private Vector2 GetPointerWorldPosition(PointerEventData eventData)
        {
            Vector3 mousePos = eventData.position;
            mousePos.z = -mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(mousePos);
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
            float currentX = 0f;
            for (int i = 0; i < levelModel.paletteClusters.Count; i++)
            {
                var cluster = levelModel.paletteClusters[i];
                var clusterView = paletteClusters[cluster].GetClusterView();
                if (clusterView == null) continue;
                
                float clusterWidth = clusterView.GetWidth();
                float targetX = currentX + clusterWidth / 2f; // Center of the cluster
                paletteClusters[cluster].transform.localPosition = new Vector3(targetX, 0f, 0f);
                
                currentX += clusterWidth + clusterSpacing;
            }
        }

        public void RemoveCluster(Cluster cluster)
        {
            paletteClusters.TryGetValue(cluster, out var paletteClusterView);
            if (paletteClusterView != null)
            {
                Destroy(paletteClusterView.gameObject);
                paletteClusters.Remove(cluster);
            }
            
            UpdateClusterPositions();
        }

        public void AddCluster(Cluster cluster, ClusterView clusterView)
        {
            if (paletteClusters.ContainsKey(cluster)) return;
            
            var paletteClusterView = Instantiate(paletteClusterPrefab, clustersContainer);
            paletteClusterView.SetClusterView(clusterView);
            paletteClusters.Add(cluster, paletteClusterView);
            
            UpdateClusterPositions();
            MoveClusterToPanel(cluster);
        }

        public void Clear()
        {
            foreach (var clusterView in paletteClusters.Values)
            {
                Destroy(clusterView.gameObject);
            }
            paletteClusters.Clear();
        }

        public void MoveClusterToPanel(Cluster cluster)
        {
            if (!paletteClusters.ContainsKey(cluster)) return;
            var clusterView = paletteClusters[cluster].GetClusterView();
            clusterView.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(DG.Tweening.Ease.OutBack);
        }
    }
}