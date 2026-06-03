using UnityEngine;

namespace TacticalRoguelike.Gameplay.Board
{
    public sealed class BoardTileView : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        private GridPosition gridPosition;
        private Color baseColor;

        public GridPosition GridPosition
        {
            get { return gridPosition; }
        }

        public void Initialize(GridPosition position, Color color)
        {
            gridPosition = position;
            baseColor = color;

            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            ApplyColor(baseColor);
        }

        public void SetHighlight(BoardHighlightType highlightType)
        {
            switch (highlightType)
            {
                case BoardHighlightType.Placement:
                    ApplyColor(new Color(0.25f, 0.7f, 1f, 1f));
                    break;
                case BoardHighlightType.Selected:
                    ApplyColor(new Color(1f, 0.9f, 0.25f, 1f));
                    break;
                case BoardHighlightType.ValidMove:
                    ApplyColor(new Color(0.25f, 1f, 0.35f, 1f));
                    break;
                case BoardHighlightType.Capture:
                    ApplyColor(new Color(1f, 0.25f, 0.25f, 1f));
                    break;
                default:
                    ApplyColor(baseColor);
                    break;
            }
        }

        private void ApplyColor(Color color)
        {
            if (meshRenderer == null)
            {
                return;
            }

            meshRenderer.sharedMaterial.color = color;
        }
    }
}
