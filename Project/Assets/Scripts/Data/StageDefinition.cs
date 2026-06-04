using UnityEngine;

namespace TacticalRoguelike.Data
{
    [CreateAssetMenu(fileName = "StageDefinition", menuName = "Tactical Roguelike/Stage Definition")]
    public sealed class StageDefinition : ScriptableObject
    {
        [SerializeField]
        private string stageName = "MVP Stage";

        [SerializeField]
        private int boardWidth = 6;

        [SerializeField]
        private int boardHeight = 6;

        [SerializeField]
        private int playerMaxHealth = 3;

        [SerializeField]
        private int maxLoadoutCost = 7;

        [SerializeField]
        private bool showResultAfterClear;

        public string StageName => stageName;
        public int BoardWidth => Mathf.Max(1, boardWidth);
        public int BoardHeight => Mathf.Max(1, boardHeight);
        public int PlayerMaxHealth => Mathf.Max(1, playerMaxHealth);
        public int MaxLoadoutCost => Mathf.Max(0, maxLoadoutCost);
        public bool ShowResultAfterClear => showResultAfterClear;

        private void OnValidate()
        {
            boardWidth = Mathf.Max(1, boardWidth);
            boardHeight = Mathf.Max(1, boardHeight);
            playerMaxHealth = Mathf.Max(1, playerMaxHealth);
            maxLoadoutCost = Mathf.Max(0, maxLoadoutCost);
        }
    }
}
