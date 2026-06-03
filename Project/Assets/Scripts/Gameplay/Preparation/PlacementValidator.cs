using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Preparation
{
    public sealed class PlacementValidator : MonoBehaviour
    {
        [SerializeField]
        private BoardManager boardManager;

        private void Awake()
        {
            EnsureBoardManager();
        }

        public bool CanPlace(PieceOwner owner, GridPosition position, out string reason)
        {
            EnsureBoardManager();

            if (boardManager == null)
            {
                reason = "BoardManager is missing.";
                return false;
            }

            if (!boardManager.IsValidPosition(position))
            {
                reason = "Position is outside the board.";
                return false;
            }

            if (boardManager.IsOccupied(position))
            {
                reason = "Position is already occupied.";
                return false;
            }

            if (owner == PieceOwner.Player && !boardManager.IsPlayerPlacementRow(position))
            {
                reason = "Player pieces must be placed on rows 0 or 1.";
                return false;
            }

            if (owner == PieceOwner.Enemy && !boardManager.IsEnemyPlacementRow(position))
            {
                reason = "Enemy pieces must be placed on the top two rows.";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        private void EnsureBoardManager()
        {
            if (boardManager != null)
            {
                return;
            }

            boardManager = FindFirstObjectByType<BoardManager>();
        }
    }
}
