using UnityEngine;

namespace SmashBall.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Configs/Abilities")]
    public class SuperHitAbility : ScriptableObject, IAbility
    {
        public int chargePerHit;
        public int maxCharges;
        public int damageAmplifier;
    }
}
