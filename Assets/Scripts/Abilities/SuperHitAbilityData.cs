using UnityEngine;

namespace SmashBall.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Configs/New Ability")]
    public class SuperHitAbilityData : BaseAbilityData, IChargeableAbility
    {
        [SerializeField] private int chargePerHit;
        [SerializeField] private int damageMultiplier;
        [SerializeField] private float ballSpeed;

        public int ChargePerHit => chargePerHit;
        public int DamageMultiplier => damageMultiplier;

        public float BallSpeed => ballSpeed;
    }
}
