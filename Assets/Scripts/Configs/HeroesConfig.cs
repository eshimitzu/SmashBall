using UnityEngine;

[CreateAssetMenu(fileName = "HeroesConfig", menuName = "Configs/HeroesConfig")]
public class HeroesConfig : ScriptableObject
{
    [SerializeField] public GameObject heroPrefab;
}
