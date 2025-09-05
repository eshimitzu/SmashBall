using SmashBall.Gameplay;

namespace SmashBall.Abilities
{
    public class SuperHitAbility : IAbility, IChargeableAbility
    {
        public SuperHitAbilityData Data { get; }

        public AbilityId Id => Data.Id;
        public int ChargePerHit => Data.ChargePerHit;
        
        
        public SuperHitAbility(IAbilityData data)
        {
            Data = data as SuperHitAbilityData;
        }

        public bool IsActive { get; private set; }

        
        public void Activate(Player player)
        {
            IsActive = true;
            player.abilityChargedFx.SetActive(true);
        }
        
        public void Deactivate(Player player)
        {
            IsActive = false;
            player.abilityChargedFx.SetActive(false);
        }

        public void BallHit(Player player, Ball ball)
        {
            if (!IsActive)
            {
                player.State.AbilityCharge.Value += ChargePerHit;
                
                if (player.State.AbilityCharge.Value >= 100)
                {
                    Activate(player);
                }
            }
            else
            {
                ball.Damage.Value *= Data.DamageMultiplier;
                ball.AimShot(Data.BallSpeed);
                player.State.AbilityCharge.Value = 0;
                player.abilityTriggerFx.Play();
                
                Deactivate(player);
            }
        }
    }
}
