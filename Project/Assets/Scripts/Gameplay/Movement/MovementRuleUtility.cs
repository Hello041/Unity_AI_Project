using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Movement
{
    public static class MovementRuleUtility
    {
        public static bool TryAddIfAvailable(List<GridPosition> results, PieceController movingPiece, BoardManager boardManager, GridPosition position)
        {
            if (results == null || movingPiece == null || boardManager == null)
            {
                return false;
            }

            if (!boardManager.IsValidPosition(position))
            {
                return false;
            }

            Component occupant = boardManager.GetOccupant(position);
            if (occupant == null)
            {
                results.Add(position);
                return true;
            }

            PieceController occupantPiece = occupant as PieceController;
            if (occupantPiece != null && occupantPiece.Owner != movingPiece.Owner)
            {
                results.Add(position);
            }

            return false;
        }

        public static bool HasEnemyPiece(PieceController movingPiece, BoardManager boardManager, GridPosition position)
        {
            if (movingPiece == null || boardManager == null || !boardManager.IsValidPosition(position))
            {
                return false;
            }

            PieceController occupantPiece = boardManager.GetOccupant(position) as PieceController;
            return occupantPiece != null && occupantPiece.Owner != movingPiece.Owner;
        }

        public static bool IsEmpty(BoardManager boardManager, GridPosition position)
        {
            return boardManager != null
                && boardManager.IsValidPosition(position)
                && boardManager.GetOccupant(position) == null;
        }
    }
}
