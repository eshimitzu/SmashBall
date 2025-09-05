namespace SmashBall.Abilities
{
    public static class AbilitiesFactory
    {
        public static IAbility Create(IAbilityData data)
        {
            IAbility ability = null;
            
            switch (data.Id)
            {
                case AbilityId.SuperHit:
                    ability = new SuperHitAbility(data);
                    break;
            }

            return ability;
        }
    }
}
