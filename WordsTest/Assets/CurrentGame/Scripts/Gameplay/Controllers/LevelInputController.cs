using UnityEngine;
using WordsTest.Gameplay.Models;
using System;

namespace WordsTest.Gameplay 
{
    public class LevelInputController : MonoBehaviour
    {
        [SerializeField] private LevelView levelView;
        [SerializeField] private LevelController levelController;

        private Cluster draggedCluster;

        private void Awake()
        {
            levelView.OnClusterDragBegin += HandleClusterDragBegin;
            levelView.OnClusterDragProgress += HandleClusterDragProgress;
            levelView.OnClusterDragEnd += HandleClusterDragEnd;
        }

        private void HandleClusterDragBegin(ClusterView view, Cluster cluster, Vector3 position)
        {
            draggedCluster = cluster;
        }

        private void HandleClusterDragProgress(ClusterView view, Cluster cluster, Vector3 position)
        {
            //var letterPosition = levelView.GetNearestLetter(position);

            //bool isValidPlacement = levelController.CheckPlacement(cluster, letterPosition);
            //view.SetHighlight(isValidPlacement);
        }

        public void HandleClusterDragEnd(ClusterView view, Cluster cluster, Vector3 position)
        {
            draggedCluster = null;
            
            if (levelView.IsPositionOnPalette(position))
            {
                levelController.ReturnClusterToPalette(cluster);
                return;
            }
            else
            {
                var letterPosition = levelView.GetNearestLetter(position);

                bool isValidPlacement = letterPosition != null && levelController.CheckPlacement(cluster, letterPosition);
                view.ResetHighlight();

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
                        levelController.ReturnClusterToPalette(cluster);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (levelView != null)
            {
                levelView.OnClusterDragBegin -= HandleClusterDragBegin;
                levelView.OnClusterDragProgress -= HandleClusterDragProgress;
                levelView.OnClusterDragEnd -= HandleClusterDragEnd;
            }
        }
    }
}