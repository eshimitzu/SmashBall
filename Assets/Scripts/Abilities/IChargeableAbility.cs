namespace SmashBall.Abilities
{
    public interface IChargeableAbility : IAbilityData
    {
        public int ChargePerHit { get; }
    }
}
