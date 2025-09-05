using UniRx;

namespace SmashBall.Gameplay
{
    public interface IGameplay
    {
        public Ball Ball { get; }
        
        public IReadOnlyReactiveProperty<GameplayState> CurrentGameplayState { get; }
    }
}
