using UniRx;

namespace SmashBall.Gameplay
{
    public interface IGameplay
    {
        public Player Player { get; }
        public Player Enemy { get; }
        public Ball Ball { get; }
        
        public IReadOnlyReactiveProperty<GameplayState> CurrentGameplayState { get; }
    }
}
