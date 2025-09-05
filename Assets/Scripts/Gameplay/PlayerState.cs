using UniRx;

namespace SmashBall.Gameplay
{
    public class PlayerState
    {
        public ReactiveProperty<int> Health = new();
        public ReactiveProperty<int> MaxHealth = new();
        public ReactiveProperty<int> AbilityCharge = new();
        public int AbilityMaxCharge;

        public PlayerState(int maxHealth)
        {
            Health.Value = maxHealth;
            MaxHealth.Value = maxHealth;
            AbilityCharge.Value = 0;
            AbilityMaxCharge = 100;
        }
    }
}
