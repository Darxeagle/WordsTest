using UnityEngine;
using System.Collections.Generic;
using WordsTest.Gameplay.Models;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace WordsTest.Gameplay
{
    public class LevelView : MonoBehaviour
    {
        public static float LETTER_SIZE = 1.3f;
        public static float LETTER_DISTANCE = 1.5f;
        public static float WORD_SPACING = 1.5f;
        public static float LETTER_SPACING = 1.5f;
        public static float CLUSTER_SPACING = 1f;
        public static float PALETTE_HEIGHT = 4f;

        [SerializeField] private Transform cellsContainer;
        [SerializeField] private ClusterView clusterPrefab;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private PaletteView paletteView;
        
        public event Action<ClusterView, Cluster, Vector3> OnClusterDragBegin;
        public event Action<ClusterView, Cluster, Vector3> OnClusterDragProgress;
        public event Action<ClusterView, Cluster, Vector3> OnClusterDragEnd;

        private LevelModel levelModel;
        private List<GameObject> cells = new();
        private Dictionary<PlacedCluster, ClusterView> placedClusters = new();
        
        public IReadOnlyDictionary<PlacedCluster, ClusterView> PlacedClusters => placedClusters;

        public void Initialize(LevelModel model)
        {
            levelModel = model;
            Clear();
            CreateCells(model.words);
            CreatePlacedClusterViews(model.placedClusters);
            paletteView.Initialize(model);
        }

        private void CreateCells(List<Word> words)
        {
            for (int i = 0; i < words.Count; i++)
            {
                var word = words[i];
                for (int j = 0; j < word.length; j++)
                {
                    var cellView = Instantiate(cellPrefab, cellsContainer);
                    cellView.transform.position = GetLetterPosition(new LetterPosition(i, j));
                    cells.Add(cellView);
                }
            }
        }
        
        private void CreatePlacedClusterViews(List<PlacedCluster> clusters)
        {
            foreach (var placedCluster in clusters)
            {
                var clusterView = CreateClusterView(placedCluster.cluster);
                clusterView.transform.SetParent(cellsContainer, false);
                clusterView.transform.localPosition = GetLetterPosition(placedCluster.position);
                placedClusters.Add(placedCluster, clusterView);
            }
        }

        public ClusterView CreateClusterView(Cluster cluster)
        {
            var clusterView = Instantiate(clusterPrefab);
            clusterView.Initialize(cluster);
            clusterView.OnDragBegin += HandleClusterDragBegin;
            clusterView.OnDragProgress += HandleClusterDragProgress;
            clusterView.OnDragEnd += HandleClusterDragEnd;
            return clusterView;
        }

        private void HandleClusterDragBegin(ClusterView view, Cluster cluster, Vector3 position)
        {
            OnClusterDragBegin?.Invoke(view, cluster, position);
        }

        private void HandleClusterDragProgress(ClusterView view, Cluster cluster, Vector3 position)
        {
            OnClusterDragProgress?.Invoke(view, cluster, position);
        }

        private void HandleClusterDragEnd(ClusterView view, Cluster cluster, Vector3 position)
        {
            OnClusterDragEnd?.Invoke(view, cluster, position);
        }

        public Vector3 GetLetterPosition(LetterPosition letterPosition)
        {
            return cellsContainer.position + new Vector3(
                -(levelModel.words[letterPosition.wordIndex].length - 1) * LETTER_SPACING / 2f +
                LETTER_SPACING * letterPosition.charIndex,
                (levelModel.words.Count - 1) * WORD_SPACING / 2f - WORD_SPACING * letterPosition.wordIndex,
                0f);
        }

        public LetterPosition GetNearestLetter(Vector3 position)
        {
            float minDistance = 2f*2f;
            LetterPosition nearestLetter = null;

            for (int i = 0; i < levelModel.words.Count; i++)
            {
                var word = levelModel.words[i];
                for (int j = 0; j < word.length; j++)
                {
                    var letterPosition = GetLetterPosition(new LetterPosition(i, j));
                    float distance = (letterPosition - position).sqrMagnitude;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestLetter = new LetterPosition(i, j);
                    }
                }
            }
            
            return nearestLetter;
        }
        
        public bool IsPositionOnPalette(Vector3 position)
        {
            return position.y < paletteView.transform.position.y + PALETTE_HEIGHT / 2f;
        }

        public async UniTask PlaceCluster(ClusterView clusterView, PlacedCluster placedCluster)
        {
            clusterView.transform.SetParent(cellsContainer);
            placedClusters.Add(placedCluster, clusterView);
            await MoveClusterToWord(clusterView, GetLetterPosition(placedCluster.position));
        }

        public async UniTask MoveClusterToWord(ClusterView clusterView, Vector3 targetPosition)
        {
            clusterView.transform.SetParent(cellsContainer);
            await clusterView.transform.DOMove(targetPosition, 0.2f).SetEase(DG.Tweening.Ease.OutBack)
                .AsyncWaitForCompletion();
        }

        private void Clear()
        {
            foreach (var cellView in cells)
            {
                Destroy(cellView);
            }
            cells.Clear();
            
            foreach (var clusterView in placedClusters.Values)
            {
                if (clusterView != null)
                {
                    clusterView.OnDragBegin -= HandleClusterDragBegin;
                    clusterView.OnDragProgress -= HandleClusterDragProgress;
                    clusterView.OnDragEnd -= HandleClusterDragEnd;
                    Destroy(clusterView.gameObject);
                }
            }
            placedClusters.Clear();

            paletteView.Clear();
        }
    }
}