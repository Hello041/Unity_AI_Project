using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Movement
{
    public sealed class KnightMovement : IMovementBehaviour
    {
        private static readonly GridPosition[] Offsets =
        {
            new GridPosition(1, 2),
            new GridPosition(2, 1),
            new GridPosition(2, -1),
            new GridPosition(1, -2),
            new GridPosition(-1, -2),
            new GridPosition(-2, -1),
            new GridPosition(-2, 1),
            new GridPosition(-1, 2)
        };

        public List<GridPosition> GetValidPositions(PieceController piece, BoardManager boardManager)
        {
            List<GridPosition> results = new List<GridPosition>();
            if (piece == null || boardManager == null)
            {
                return results;
            }

            GridPosition origin = piece.GridPosition;
            for (int i = 0; i < Offsets.Length; i++)
            {
                GridPosition offset = Offsets[i];
                MovementRuleUtility.TryAddIfAvailable(results, piece, boardManager, new GridPosition(origin.X + offset.X, origin.Y + offset.Y));
            }

            return results;
        }
    }
}
