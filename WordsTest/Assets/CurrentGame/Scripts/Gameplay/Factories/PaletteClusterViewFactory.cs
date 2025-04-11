using CurrentGame.Gameplay.Views;
using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Factories
{
    public class PaletteClusterViewFactory : PlaceholderFactory<Transform, PaletteClusterView>
    {
        [Inject(Id = "PaletteClusterViewPrefab")] private GameObject paletteClusterViewPrefab;
        [Inject] private DiContainer diContainer;
        
        public override PaletteClusterView Create(Transform parent)
        {
            var paletteClusterView = diContainer.InstantiatePrefabForComponent<PaletteClusterView>(paletteClusterViewPrefab, parent);
            return paletteClusterView;
        }
    }
}