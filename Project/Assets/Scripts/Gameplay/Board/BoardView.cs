using System.Collections.Generic;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private Transform tileRoot;

        [SerializeField]
        private Color lightTileColor = new Color(0.78f, 0.72f, 0.58f, 1f);

        [SerializeField]
        private Color darkTileColor = new Color(0.36f, 0.28f, 0.2f, 1f);

        [SerializeField]
        private float tileThickness = 0.05f;

        private readonly Dictionary<GridPosition, BoardTileView> tiles = new Dictionary<GridPosition, BoardTileView>();

        private void Awake()
        {
            if (boardManager == null)
            {
                boardManager = GetComponent<BoardManager>();
            }
        }

        private void Start()
        {
            GenerateBoard();
        }

[ContextMenu("Generate Board")]
        public void GenerateBoard()
        {
            if (boardManager == null)
            {
                boardManager = GetComponent<BoardManager>();
            }

            if (boardManager == null)
            {
                Debug.LogError("BoardView requires a BoardManager reference.");
                return;
            }

            boardManager.InitializeBoard();
            ClearExistingTiles();
            EnsureTileRoot();

            for (int y = 0; y < boardManager.BoardHeight; y++)
            {
                for (int x = 0; x < boardManager.BoardWidth; x++)
                {
                    CreateTile(new GridPosition(x, y));
                }
            }
        }

        public BoardTileView GetTile(GridPosition position)
        {
            BoardTileView tile;
            tiles.TryGetValue(position, out tile);
            return tile;
        }

        public void ClearHighlights()
        {
            foreach (BoardTileView tile in tiles.Values)
            {
                tile.SetHighlight(BoardHighlightType.None);
            }
        }

        public void SetHighlight(GridPosition position, BoardHighlightType highlightType)
        {
            BoardTileView tile = GetTile(position);
            if (tile != null)
            {
                tile.SetHighlight(highlightType);
            }
        }

        private void CreateTile(GridPosition position)
        {
            GameObject tileObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tileObject.name = "Tile_" + position.X + "_" + position.Y;
            tileObject.transform.SetParent(tileRoot, false);
            tileObject.transform.position = boardManager.GridToWorldCenter(position);
            tileObject.transform.localScale = new Vector3(boardManager.TileSize, tileThickness, boardManager.TileSize);

            MeshRenderer renderer = tileObject.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            Color color = ((position.X + position.Y) % 2 == 0) ? lightTileColor : darkTileColor;
            BoardTileView tileView = tileObject.AddComponent<BoardTileView>();
            tileView.Initialize(position, color);
            tiles[position] = tileView;
        }

        private void EnsureTileRoot()
        {
            if (tileRoot != null)
            {
                return;
            }

            GameObject rootObject = new GameObject("Board Tiles");
            rootObject.transform.SetParent(transform, false);
            tileRoot = rootObject.transform;
        }

        private void ClearExistingTiles()
        {
            tiles.Clear();

            if (tileRoot == null)
            {
                return;
            }

            for (int i = tileRoot.childCount - 1; i >= 0; i--)
            {
                Transform child = tileRoot.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
