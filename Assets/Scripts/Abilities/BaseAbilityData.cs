using UnityEngine;

namespace SmashBall.Abilities
{
    public class BaseAbilityData : ScriptableObject, IAbilityData
    {
        [SerializeField] private AbilityId id;

        public AbilityId Id => id;
    }
}
