using TacticalRoguelike.Gameplay.Cooldown;
using TacticalRoguelike.Gameplay.Board;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Pieces
{
    public sealed class PieceController : MonoBehaviour
    {
        [SerializeField]
        private PieceDefinition definition;

        [SerializeField]
        private PieceOwner owner;

        [SerializeField]
        private GridPosition gridPosition;

        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private PieceView pieceView;

        [SerializeField]
        private PieceCooldown pieceCooldown;

        [SerializeField]
        private float worldHeightOffset = 0.4f;

        private bool isCaptured;
        private bool isPlacedOnBoard;

        public PieceDefinition Definition
        {
            get { return definition; }
        }

        public PieceOwner Owner
        {
            get { return owner; }
        }

        public GridPosition GridPosition
        {
            get { return gridPosition; }
        }

        public PieceCooldown Cooldown
        {
            get
            {
                EnsureReferences();
                return pieceCooldown;
            }
        }

        public bool IsCaptured
        {
            get { return isCaptured; }
        }

        private void Awake()
        {
            EnsureReferences();
        }

private void Start()
        {
            RegisterSerializedPlacement();
        }


        private void OnDestroy()
        {
            ClearBoardOccupancy();
        }

public bool Initialize(PieceDefinition pieceDefinition, PieceOwner pieceOwner, GridPosition startPosition, BoardManager targetBoard)
        {
            EnsureReferences();

            if (pieceDefinition == null || targetBoard == null)
            {
                Debug.LogError("PieceController initialization requires a definition and BoardManager.");
                return false;
            }

            if (!targetBoard.IsValidPosition(startPosition))
            {
                Debug.LogWarning("Invalid piece start position: " + startPosition);
                return false;
            }

            if (!targetBoard.TrySetOccupant(startPosition, this))
            {
                Debug.LogWarning("Board position is already occupied: " + startPosition);
                return false;
            }

            definition = pieceDefinition;
            owner = pieceOwner;
            gridPosition = startPosition;
            boardManager = targetBoard;
            isCaptured = false;
            isPlacedOnBoard = true;

            transform.position = GetBoardWorldPosition();
            ConfigureCooldown();
            ApplyView();
            return true;
        }

        public void Capture()
        {
            if (isCaptured)
            {
                return;
            }

            isCaptured = true;
            ClearBoardOccupancy();
            EnsureReferences();

            bool isKing = definition != null && definition.PieceType == PieceType.King;
            if (pieceView != null)
            {
                pieceView.PlayCaptureFeedback(isKing, DeactivateAfterCapture);
                return;
            }

            DeactivateAfterCapture();
        }

        private void DeactivateAfterCapture()
        {
            gameObject.SetActive(false);
        }


        private void ApplyView()
        {
            EnsureReferences();

            if (pieceView != null && definition != null)
            {
                pieceView.SetSprite(definition.GetSprite(owner));
                pieceView.FaceCamera(Camera.main);
            }
        }

private void EnsureReferences()
        {
            if (pieceView == null)
            {
                pieceView = GetComponent<PieceView>();
            }

            if (pieceView == null)
            {
                pieceView = gameObject.AddComponent<PieceView>();
            }

            if (pieceCooldown == null)
            {
                pieceCooldown = GetComponent<PieceCooldown>();
            }

            if (pieceCooldown == null)
            {
                pieceCooldown = gameObject.AddComponent<PieceCooldown>();
            }
        }

        private void ClearBoardOccupancy()
        {
            if (!isPlacedOnBoard || boardManager == null)
            {
                return;
            }

            boardManager.ClearOccupant(gridPosition, this);
            isPlacedOnBoard = false;
        }
    

private void RegisterSerializedPlacement()
        {
            if (isCaptured || definition == null || boardManager == null)
            {
                ConfigureCooldown();
                ApplyView();
                return;
            }

            if (!boardManager.IsValidPosition(gridPosition))
            {
                Debug.LogWarning("Invalid serialized piece position: " + gridPosition, this);
                return;
            }

            if (isPlacedOnBoard && boardManager.GetOccupant(gridPosition) == this)
            {
                transform.position = GetBoardWorldPosition();
                ConfigureCooldown();
                ApplyView();
                return;
            }

            isPlacedOnBoard = false;
            if (boardManager.TrySetOccupant(gridPosition, this))
            {
                isPlacedOnBoard = true;
                transform.position = GetBoardWorldPosition();
            }
            else if (boardManager.GetOccupant(gridPosition) == this)
            {
                isPlacedOnBoard = true;
                transform.position = GetBoardWorldPosition();
            }
            else
            {
                Debug.LogWarning("Could not register serialized piece occupancy: " + gridPosition, this);
            }

            ConfigureCooldown();
            ApplyView();
        }


private Vector3 GetBoardWorldPosition()
        {
            if (boardManager == null)
            {
                return transform.position;
            }

            return boardManager.GridToWorldCenter(gridPosition) + Vector3.up * worldHeightOffset;
        }


public bool TryMoveTo(GridPosition targetPosition)
        {
            if (isCaptured || boardManager == null || !boardManager.IsValidPosition(targetPosition))
            {
                return false;
            }

            Component occupant = boardManager.GetOccupant(targetPosition);
            if (occupant != null && occupant != this)
            {
                return false;
            }

            boardManager.ClearOccupant(gridPosition, this);
            gridPosition = targetPosition;

            if (!boardManager.TrySetOccupant(gridPosition, this))
            {
                Debug.LogWarning("Failed to register moved piece at: " + gridPosition, this);
                return false;
            }

            isPlacedOnBoard = true;
            transform.position = GetBoardWorldPosition();
            ApplyView();
            return true;
        }


private void ConfigureCooldown()
        {
            EnsureReferences();

            if (pieceCooldown != null && definition != null)
            {
                pieceCooldown.Configure(definition.BaseCooldown);
            }
        }
}
}
