using UnityEngine;

namespace TacticalRoguelike.Gameplay.Pieces
{
    [CreateAssetMenu(menuName = "Tactical Roguelike/Piece Definition")]
    public sealed class PieceDefinition : ScriptableObject
    {
        [SerializeField]
        private PieceType pieceType;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private int loadoutCost;

        [SerializeField]
        private float baseCooldown = 1f;

        [SerializeField]
        private Sprite playerSprite;

        [SerializeField]
        private Sprite enemySprite;

        public PieceType PieceType
        {
            get { return pieceType; }
        }

        public string DisplayName
        {
            get { return displayName; }
        }

        public int LoadoutCost
        {
            get { return loadoutCost; }
        }

        public float BaseCooldown
        {
            get { return baseCooldown; }
        }

        public Sprite PlayerSprite
        {
            get { return playerSprite; }
        }

        public Sprite EnemySprite
        {
            get { return enemySprite; }
        }

        public Sprite GetSprite(PieceOwner owner)
        {
            return owner == PieceOwner.Player ? playerSprite : enemySprite;
        }
    }
}
