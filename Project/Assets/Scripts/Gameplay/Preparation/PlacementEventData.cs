using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Preparation
{
    public sealed class PlacementEventData
    {
        public PlacementEventData(PieceDefinition pieceDefinition, PieceOwner owner, GridPosition position, bool success, string message)
        {
            PieceDefinition = pieceDefinition;
            Owner = owner;
            Position = position;
            Success = success;
            Message = message;
        }

        public PieceDefinition PieceDefinition { get; private set; }
        public PieceOwner Owner { get; private set; }
        public GridPosition Position { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }
    }
}
