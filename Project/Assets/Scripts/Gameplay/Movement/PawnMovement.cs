using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Movement
{
    public sealed class PawnMovement : IMovementBehaviour
    {
        public List<GridPosition> GetValidPositions(PieceController piece, BoardManager boardManager)
        {
            List<GridPosition> results = new List<GridPosition>();
            if (piece == null || boardManager == null)
            {
                return results;
            }

            int direction = piece.Owner == PieceOwner.Player ? 1 : -1;
            GridPosition origin = piece.GridPosition;
            GridPosition forward = new GridPosition(origin.X, origin.Y + direction);
            if (MovementRuleUtility.IsEmpty(boardManager, forward))
            {
                results.Add(forward);
            }

            AddCaptureIfEnemy(results, piece, boardManager, new GridPosition(origin.X - 1, origin.Y + direction));
            AddCaptureIfEnemy(results, piece, boardManager, new GridPosition(origin.X + 1, origin.Y + direction));
            return results;
        }

        private void AddCaptureIfEnemy(List<GridPosition> results, PieceController piece, BoardManager boardManager, GridPosition position)
        {
            if (MovementRuleUtility.HasEnemyPiece(piece, boardManager, position))
            {
                results.Add(position);
            }
        }
    }
}
