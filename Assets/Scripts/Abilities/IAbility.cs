using SmashBall.Gameplay;

namespace SmashBall.Abilities
{
    public interface IAbility
    {
        bool IsActive { get; }
        
        void Activate(Player player);
        void Deactivate(Player player);

        void BallHit(Player player, Ball ball);
    }
}
