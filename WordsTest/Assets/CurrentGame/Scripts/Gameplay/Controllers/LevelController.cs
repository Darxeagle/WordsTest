using System.Collections.Generic;
using System.Linq;
using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Views;
using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Controllers
{
    public class LevelController : IInitializable
    {
        [Inject] private LevelView levelView;
        [Inject] private PaletteView paletteView;
        [Inject] private LevelLoader levelLoader;
        [Inject] private RemoteConfigManager remoteConfigManager;

        private LevelModel levelModel;
        private int currentLevelIndex = 0;

        public async void Initialize()
        {
            // Start with level 1
            LoadLevelByIndex(0);
            
            // Initialize remote config for future levels
            await remoteConfigManager.Initialize();
        }

        public void LoadLevelByIndex(int index)
        {
            var levelData = levelLoader.LoadLevelByIndex(index);
            if (levelData != null)
            {
                currentLevelIndex = index;
                levelModel = levelLoader.CreateModel(levelData);
                InitializeGame();
            }
        }

        public void LoadNextLevel()
        {
            LoadLevelByIndex(currentLevelIndex + 1);
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

        public void CheckCompleted()
        {
            var placedClustersGroups = levelModel.placedClusters.GroupBy(pc => pc.position.wordIndex).ToList();
            var requiredWords = levelModel.words.ToList();
            
            foreach (var placedClusterGroup in placedClustersGroups)
            {
                foreach (var word in requiredWords)
                {
                    var remainingChars = word.characters.ToList();
                    
                    foreach (var placedCluster in placedClusterGroup)
                    {
                        for (int i = 0; i < placedCluster.cluster.length; i++)
                        {
                            var charIndex = placedCluster.position.charIndex + i;
                            if (word.characters[charIndex] == placedCluster.cluster.characters[i])
                            {
                                remainingChars.Remove(word.characters[charIndex]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    
                    if (remainingChars.Count == 0)
                    {
                        requiredWords.Remove(word);
                        break;
                    }
                }
            }
            
            if (requiredWords.Count == 0)
            {
                // All words are formed
                Debug.Log("Level Completed!");
            }
            else
            {
                // Not all words are formed
                Debug.Log("Level Not Completed Yet!");
            }
        }
    }
}