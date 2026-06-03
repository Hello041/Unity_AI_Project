using System;
using System.Collections.Generic;
using TacticalRoguelike.Core;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Preparation
{
    public sealed class PreparationManager : MonoBehaviour
    {
        [SerializeField]
        private int maxLoadoutCost = 7;

        [SerializeField]
        private PieceDefinition[] availablePieces;

        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private PlacementValidator placementValidator;

        [SerializeField]
        private Transform piecesRoot;

        [SerializeField]
        private GameManager gameManager;

        private readonly List<PieceDefinition> selectedLoadout = new List<PieceDefinition>();
        private readonly List<PieceController> placedPieces = new List<PieceController>();
        private readonly List<int> placedLoadoutIndexes = new List<int>();

        public event Action<LoadoutEventData> OnLoadoutChanged;
        public event Action<PlacementEventData> OnPiecePlaced;

        public int CurrentCost
        {
            get { return CalculateCurrentCost(); }
        }

        public int MaxLoadoutCost
        {
            get { return maxLoadoutCost; }
        }

        public int SelectedCount
        {
            get { return selectedLoadout.Count; }
        }

        public int PlacedCount
        {
            get { return placedPieces.Count; }
        }

        private void Awake()
        {
            EnsureReferences();
        }

        public bool TryAddPieceToLoadout(PieceDefinition pieceDefinition)
        {
            EnsureReferences();

            if (pieceDefinition == null)
            {
                Debug.LogWarning("Cannot add a null piece to loadout.");
                return false;
            }

            int nextCost = CurrentCost + Mathf.Max(0, pieceDefinition.LoadoutCost);
            if (nextCost > maxLoadoutCost)
            {
                Debug.LogWarning("Loadout cost would exceed budget: " + nextCost + "/" + maxLoadoutCost);
                return false;
            }

            selectedLoadout.Add(pieceDefinition);
            PublishLoadoutChanged(pieceDefinition);
            return true;
        }

        public bool TryRemoveLastPieceFromLoadout()
        {
            if (selectedLoadout.Count == 0)
            {
                return false;
            }

            PieceDefinition removedPiece = selectedLoadout[selectedLoadout.Count - 1];
            selectedLoadout.RemoveAt(selectedLoadout.Count - 1);
            PublishLoadoutChanged(removedPiece);
            return true;
        }

        public void ClearPreparation()
        {
            selectedLoadout.Clear();
            ClearPlacedPieces();
            PublishLoadoutChanged(null);
        }

        public bool TryPlaceSelectedPiece(int loadoutIndex, GridPosition position)
        {
            EnsureReferences();

            if (loadoutIndex < 0 || loadoutIndex >= selectedLoadout.Count)
            {
                PublishPlacement(null, PieceOwner.Player, position, false, "Invalid loadout index.");
                return false;
            }

            PieceDefinition pieceDefinition = selectedLoadout[loadoutIndex];
            if (IsLoadoutIndexAlreadyPlaced(loadoutIndex))
            {
                PublishPlacement(pieceDefinition, PieceOwner.Player, position, false, "This loadout piece is already placed.");
                return false;
            }

            string reason;
            if (!placementValidator.CanPlace(PieceOwner.Player, position, out reason))
            {
                PublishPlacement(pieceDefinition, PieceOwner.Player, position, false, reason);
                return false;
            }

            GameObject pieceObject = new GameObject("Player_" + pieceDefinition.DisplayName + "_" + loadoutIndex);
            pieceObject.transform.SetParent(piecesRoot, false);

            PieceController pieceController = pieceObject.AddComponent<PieceController>();
            if (!pieceController.Initialize(pieceDefinition, PieceOwner.Player, position, boardManager))
            {
                DestroyImmediate(pieceObject);
                PublishPlacement(pieceDefinition, PieceOwner.Player, position, false, "Piece initialization failed.");
                return false;
            }

            placedPieces.Add(pieceController);
            placedLoadoutIndexes.Add(loadoutIndex);
            PublishPlacement(pieceDefinition, PieceOwner.Player, position, true, "Piece placed.");
            return true;
        }

        public bool CanStartBattle()
        {
            return HasRequiredKing()
                && selectedLoadout.Count > 0
                && placedPieces.Count == selectedLoadout.Count
                && CurrentCost <= maxLoadoutCost;
        }

public bool TryStartBattle()
        {
            if (!CanStartBattle())
            {
                Debug.LogWarning("Cannot start battle. Loadout and placement are not complete.");
                return false;
            }

            if (gameManager != null)
            {
                if (gameManager.CurrentState != GameState.Preparation)
                {
                    Debug.LogWarning("Cannot start battle because GameManager is not in Preparation state.");
                    return false;
                }

                gameManager.StartBattle();
            }

            return true;
        }

        [ContextMenu("Debug/Add MVP Loadout")]
        private void DebugAddMvpLoadout()
        {
            ClearPreparation();
            AddAvailablePiece(PieceType.King);
            AddAvailablePiece(PieceType.Rook);
            AddAvailablePiece(PieceType.Knight);
            AddAvailablePiece(PieceType.Pawn);
            AddAvailablePiece(PieceType.Pawn);
        }

        [ContextMenu("Debug/Place MVP Loadout")]
        private void DebugPlaceMvpLoadout()
        {
            TryPlaceSelectedPiece(0, new GridPosition(2, 0));
            TryPlaceSelectedPiece(1, new GridPosition(0, 0));
            TryPlaceSelectedPiece(2, new GridPosition(1, 0));
            TryPlaceSelectedPiece(3, new GridPosition(3, 1));
            TryPlaceSelectedPiece(4, new GridPosition(4, 1));
        }

        private bool AddAvailablePiece(PieceType pieceType)
        {
            for (int i = 0; i < availablePieces.Length; i++)
            {
                PieceDefinition piece = availablePieces[i];
                if (piece != null && piece.PieceType == pieceType)
                {
                    return TryAddPieceToLoadout(piece);
                }
            }

            Debug.LogWarning("Available piece not found: " + pieceType);
            return false;
        }

private bool IsLoadoutIndexAlreadyPlaced(int loadoutIndex)
        {
            return placedLoadoutIndexes.Contains(loadoutIndex);
        }

        private bool HasRequiredKing()
        {
            for (int i = 0; i < selectedLoadout.Count; i++)
            {
                if (selectedLoadout[i] != null && selectedLoadout[i].PieceType == PieceType.King)
                {
                    return true;
                }
            }

            return false;
        }

        private int CalculateCurrentCost()
        {
            int cost = 0;
            for (int i = 0; i < selectedLoadout.Count; i++)
            {
                if (selectedLoadout[i] != null)
                {
                    cost += Mathf.Max(0, selectedLoadout[i].LoadoutCost);
                }
            }

            return cost;
        }

        private void ClearPlacedPieces()
        {
            for (int i = placedPieces.Count - 1; i >= 0; i--)
            {
                PieceController piece = placedPieces[i];
                if (piece != null)
                {
                    DestroyImmediate(piece.gameObject);
                }
            }

            placedPieces.Clear();
            placedLoadoutIndexes.Clear();

            if (boardManager != null)
            {
                boardManager.InitializeBoard();
            }
        }

        private void PublishLoadoutChanged(PieceDefinition changedPiece)
        {
            Action<LoadoutEventData> handler = OnLoadoutChanged;
            if (handler != null)
            {
                handler(new LoadoutEventData(changedPiece, CurrentCost, maxLoadoutCost, selectedLoadout.Count, HasRequiredKing()));
            }
        }

        private void PublishPlacement(PieceDefinition pieceDefinition, PieceOwner owner, GridPosition position, bool success, string message)
        {
            Debug.Log(message);

            Action<PlacementEventData> handler = OnPiecePlaced;
            if (handler != null)
            {
                handler(new PlacementEventData(pieceDefinition, owner, position, success, message));
            }
        }

        private void EnsureReferences()
        {
            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }

            if (placementValidator == null)
            {
                placementValidator = FindFirstObjectByType<PlacementValidator>();
            }

            if (placementValidator == null)
            {
                placementValidator = gameObject.AddComponent<PlacementValidator>();
            }

            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            if (piecesRoot == null)
            {
                GameObject rootObject = GameObject.Find("PiecesRoot");
                if (rootObject == null)
                {
                    rootObject = new GameObject("PiecesRoot");
                }

                piecesRoot = rootObject.transform;
            }
        }
    }
}
