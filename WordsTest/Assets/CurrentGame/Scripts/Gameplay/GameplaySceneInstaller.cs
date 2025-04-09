using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Controllers;
using CurrentGame.Gameplay.Views;
using Zenject;

namespace CurrentGame.Gameplay
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<RemoteConfigManager>().AsSingle();
            Container.Bind<LevelsDataManager>().AsSingle();
            Container.Bind<LevelLoader>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelInputController>().AsSingle();
            Container.Bind<LevelView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PaletteView>().FromComponentInHierarchy().AsSingle();
        }
    }
}