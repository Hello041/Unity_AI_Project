using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Movement
{
    public sealed class KingMovement : IMovementBehaviour
    {
        public List<GridPosition> GetValidPositions(PieceController piece, BoardManager boardManager)
        {
            List<GridPosition> results = new List<GridPosition>();
            if (piece == null || boardManager == null)
            {
                return results;
            }

            GridPosition origin = piece.GridPosition;
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    MovementRuleUtility.TryAddIfAvailable(results, piece, boardManager, new GridPosition(origin.X + x, origin.Y + y));
                }
            }

            return results;
        }
    }
}
