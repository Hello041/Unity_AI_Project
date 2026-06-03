using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Preparation
{
    public sealed class ManualPlacementController : MonoBehaviour
    {
        [SerializeField]
        private PreparationManager preparationManager;

        [SerializeField]
        private BoardView boardView;

        [SerializeField]
        private BoardManager boardManager;

        private PieceDefinition selectedPieceDefinition;
        private string lastMessage = "Select a piece.";

        public PieceDefinition SelectedPieceDefinition
        {
            get { return selectedPieceDefinition; }
        }

        public string LastMessage
        {
            get { return lastMessage; }
        }

        private void Awake()
        {
            EnsureReferences();
        }

        public void SelectPieceForPlacement(PieceDefinition pieceDefinition)
        {
            EnsureReferences();
            selectedPieceDefinition = pieceDefinition;

            if (selectedPieceDefinition == null)
            {
                lastMessage = "No piece selected.";
                return;
            }

            lastMessage = "Selected: " + selectedPieceDefinition.DisplayName + ". Click a player row tile.";
            HighlightPlayerRows();
        }

        public bool HandleTileClicked(GridPosition position)
        {
            EnsureReferences();

            if (selectedPieceDefinition == null)
            {
                lastMessage = "Select a piece first.";
                return false;
            }

            if (preparationManager == null)
            {
                lastMessage = "PreparationManager missing.";
                return false;
            }

            if (!preparationManager.TryAddPieceToLoadout(selectedPieceDefinition))
            {
                lastMessage = "Cannot add piece to loadout.";
                return false;
            }

            int loadoutIndex = preparationManager.SelectedCount - 1;
            if (!preparationManager.TryPlaceSelectedPiece(loadoutIndex, position))
            {
                preparationManager.TryRemoveLastPieceFromLoadout();
                lastMessage = "Cannot place " + selectedPieceDefinition.DisplayName + " at " + position + ".";
                HighlightPlayerRows();
                return false;
            }

            lastMessage = "Placed " + selectedPieceDefinition.DisplayName + " at " + position + ".";
            selectedPieceDefinition = null;

            if (boardView != null)
            {
                boardView.ClearHighlights();
            }

            return true;
        }

        public void ClearSelection()
        {
            selectedPieceDefinition = null;
            lastMessage = "Selection cleared.";

            if (boardView != null)
            {
                boardView.ClearHighlights();
            }
        }

private void HighlightPlayerRows()
        {
            if (boardView == null || boardManager == null)
            {
                return;
            }

            boardView.ClearHighlights();
            for (int y = 0; y <= 1; y++)
            {
                for (int x = 0; x < boardManager.BoardWidth; x++)
                {
                    boardView.SetHighlight(new GridPosition(x, y), BoardHighlightType.Placement);
                }
            }
        }

private void EnsureReferences()
        {
            if (preparationManager == null)
            {
                preparationManager = FindFirstObjectByType<PreparationManager>();
            }

            if (boardView == null)
            {
                boardView = FindFirstObjectByType<BoardView>();
            }

            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }
        }
    }
}
