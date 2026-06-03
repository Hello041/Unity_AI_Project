using System;
using TacticalRoguelike.Core;
using TacticalRoguelike.Gameplay.Cooldown;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Movement;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Interaction
{
    public sealed class PieceActionController : MonoBehaviour
    {
        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private PieceMovementService movementService;

        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private PlayerGlobalCooldown playerGlobalCooldown;

        public event Action<PieceController, GridPosition> OnPieceMoved;
        public event Action<PieceController, PieceController, GridPosition> OnPieceCaptured;

        private void Awake()
        {
            EnsureReferences();
        }

public bool TryMoveSelectedPiece(PieceController selectedPiece, GridPosition targetPosition)
        {
            EnsureReferences();

            if (selectedPiece == null || selectedPiece.IsCaptured)
            {
                return false;
            }

            if (gameManager != null && gameManager.CurrentState != GameState.Playing)
            {
                Debug.LogWarning("Pieces can only move while the game is Playing.");
                return false;
            }

            if (movementService == null || boardManager == null)
            {
                Debug.LogError("PieceActionController requires BoardManager and PieceMovementService.");
                return false;
            }

            if (!CanUseCooldowns(selectedPiece))
            {
                return false;
            }

            if (!movementService.CanMoveTo(selectedPiece, targetPosition))
            {
                return false;
            }

            PieceController targetPiece = boardManager.GetOccupant(targetPosition) as PieceController;
            if (targetPiece != null)
            {
                if (targetPiece.Owner == selectedPiece.Owner)
                {
                    return false;
                }

                HandleCapture(selectedPiece, targetPiece, targetPosition);
            }

            if (!selectedPiece.TryMoveTo(targetPosition))
            {
                return false;
            }

            StartCooldowns(selectedPiece);
            HandlePlayerKingThreatAfterMove(selectedPiece);

            Action<PieceController, GridPosition> movedHandler = OnPieceMoved;
            if (movedHandler != null)
            {
                movedHandler(selectedPiece, targetPosition);
            }

            return true;
        }

        private void HandleCapture(PieceController attacker, PieceController targetPiece, GridPosition targetPosition)
        {
            bool capturedKing = targetPiece.Definition != null && targetPiece.Definition.PieceType == PieceType.King;
            PieceOwner capturedOwner = targetPiece.Owner;

            targetPiece.Capture();

            Action<PieceController, PieceController, GridPosition> capturedHandler = OnPieceCaptured;
            if (capturedHandler != null)
            {
                capturedHandler(attacker, targetPiece, targetPosition);
            }

            if (!capturedKing || gameManager == null)
            {
                return;
            }

            if (capturedOwner == PieceOwner.Enemy)
            {
                gameManager.NotifyEnemyKingCaptured();
            }
            else
            {
                gameManager.NotifyPlayerKingCaptured();
            }
        }

private void EnsureReferences()
        {
            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }

            if (movementService == null)
            {
                movementService = FindFirstObjectByType<PieceMovementService>();
            }

            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            if (playerGlobalCooldown == null)
            {
                playerGlobalCooldown = FindFirstObjectByType<PlayerGlobalCooldown>();
            }
        }
    

private bool CanUseCooldowns(PieceController selectedPiece)
        {
            if (selectedPiece.Cooldown != null && !selectedPiece.Cooldown.IsReady)
            {
                return false;
            }

            if (selectedPiece.Owner == PieceOwner.Player && playerGlobalCooldown != null && !playerGlobalCooldown.IsReady)
            {
                return false;
            }

            return true;
        }

        private void StartCooldowns(PieceController selectedPiece)
        {
            if (selectedPiece.Cooldown != null)
            {
                selectedPiece.Cooldown.StartCooldown();
            }

            if (selectedPiece.Owner == PieceOwner.Player && playerGlobalCooldown != null)
            {
                playerGlobalCooldown.StartCooldown();
            }
        }


private void HandlePlayerKingThreatAfterMove(PieceController selectedPiece)
        {
            if (selectedPiece == null
                || selectedPiece.Owner != PieceOwner.Player
                || selectedPiece.Definition == null
                || selectedPiece.Definition.PieceType != PieceType.King
                || movementService == null
                || gameManager == null
                || gameManager.CurrentState != GameState.Playing)
            {
                return;
            }

            if (movementService.IsPositionThreatenedBy(selectedPiece.GridPosition, PieceOwner.Enemy))
            {
                gameManager.NotifyPlayerKingCaptured();
            }
        }
}
}
