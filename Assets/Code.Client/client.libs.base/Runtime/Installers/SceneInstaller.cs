namespace Client.Libs.Installers {

	public abstract class SceneInstaller<TDerived> : BaseInstaller<TDerived> where TDerived : SceneInstaller<TDerived> { }

}