using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Views;
using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Controllers 
{
    public class LevelInputController : IInitializable
    {
        [Inject] private LevelView levelView;
        [Inject] private LevelController levelController;

        private Cluster draggedCluster;

        public void Initialize()
        {
            levelView.OnClusterDragBegin += HandleClusterDragBegin;
            levelView.OnClusterDragProgress += HandleClusterDragProgress;
            levelView.OnClusterDragEnd += HandleClusterDragEnd;
        }

        private void HandleClusterDragBegin(ClusterView view, Cluster cluster, Vector3 position)
        {
            if (draggedCluster != null) return;
            draggedCluster = cluster;
        }

        private void HandleClusterDragProgress(ClusterView view, Cluster cluster, Vector3 position)
        {
        }

        public void HandleClusterDragEnd(ClusterView view, Cluster cluster, Vector3 position)
        {
            if (cluster != draggedCluster) return;
            draggedCluster = null;
            
            if (levelView.IsPositionOnPalette(position))
            {
                levelController.ReturnClusterToPalette(cluster, position);
                return;
            }
            else
            {
                var letterPosition = levelView.GetNearestLetter(position);

                bool isValidPlacement = letterPosition != null && levelController.CheckPlacement(cluster, letterPosition);

                if (isValidPlacement)
                {
                    levelController.PlaceCluster(view, letterPosition);
                }
                else
                {
                    if (levelController.IsPlacedCLuster(cluster))
                    {
                        levelController.ReturnClusterToPlace(view);
                    }
                    else
                    {
                        levelController.ReturnClusterToPalette(cluster, position);
                    }
                }
            }
        }
    }
}