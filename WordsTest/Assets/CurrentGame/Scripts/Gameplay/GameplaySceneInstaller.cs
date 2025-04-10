using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Controllers;
using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Views;
using CurrentGame.Victory;
using Zenject;

namespace CurrentGame.Gameplay
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<RemoteConfigManager>().AsSingle().NonLazy();
            Container.Bind<LevelsDataManager>().AsSingle().NonLazy();
            Container.Bind<GameModel>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelInputController>().AsSingle();
            Container.Bind<LevelModel>().AsSingle();
            Container.Bind<LevelView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PaletteView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<VictoryScreen>().FromComponentInHierarchy().AsSingle();
        }
    }
}