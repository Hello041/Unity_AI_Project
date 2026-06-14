using TacticalRoguelike.Core;
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

        [SerializeField]
        private GameManager gameManager;

        
        private PieceController selectedPlacedPiece;
private PieceDefinition selectedPieceDefinition;
        private string lastMessage = "배치할 말을 선택하세요.";

        

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

private void OnEnable()
        {
            EnsureReferences();
            if (gameManager != null)
            {
                gameManager.OnStateChanged += HandleGameStateChanged;
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.OnStateChanged -= HandleGameStateChanged;
            }
        }


public void SelectPieceForPlacement(PieceDefinition pieceDefinition)
        {
            EnsureReferences();
            selectedPlacedPiece = null;
            selectedPieceDefinition = pieceDefinition;

            if (selectedPieceDefinition == null)
            {
                lastMessage = "선택된 말이 없습니다.";
                return;
            }

            lastMessage = "선택: " + GetLocalizedPieceName(selectedPieceDefinition) + ". 플레이어 배치 영역을 클릭하세요.";
            HighlightPlayerRows();
        }

public bool HandleTileClicked(GridPosition position)
        {
            EnsureReferences();

            if (preparationManager == null || boardManager == null)
            {
                lastMessage = "배치 시스템 참조가 없습니다.";
                return false;
            }

            PieceController clickedPiece = boardManager.GetOccupant(position) as PieceController;
            if (clickedPiece != null && clickedPiece.Owner == PieceOwner.Player)
            {
                selectedPieceDefinition = null;
                selectedPlacedPiece = clickedPiece;
                lastMessage = "배치된 " + GetLocalizedPieceName(clickedPiece.Definition) + " 선택. 빈 플레이어 배치 칸을 클릭하세요.";
                HighlightPlayerRows();
                return true;
            }

            if (selectedPlacedPiece != null)
            {
                bool repositioned = preparationManager.TryRepositionPlacedPiece(selectedPlacedPiece, position);
                lastMessage = repositioned
                    ? GetLocalizedPieceName(selectedPlacedPiece.Definition) + " 위치를 " + position + "(으)로 변경했습니다."
                    : position + " 위치로 이동할 수 없습니다.";

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
                lastMessage = "말을 먼저 선택하거나, 배치된 말을 클릭해 위치를 변경하세요.";
                return false;
            }

            int loadoutIndex = preparationManager.FindFirstUnplacedLoadoutIndex(selectedPieceDefinition);
            bool addedNewPiece = false;
            if (loadoutIndex < 0)
            {
                if (!preparationManager.TryAddPieceToLoadout(selectedPieceDefinition))
                {
                    lastMessage = selectedPieceDefinition.PieceType == PieceType.King
                        ? "플레이어 킹은 하나만 배치할 수 있습니다."
                        : "편성에 말을 추가할 수 없습니다.";
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

                lastMessage = GetLocalizedPieceName(selectedPieceDefinition) + "을(를) " + position + " 위치에 배치할 수 없습니다.";
                HighlightPlayerRows();
                return false;
            }

            lastMessage = GetLocalizedPieceName(selectedPieceDefinition) + "을(를) " + position + " 위치에 배치했습니다.";
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
            lastMessage = "선택을 해제했습니다.";

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

            if (selectedPlacedPiece != null && !selectedPlacedPiece.IsCaptured)
            {
                boardView.SetHighlight(selectedPlacedPiece.GridPosition, BoardHighlightType.Selected);
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



            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }
if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }
        }


private static string GetLocalizedPieceName(PieceDefinition definition)
        {
            if (definition == null) return "말";
            switch (definition.PieceType)
            {
                case PieceType.King: return "킹";
                case PieceType.Rook: return "룩";
                case PieceType.Knight: return "나이트";
                case PieceType.Pawn: return "폰";
                default: return definition.DisplayName;
            }
        }


private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Preparation)
            {
                return;
            }

            selectedPieceDefinition = null;
            selectedPlacedPiece = null;
            if (boardView != null)
            {
                boardView.ClearHighlights();
            }
        }
}
}
