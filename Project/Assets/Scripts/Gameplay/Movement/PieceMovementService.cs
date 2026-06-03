using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Movement
{
    public sealed class PieceMovementService : MonoBehaviour
    {
        [SerializeField]
        private BoardManager boardManager;

        private readonly MovementBehaviourResolver resolver = new MovementBehaviourResolver();

        private void Awake()
        {
            EnsureBoardManager();
        }

        public List<GridPosition> GetValidPositions(PieceController piece)
        {
            EnsureBoardManager();

            if (piece == null || piece.Definition == null || boardManager == null || piece.IsCaptured)
            {
                return new List<GridPosition>();
            }

            IMovementBehaviour movementBehaviour = resolver.Resolve(piece.Definition.PieceType);
            return movementBehaviour.GetValidPositions(piece, boardManager);
        }

        public bool CanMoveTo(PieceController piece, GridPosition targetPosition)
        {
            List<GridPosition> validPositions = GetValidPositions(piece);
            for (int i = 0; i < validPositions.Count; i++)
            {
                if (validPositions[i] == targetPosition)
                {
                    return true;
                }
            }

            return false;
        }

public bool IsPositionThreatenedBy(GridPosition position, PieceOwner threateningOwner)
        {
            PieceController[] pieces = FindObjectsByType<PieceController>(FindObjectsSortMode.None);
            for (int i = 0; i < pieces.Length; i++)
            {
                PieceController piece = pieces[i];
                if (piece == null || piece.IsCaptured || piece.Owner != threateningOwner)
                {
                    continue;
                }

                if (CanMoveTo(piece, position))
                {
                    return true;
                }
            }

            return false;
        }


        private void EnsureBoardManager()
        {
            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }
        }
    }
}
