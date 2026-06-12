using Cysharp.Threading.Tasks;

namespace Client.Libs.Contracts
{
    public interface IApplicationLoader
    {
        UniTask Play();
    }
}