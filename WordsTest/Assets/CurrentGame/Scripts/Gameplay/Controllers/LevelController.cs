using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Views;
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
            }
            
            levelModel.RemovePaletteCluster(clusterView.Model);
            
            var placedCluster = new PlacedCluster
            {
                cluster = clusterView.Model,
                position = letterPosition,
            };
            levelModel.AddPlacedCluster(placedCluster);
            
            levelView.PlaceCluster(clusterView, placedCluster);
            paletteView.RemoveCluster(clusterView.Model);
        }

        public void ReturnClusterToPalette(Cluster cluster, Vector3 position)
        {
            if (levelModel.PaletteClusters.Contains(cluster))
            {
                paletteView.ReturnClusterToPanel(cluster);
            }
            else
            {
                var placedCluster = levelModel.PlacedClusters.First(fc => fc.cluster == cluster);
                var clusterView = levelView.PlacedClusters[placedCluster];
                
                var insertIndex = paletteView.GetInsertIndex(position);

                levelModel.RemovePlacedCluster(placedCluster);
                levelModel.AddPaletteCluster(cluster, insertIndex);
                
                paletteView.AddCluster(cluster, clusterView);
            }
        }
        
        public void ReturnClusterToPlace(ClusterView cluster)
        {
            var placedCluster = levelModel.PlacedClusters.FirstOrDefault(fc => fc.cluster == cluster.Model);
            if (placedCluster != null)
            {
                Vector3 targetPos = levelView.GetLetterPosition(placedCluster.position);
                levelView.MoveClusterToWord(cluster, targetPos);
            }
        }

        public bool IsPlacedCLuster(Cluster cluster)
        {
            return levelModel.PlacedClusters.Any(pc => pc.cluster == cluster);
        }

        public void CheckCompleted()
        {
            gameController.CheckFinish();
        }
    }
}