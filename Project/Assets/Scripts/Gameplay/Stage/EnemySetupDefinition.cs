using UnityEngine;

namespace TacticalRoguelike.Gameplay.Stage
{
    [CreateAssetMenu(menuName = "Tactical Roguelike/Enemy Setup Definition")]
    public sealed class EnemySetupDefinition : ScriptableObject
    {
        [SerializeField]
        private string patternName;

        [SerializeField]
        private int randomWeight = 1;

        [SerializeField]
        private EnemySpawnEntry[] spawnEntries;

        public string PatternName
        {
            get { return patternName; }
        }

        public int RandomWeight
        {
            get { return Mathf.Max(1, randomWeight); }
        }

        public EnemySpawnEntry[] SpawnEntries
        {
            get { return spawnEntries; }
        }
    }
}
