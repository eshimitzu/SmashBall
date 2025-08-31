using Cysharp.Threading.Tasks;

public interface IGameLoader
{
    public UniTask Load();
    float CurrentProgress { get; }
}
