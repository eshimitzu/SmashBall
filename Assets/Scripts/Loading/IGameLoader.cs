using Cysharp.Threading.Tasks;

namespace SmashBall.Loading
{
    public interface IGameLoader
    {
        public UniTask Load();
        float CurrentProgress { get; }
    }
}
