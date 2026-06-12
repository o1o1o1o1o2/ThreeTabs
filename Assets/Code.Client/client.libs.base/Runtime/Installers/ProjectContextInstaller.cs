using System;
using Shared.Libs.Utils;
using Zenject;

namespace Client.Libs.Installers
{
    public abstract class ProjectContextInstaller<TDerived> : BaseInstaller<TDerived>
        where TDerived : ProjectContextInstaller<TDerived>
    {
        public override void InstallBindings()
        {
            if (GetType() != TypeOf<TDerived>.Raw)
                throw new Exception($"{GetType().FullName}: invalid generic base param: must be <{GetType().Name}> but actual is <{TypeOf<TDerived>.Raw.Name}>!");

            if (!GetComponent<ProjectContext>())
                throw new Exception($"{GetType().Name} can be only used on {nameof(ProjectContext)} container!");
            base.InstallBindings();
        }
    }
}