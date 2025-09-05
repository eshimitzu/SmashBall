using UniRx;

public class PlayerState
{
    public ReactiveProperty<int> Health = new();
    public ReactiveProperty<int> MaxHealth = new();
    public ReactiveProperty<int> AbilityCharge = new();
    public ReactiveProperty<int> AbilityMaxCharge = new();

    public PlayerState(int maxHealth, int abilityMaxCharge)
    {
        Health.Value = maxHealth;
        MaxHealth.Value = maxHealth;
        AbilityCharge.Value = 0;
        AbilityMaxCharge.Value = abilityMaxCharge;
    }
}
