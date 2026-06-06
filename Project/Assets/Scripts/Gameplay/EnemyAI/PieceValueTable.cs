using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.EnemyAI
{
    public static class PieceValueTable
    {
        public const int KingValue = 100;
        public const int RookValue = 5;
        public const int KnightValue = 3;
        public const int PawnValue = 1;

        public static int GetValue(PieceDefinition definition)
        {
            if (definition == null)
            {
                return 0;
            }

            return GetValue(definition.PieceType);
        }

        public static int GetValue(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.King:
                    return KingValue;
                case PieceType.Rook:
                    return RookValue;
                case PieceType.Knight:
                    return KnightValue;
                case PieceType.Pawn:
                    return PawnValue;
                default:
                    return 0;
            }
        }
    }
}
