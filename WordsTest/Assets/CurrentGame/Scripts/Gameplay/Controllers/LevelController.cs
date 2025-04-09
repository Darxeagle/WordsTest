using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using WordsTest.Gameplay.Models;

namespace WordsTest.Gameplay
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelView levelView;
        [SerializeField] private PaletteView paletteView;
        
        private LevelLoader levelLoader;
        private LevelModel levelModel;

        private void Awake()
        {
            levelLoader = new LevelLoader();
        }
        
        private void Start()
        {
            var levelData = new LevelData();
            levelData.words = new List<string> { "banana", "cherry", "orange", "papaya", "grapes", "tomato" };
            levelData.clusters = new List<string> { "ban", "ana", "cher", "ry", "or", "ange", "pap", "aya", "gr", "apes", "toma", "to" };
            levelModel = levelLoader.CreateModel(levelData);
            InitializeGame();
        }

        public void LoadLevel(string levelId)
        {
            var levelData = levelLoader.LoadLevel(levelId);
            if (levelData != null)
            {
                levelModel = levelLoader.CreateModel(levelData);
                InitializeGame();
            }
        }

        private void InitializeGame()
        {
            levelView.Initialize(levelModel);
        }

        public List<Word> GetWords()
        {
            return levelModel.words;
        }

        public bool CheckPlacement(Cluster cluster, LetterPosition letterPosition)
        {
            var word = levelModel.words[letterPosition.wordIndex];
            
            if (cluster == null || letterPosition.wordIndex < 0 || letterPosition.charIndex + cluster.length > word.length)
                return false;

            // Check if position is already occupied
            return !levelModel.placedClusters.Any(pc => pc.cluster != cluster &&
                pc.position.wordIndex == letterPosition.wordIndex && 
                (pc.position.charIndex + pc.cluster.length > letterPosition.charIndex && 
                 pc.position.charIndex < letterPosition.charIndex + cluster.length));
        }

        public void PlaceCluster(ClusterView clusterView, LetterPosition letterPosition)
        {
            var existingPlacedCluster = levelModel.placedClusters
                .FirstOrDefault(pc => pc.cluster == clusterView.Model);

            if (existingPlacedCluster != null)
            {
                levelModel.placedClusters.Remove(existingPlacedCluster);
            }
            
            levelModel.paletteClusters.Remove(clusterView.Model);
            
            var placedCluster = new PlacedCluster
            {
                cluster = clusterView.Model,
                position = letterPosition,
            };
            levelModel.placedClusters.Add(placedCluster);
            
            levelView.PlaceCluster(clusterView, placedCluster);
            paletteView.RemoveCluster(clusterView.Model);
        }

        public void ReturnClusterToPalette(Cluster cluster)
        {
            if (levelModel.paletteClusters.Contains(cluster))
            {
                paletteView.MoveClusterToPanel(cluster);
            }
            else
            {
                var placedCluster = levelModel.placedClusters.First(fc => fc.cluster == cluster);
                var clusterView = levelView.PlacedClusters[placedCluster];

                levelModel.placedClusters.Remove(placedCluster);
                levelModel.paletteClusters.Add(cluster);
                
                paletteView.AddCluster(cluster, clusterView);
            }
        }
        
        public void ReturnClusterToPlace(ClusterView cluster)
        {
            var placedCluster = levelModel.placedClusters.FirstOrDefault(fc => fc.cluster == cluster.Model);
            if (placedCluster != null)
            {
                Vector3 targetPos = levelView.GetLetterPosition(placedCluster.position);
                levelView.MoveClusterToWord(cluster, targetPos);
            }
        }

        public bool IsPlacedCLuster(Cluster cluster)
        {
            return levelModel.placedClusters.Any(pc => pc.cluster == cluster);
        }


    }
}