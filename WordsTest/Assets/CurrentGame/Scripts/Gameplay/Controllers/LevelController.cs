using System.Collections.Generic;
using System.Linq;
using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Controllers
{
    public class LevelController
    {
        [Inject] private LevelView levelView;
        [Inject] private PaletteView paletteView;
        [Inject] private RemoteConfigManager remoteConfigManager;
        [Inject] private GameController gameController;
        [Inject] private LevelModel levelModel;

        public void InitializeLevel()
        {
            levelView.Initialize(levelModel);
            paletteView.Initialize(levelModel);
        }

        public bool CheckPlacement(Cluster cluster, LetterPosition letterPosition)
        {
            var word = levelModel.Words[letterPosition.wordIndex];
            
            if (cluster == null || letterPosition.wordIndex < 0 || letterPosition.charIndex + cluster.length > word.length)
                return false;

            // Check if position is already occupied
            return !levelModel.PlacedClusters.Any(pc => pc.cluster != cluster &&
                pc.position.wordIndex == letterPosition.wordIndex && 
                (pc.position.charIndex + pc.cluster.length > letterPosition.charIndex && 
                 pc.position.charIndex < letterPosition.charIndex + cluster.length));
        }

        public void PlaceCluster(ClusterView clusterView, LetterPosition letterPosition)
        {
            var existingPlacedCluster = levelModel.PlacedClusters
                .FirstOrDefault(pc => pc.cluster == clusterView.Model);

            if (existingPlacedCluster != null)
            {
                levelModel.RemovePlacedCluster(existingPlacedCluster);
                levelView.RemoveCluster(existingPlacedCluster);
            }
            
            var removeIndex = levelModel.PaletteClusters.ToList().IndexOf(clusterView.Model);
            levelModel.RemovePaletteCluster(clusterView.Model);
            
            var placedCluster = new PlacedCluster
            {
                cluster = clusterView.Model,
                position = letterPosition,
            };
            levelModel.AddPlacedCluster(placedCluster);
            
            levelView.AnimateClusterPlace(clusterView, placedCluster).Forget();
            paletteView.RemoveCluster(clusterView.Model, removeIndex);
        }

        public void ReturnClusterToPalette(Cluster cluster, Vector3 position)
        {
            if (levelModel.PaletteClusters.Contains(cluster))
            {
                paletteView.AnimateClusterReturnToPanel(cluster).Forget();
            }
            else
            {
                var placedCluster = levelModel.PlacedClusters.First(fc => fc.cluster == cluster);
                var clusterView = levelView.PlacedClusters[placedCluster];
                
                var insertIndex = paletteView.GetInsertIndex(position);

                levelModel.RemovePlacedCluster(placedCluster);
                levelModel.AddPaletteCluster(cluster, insertIndex);
                
                levelView.RemoveCluster(placedCluster);
                paletteView.AddCluster(cluster, clusterView, insertIndex);
            }
        }
        
        public void ReturnClusterToPlace(ClusterView cluster)
        {
            var placedCluster = levelModel.PlacedClusters.FirstOrDefault(fc => fc.cluster == cluster.Model);
            if (placedCluster != null)
            {
                Vector3 targetPos = levelView.GetLetterPosition(placedCluster.position);
                levelView.AnimateClusterMove(cluster, targetPos).Forget();
            }
        }

        public bool IsPlacedCLuster(Cluster cluster)
        {
            return levelModel.PlacedClusters.Any(pc => pc.cluster == cluster);
        }

        public List<Word> CheckCompleted()
        {
            var placedClustersGroups = levelModel.PlacedClusters.GroupBy(pc => pc.position.wordIndex).ToList();
            var wordsLeft = levelModel.Words.ToList();
            var completedWords = new List<Word>();
            
            foreach (var placedClusterGroup in placedClustersGroups)
            {
                var orderedClusters = placedClusterGroup.OrderBy(pc => pc.position.charIndex).ToList();
                var completeChars = new char[orderedClusters.Last().position.charIndex + orderedClusters.Last().cluster.length];
                
                foreach (var placedCluster in orderedClusters)
                {
                    for (int i = 0; i < placedCluster.cluster.length; i++)
                    {
                        completeChars[placedCluster.position.charIndex + i] = placedCluster.cluster.characters[i];
                    }
                }
                
                var word = wordsLeft.FirstOrDefault(w => w.characters.SequenceEqual(completeChars));
                if (word != null)
                {
                    wordsLeft.Remove(word);
                    completedWords.Add(word);
                }
            }

            if (wordsLeft.Count == 0) return completedWords;
            else return null;
        }
    }
}