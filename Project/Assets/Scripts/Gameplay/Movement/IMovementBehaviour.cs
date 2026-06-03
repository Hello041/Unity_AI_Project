using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Movement
{
    public interface IMovementBehaviour
    {
        List<GridPosition> GetValidPositions(PieceController piece, BoardManager boardManager);
    }
}
