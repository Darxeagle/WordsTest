using CurrentGame.GameFlow;
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
        [Inject] private EventManager eventManager;

        private Cluster draggedCluster;
        
        public bool IsDragging => draggedCluster != null;

        public void Initialize()
        {
            levelView.OnClusterDragBegin += HandleClusterDragBegin;
            levelView.OnClusterDragProgress += HandleClusterDragProgress;
            levelView.OnClusterDragEnd += HandleClusterDragEnd;
        }

        private void HandleClusterDragBegin(ClusterView view, Cluster cluster, Vector3 position)
        {
            if (IsDragging) return;
            draggedCluster = cluster;
            eventManager.TriggerEvent(EventId.ClusterDragBegin);
        }

        private void HandleClusterDragProgress(ClusterView view, Cluster cluster, Vector3 position)
        {
        }

        public void HandleClusterDragEnd(ClusterView view, Cluster cluster, Vector3 position)
        {
            if (cluster != draggedCluster) return;
            draggedCluster = null;
            eventManager.TriggerEvent(EventId.ClusterDragEnd);
            
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