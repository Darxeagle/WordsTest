using CurrentGame.GameFlow;
using CurrentGame.Gameplay.Controllers;
using CurrentGame.Gameplay.Models;
using CurrentGame.Gameplay.Saves;
using CurrentGame.Gameplay.Views;
using CurrentGame.MainMenu;
using CurrentGame.Options;
using CurrentGame.Sounds;
using CurrentGame.Victory;
using Zenject;

namespace CurrentGame.Gameplay
{
    public class MainSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RemoteConfigManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelsDataManager>().AsSingle().NonLazy();
            Container.Bind<TransitionManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<EventManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SoundsConfig>().FromScriptableObjectResource("SoundsConfig").AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SoundController>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<MusicController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameModel>().AsSingle();
            Container.Bind<MainMenuScreen>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<OptionsScreen>().FromComponentInHierarchy().AsSingle();
            
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelInputController>().AsSingle();
            Container.Bind<LevelModel>().AsSingle();
            Container.Bind<LevelView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PaletteView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameplayUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<VictoryScreen>().FromComponentInHierarchy().AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
            Container.Bind<ScreenAdjuster>().FromComponentInHierarchy().AsSingle();
        }
    }
}