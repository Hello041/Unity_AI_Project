using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Preparation
{
    public sealed class LoadoutEventData
    {
        public LoadoutEventData(PieceDefinition changedPiece, int currentCost, int maxCost, int selectedCount, bool hasRequiredKing)
        {
            ChangedPiece = changedPiece;
            CurrentCost = currentCost;
            MaxCost = maxCost;
            SelectedCount = selectedCount;
            HasRequiredKing = hasRequiredKing;
        }

        public PieceDefinition ChangedPiece { get; private set; }
        public int CurrentCost { get; private set; }
        public int MaxCost { get; private set; }
        public int SelectedCount { get; private set; }
        public bool HasRequiredKing { get; private set; }
    }
}
