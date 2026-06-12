using Client.Libs.Installers;

namespace Client.Libs.GameState
{
    public class GameStatesInstaller : SceneInstaller<GameStatesInstaller>
    {
        protected override void OnInstallBindings()
        {
            Container.BindInterfacesTo<EmptyGameState>().AsSingle();
            Container.BindInterfacesTo<GameStateMachine>().AsSingle();
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }
    }
}