using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Movement
{
    public sealed class RookMovement : IMovementBehaviour
    {
        private static readonly GridPosition[] Directions =
        {
            new GridPosition(1, 0),
            new GridPosition(-1, 0),
            new GridPosition(0, 1),
            new GridPosition(0, -1)
        };

        public List<GridPosition> GetValidPositions(PieceController piece, BoardManager boardManager)
        {
            List<GridPosition> results = new List<GridPosition>();
            if (piece == null || boardManager == null)
            {
                return results;
            }

            GridPosition origin = piece.GridPosition;
            for (int i = 0; i < Directions.Length; i++)
            {
                AddLine(results, piece, boardManager, origin, Directions[i]);
            }

            return results;
        }

        private void AddLine(List<GridPosition> results, PieceController piece, BoardManager boardManager, GridPosition origin, GridPosition direction)
        {
            GridPosition current = new GridPosition(origin.X + direction.X, origin.Y + direction.Y);
            while (boardManager.IsValidPosition(current))
            {
                bool canContinue = MovementRuleUtility.TryAddIfAvailable(results, piece, boardManager, current);
                if (!canContinue)
                {
                    break;
                }

                current = new GridPosition(current.X + direction.X, current.Y + direction.Y);
            }
        }
    }
}
