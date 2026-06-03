using UnityEngine;

namespace TacticalRoguelike.Gameplay.Board
{
    public sealed class BoardManager : MonoBehaviour
    {
        [SerializeField]
        private int boardWidth = 6;

        [SerializeField]
        private int boardHeight = 6;

        [SerializeField]
        private float tileSize = 1f;

        [SerializeField]
        private Vector3 boardOrigin = Vector3.zero;

        private Component[,] occupants;

        public int BoardWidth
        {
            get { return boardWidth; }
        }

        public int BoardHeight
        {
            get { return boardHeight; }
        }

        public float TileSize
        {
            get { return tileSize; }
        }

        private void Awake()
        {
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            boardWidth = Mathf.Max(1, boardWidth);
            boardHeight = Mathf.Max(1, boardHeight);
            tileSize = Mathf.Max(0.1f, tileSize);
            occupants = new Component[boardWidth, boardHeight];
        }

        public bool IsValidPosition(GridPosition position)
        {
            return position.X >= 0
                && position.Y >= 0
                && position.X < boardWidth
                && position.Y < boardHeight;
        }

        public Vector3 GridToWorld(GridPosition position)
        {
            return boardOrigin + new Vector3(position.X * tileSize, 0f, position.Y * tileSize);
        }

        public Vector3 GridToWorldCenter(GridPosition position)
        {
            return GridToWorld(position) + new Vector3(tileSize * 0.5f, 0f, tileSize * 0.5f);
        }

        public GridPosition WorldToGrid(Vector3 worldPosition)
        {
            Vector3 local = worldPosition - boardOrigin;
            int x = Mathf.FloorToInt(local.x / tileSize);
            int y = Mathf.FloorToInt(local.z / tileSize);
            return new GridPosition(x, y);
        }

        public bool IsOccupied(GridPosition position)
        {
            if (!IsValidPosition(position))
            {
                return false;
            }

            EnsureOccupants();
            return occupants[position.X, position.Y] != null;
        }

        public Component GetOccupant(GridPosition position)
        {
            if (!IsValidPosition(position))
            {
                return null;
            }

            EnsureOccupants();
            return occupants[position.X, position.Y];
        }

        public bool TrySetOccupant(GridPosition position, Component occupant)
        {
            if (!IsValidPosition(position) || occupant == null)
            {
                return false;
            }

            EnsureOccupants();
            if (occupants[position.X, position.Y] != null)
            {
                return false;
            }

            occupants[position.X, position.Y] = occupant;
            return true;
        }

        public void ClearOccupant(GridPosition position, Component occupant)
        {
            if (!IsValidPosition(position))
            {
                return;
            }

            EnsureOccupants();
            if (occupants[position.X, position.Y] == occupant)
            {
                occupants[position.X, position.Y] = null;
            }
        }

        public bool IsPlayerPlacementRow(GridPosition position)
        {
            return IsValidPosition(position) && position.Y >= 0 && position.Y <= 1;
        }

        public bool IsEnemyPlacementRow(GridPosition position)
        {
            return IsValidPosition(position) && position.Y >= boardHeight - 2 && position.Y < boardHeight;
        }

        private void EnsureOccupants()
        {
            if (occupants == null || occupants.GetLength(0) != boardWidth || occupants.GetLength(1) != boardHeight)
            {
                InitializeBoard();
            }
        }
    }
}
