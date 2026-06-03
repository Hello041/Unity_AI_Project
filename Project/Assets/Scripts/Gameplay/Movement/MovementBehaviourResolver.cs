using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Movement
{
    public sealed class MovementBehaviourResolver
    {
        private readonly KingMovement kingMovement = new KingMovement();
        private readonly RookMovement rookMovement = new RookMovement();
        private readonly KnightMovement knightMovement = new KnightMovement();
        private readonly PawnMovement pawnMovement = new PawnMovement();

        public IMovementBehaviour Resolve(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.King:
                    return kingMovement;
                case PieceType.Rook:
                    return rookMovement;
                case PieceType.Knight:
                    return knightMovement;
                case PieceType.Pawn:
                    return pawnMovement;
                default:
                    return pawnMovement;
            }
        }
    }
}
