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

        
        private PieceController selectedPlacedPiece;
private PieceDefinition selectedPieceDefinition;
        private string lastMessage = "Select a piece.";

        

        public PieceController SelectedPlacedPiece
        {
            get { return selectedPlacedPiece; }
        }
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
            selectedPlacedPiece = null;
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

            if (preparationManager == null || boardManager == null)
            {
                lastMessage = "Preparation references missing.";
                return false;
            }

            PieceController clickedPiece = boardManager.GetOccupant(position) as PieceController;
            if (clickedPiece != null && clickedPiece.Owner == PieceOwner.Player)
            {
                selectedPieceDefinition = null;
                selectedPlacedPiece = clickedPiece;
                lastMessage = "Selected placed " + clickedPiece.Definition.DisplayName + ". Click an empty player row tile.";
                HighlightPlayerRows();
                return true;
            }

            if (selectedPlacedPiece != null)
            {
                bool repositioned = preparationManager.TryRepositionPlacedPiece(selectedPlacedPiece, position);
                lastMessage = repositioned
                    ? "Repositioned " + selectedPlacedPiece.Definition.DisplayName + " to " + position + "."
                    : "Cannot reposition piece to " + position + ".";

                if (repositioned)
                {
                    selectedPlacedPiece = null;
                    if (boardView != null)
                    {
                        boardView.ClearHighlights();
                    }
                }
                else
                {
                    HighlightPlayerRows();
                }

                return repositioned;
            }

            if (selectedPieceDefinition == null)
            {
                lastMessage = "Select a piece first, or click a placed piece to reposition it.";
                return false;
            }

            int loadoutIndex = preparationManager.FindFirstUnplacedLoadoutIndex(selectedPieceDefinition);
            bool addedNewPiece = false;
            if (loadoutIndex < 0)
            {
                if (!preparationManager.TryAddPieceToLoadout(selectedPieceDefinition))
                {
                    lastMessage = selectedPieceDefinition.PieceType == PieceType.King
                        ? "Only one Player King is allowed."
                        : "Cannot add piece to loadout.";
                    return false;
                }

                loadoutIndex = preparationManager.SelectedCount - 1;
                addedNewPiece = true;
            }

            if (!preparationManager.TryPlaceSelectedPiece(loadoutIndex, position))
            {
                if (addedNewPiece)
                {
                    preparationManager.TryRemoveLastPieceFromLoadout();
                }

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
            selectedPlacedPiece = null;
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
