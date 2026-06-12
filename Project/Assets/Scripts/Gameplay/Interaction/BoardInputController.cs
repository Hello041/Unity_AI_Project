using System.Collections.Generic;
using TacticalRoguelike.Core;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Movement;
using TacticalRoguelike.Gameplay.Preparation;
using TacticalRoguelike.Gameplay.Pieces;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Interaction
{
    public sealed class BoardInputController : MonoBehaviour
    {
        [SerializeField]
        private Camera targetCamera;

        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private BoardView boardView;

        [SerializeField]
        private PieceMovementService movementService;

        [SerializeField]
        private PieceActionController actionController;

        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private ManualPlacementController manualPlacementController;
        private PieceController selectedPiece;

        public PieceController SelectedPiece
        {
            get { return selectedPiece; }
        }
        private readonly List<GridPosition> currentValidPositions = new List<GridPosition>();

        private void Awake()
        {
            EnsureReferences();
        }

private void Update()
        {
            if (!WasPrimaryPointerPressed())
            {
                return;
            }

            if (gameManager != null && gameManager.CurrentState != GameState.Preparation && gameManager.CurrentState != GameState.Playing)
            {
                return;
            }

            GridPosition clickedPosition;
            if (TryGetClickedBoardPosition(out clickedPosition))
            {
                HandleTileClicked(clickedPosition);
            }
        }

        public void ClearSelection()
        {
            selectedPiece = null;
            currentValidPositions.Clear();

            if (boardView != null)
            {
                boardView.ClearHighlights();
            }
        }

public void HandleTileClicked(GridPosition clickedPosition)
        {
            EnsureReferences();

            if (boardManager == null)
            {
                return;
            }

            if (gameManager != null && gameManager.CurrentState == GameState.Preparation)
            {
                if (manualPlacementController != null)
                {
                    manualPlacementController.HandleTileClicked(clickedPosition);
                }

                return;
            }

            if (gameManager != null && gameManager.CurrentState != GameState.Playing)
            {
                return;
            }

            PieceController clickedPiece = boardManager.GetOccupant(clickedPosition) as PieceController;
            if (clickedPiece != null && clickedPiece.Owner == PieceOwner.Player)
            {
                SelectPiece(clickedPiece);
                return;
            }

            if (selectedPiece != null && IsCurrentValidPosition(clickedPosition))
            {
                bool moved = actionController != null && actionController.TryMoveSelectedPiece(selectedPiece, clickedPosition);
                if (moved)
                {
                    ClearSelection();
                }
            }
        }

        private void SelectPiece(PieceController piece)
        {
            selectedPiece = piece;
            currentValidPositions.Clear();

            if (boardView != null)
            {
                boardView.ClearHighlights();
                boardView.SetHighlight(piece.GridPosition, BoardHighlightType.Selected);
            }

            if (movementService == null)
            {
                return;
            }

            List<GridPosition> validPositions = movementService.GetValidPositions(piece);
            for (int i = 0; i < validPositions.Count; i++)
            {
                GridPosition position = validPositions[i];
                currentValidPositions.Add(position);

                if (boardView != null)
                {
                    PieceController occupant = boardManager.GetOccupant(position) as PieceController;
                    BoardHighlightType highlight = occupant != null && occupant.Owner != piece.Owner
                        ? BoardHighlightType.Capture
                        : BoardHighlightType.ValidMove;
                    boardView.SetHighlight(position, highlight);
                }
            }
        }

        private bool IsCurrentValidPosition(GridPosition position)
        {
            for (int i = 0; i < currentValidPositions.Count; i++)
            {
                if (currentValidPositions[i] == position)
                {
                    return true;
                }
            }

            return false;
        }

private bool TryGetClickedBoardPosition(out GridPosition position)
        {
            EnsureReferences();
            position = new GridPosition(-1, -1);

            if (targetCamera == null)
            {
                return false;
            }

            Vector2 pointerPosition;
            if (!TryGetPointerPosition(out pointerPosition))
            {
                return false;
            }

            Ray ray = targetCamera.ScreenPointToRay(pointerPosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 100f))
            {
                return false;
            }

            BoardTileView tileView = hit.collider.GetComponent<BoardTileView>();
            if (tileView == null)
            {
                return false;
            }

            position = tileView.GridPosition;
            return true;
        }

private void EnsureReferences()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }

            if (boardView == null)
            {
                boardView = FindFirstObjectByType<BoardView>();
            }

            if (movementService == null)
            {
                movementService = FindFirstObjectByType<PieceMovementService>();
            }

            if (actionController == null)
            {
                actionController = FindFirstObjectByType<PieceActionController>();
            }

            if (manualPlacementController == null)
            {
                manualPlacementController = FindFirstObjectByType<ManualPlacementController>();
            }

            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }
        }
    

private static bool WasPrimaryPointerPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetMouseButtonDown(0);
#else
            return false;
#endif
        }

        private static bool TryGetPointerPosition(out Vector2 pointerPosition)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                pointerPosition = Mouse.current.position.ReadValue();
                return true;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            pointerPosition = Input.mousePosition;
            return true;
#endif
            pointerPosition = default;
            return false;
        }
}
}
