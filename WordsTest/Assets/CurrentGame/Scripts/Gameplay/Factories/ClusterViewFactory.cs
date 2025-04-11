using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Views;
using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Factories
{
    public class ClusterViewFactory : PlaceholderFactory<Cluster, Transform, ClusterView>
    {
        [Inject(Id = "ClusterViewPrefab")] private GameObject clusterViewPrefab;
        [Inject] private DiContainer diContainer;
        
        public override ClusterView Create(Cluster model, Transform parent)
        {
            var clusterView = diContainer.InstantiatePrefabForComponent<ClusterView>(clusterViewPrefab, parent);
            clusterView.Initialize(model);
            return clusterView;
        }
    }
}