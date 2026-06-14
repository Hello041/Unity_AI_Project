using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Movement;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.EnemyAI
{
    public sealed class EnemyMoveSelector
    {
        public enum EnemyMoveKind
        {
            None,
            PlayerKingCapture,
            OpeningPawnMove,
            HighestValueCapture,
            RandomMove
        }

        private readonly HashSet<int> openedPawnIds = new HashSet<int>();

        public readonly struct EnemyMove
        {
            public EnemyMove(PieceController piece, GridPosition targetPosition, PieceController targetPiece, bool isCapture, EnemyMoveKind moveKind)
            {
                Piece = piece;
                TargetPosition = targetPosition;
                TargetPiece = targetPiece;
                IsCapture = isCapture;
                MoveKind = moveKind;
            }

            public PieceController Piece { get; }
            public GridPosition TargetPosition { get; }
            public PieceController TargetPiece { get; }
            public bool IsCapture { get; }
            public EnemyMoveKind MoveKind { get; }
            public bool IsValid { get { return Piece != null; } }
        }

public EnemyMove SelectMove(BoardManager boardManager, PieceMovementService movementService)
        {
            if (boardManager == null || movementService == null)
            {
                return default;
            }

            PieceController[] pieces = Object.FindObjectsByType<PieceController>(FindObjectsSortMode.None);
            List<EnemyMove> openingPawnMoves = new List<EnemyMove>();
            List<EnemyMove> nonKingRandomMoves = new List<EnemyMove>();
            List<EnemyMove> kingRandomMoves = new List<EnemyMove>();
            EnemyMove bestNonKingCapture = default;
            EnemyMove bestKingCapture = default;
            int bestNonKingCaptureValue = -1;
            int bestKingCaptureValue = -1;

            for (int i = 0; i < pieces.Length; i++)
            {
                PieceController piece = pieces[i];
                if (!CanAct(piece))
                {
                    continue;
                }

                bool isKingAttacker = IsKing(piece);
                List<GridPosition> validPositions = movementService.GetValidPositions(piece);
                for (int positionIndex = 0; positionIndex < validPositions.Count; positionIndex++)
                {
                    GridPosition targetPosition = validPositions[positionIndex];
                    PieceController targetPiece = boardManager.GetOccupant(targetPosition) as PieceController;
                    bool isCapture = targetPiece != null && targetPiece.Owner == PieceOwner.Player;

                    if (isCapture && IsPlayerKing(targetPiece))
                    {
                        return new EnemyMove(piece, targetPosition, targetPiece, true, EnemyMoveKind.PlayerKingCapture);
                    }

                    if (ShouldUseOpeningPawnMove(piece, isCapture))
                    {
                        openingPawnMoves.Add(new EnemyMove(piece, targetPosition, null, false, EnemyMoveKind.OpeningPawnMove));
                    }

                    if (isCapture)
                    {
                        int captureValue = PieceValueTable.GetValue(targetPiece.Definition);
                        if (isKingAttacker)
                        {
                            if (!bestKingCapture.IsValid || captureValue > bestKingCaptureValue)
                            {
                                bestKingCapture = new EnemyMove(piece, targetPosition, targetPiece, true, EnemyMoveKind.HighestValueCapture);
                                bestKingCaptureValue = captureValue;
                            }
                        }
                        else if (!bestNonKingCapture.IsValid || captureValue > bestNonKingCaptureValue)
                        {
                            bestNonKingCapture = new EnemyMove(piece, targetPosition, targetPiece, true, EnemyMoveKind.HighestValueCapture);
                            bestNonKingCaptureValue = captureValue;
                        }
                    }
                    else if (isKingAttacker)
                    {
                        kingRandomMoves.Add(new EnemyMove(piece, targetPosition, null, false, EnemyMoveKind.RandomMove));
                    }
                    else
                    {
                        nonKingRandomMoves.Add(new EnemyMove(piece, targetPosition, null, false, EnemyMoveKind.RandomMove));
                    }
                }
            }

            if (bestNonKingCapture.IsValid
                && (!bestKingCapture.IsValid || bestNonKingCaptureValue >= bestKingCaptureValue))
            {
                return bestNonKingCapture;
            }

            if (bestKingCapture.IsValid)
            {
                return bestKingCapture;
            }

            if (openingPawnMoves.Count > 0)
            {
                return openingPawnMoves[Random.Range(0, openingPawnMoves.Count)];
            }

            if (nonKingRandomMoves.Count > 0)
            {
                return nonKingRandomMoves[Random.Range(0, nonKingRandomMoves.Count)];
            }

            if (kingRandomMoves.Count == 0)
            {
                return default;
            }

            return kingRandomMoves[Random.Range(0, kingRandomMoves.Count)];
        }

public void MarkMoveCompleted(EnemyMove selectedMove)
        {
            if (selectedMove.Piece == null || selectedMove.MoveKind != EnemyMoveKind.OpeningPawnMove)
            {
                return;
            }

            openedPawnIds.Add(selectedMove.Piece.GetInstanceID());
        }

        public void ResetOpeningMemory()
        {
            openedPawnIds.Clear();
        }


        private bool CanAct(PieceController piece)
        {
            if (piece == null || piece.IsCaptured || piece.Owner != PieceOwner.Enemy || piece.Definition == null)
            {
                return false;
            }

            return piece.Cooldown == null || piece.Cooldown.IsReady;
        }

        private bool IsPlayerKing(PieceController piece)
        {
            return piece != null
                && piece.Owner == PieceOwner.Player
                && piece.Definition != null
                && piece.Definition.PieceType == PieceType.King;
        }
    

private bool ShouldUseOpeningPawnMove(PieceController piece, bool isCapture)
        {
            if (isCapture || piece == null || piece.Definition == null || piece.Definition.PieceType != PieceType.Pawn)
            {
                return false;
            }

            return !openedPawnIds.Contains(piece.GetInstanceID());
        }


private bool IsKing(PieceController piece)
        {
            return piece != null
                && piece.Definition != null
                && piece.Definition.PieceType == PieceType.King;
        }
}
}
