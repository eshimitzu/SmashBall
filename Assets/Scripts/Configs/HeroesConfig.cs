using System;
using SmashBall.Abilities;
using SmashBall.Gameplay;
using UnityEngine;

namespace SmashBall.Configs
{
    [CreateAssetMenu(fileName = "HeroesConfig", menuName = "Configs/HeroesConfig")]
    public class HeroesConfig : ScriptableObject
    {
        [SerializeField] public HeroConfig spikeHero;
    }


    [Serializable]
    public class HeroConfig
    {
        public Player heroPrefab;
        public Player heroPrefabEnemy;
        public int health;
        public int attack;
        public IAbility ability;
    }
}
