using UnityEngine;

namespace TacticalRoguelike.Gameplay.Pieces
{
    public sealed class PieceView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Vector3 localVisualOffset = new Vector3(0f, 0.55f, 0f);

        private void Awake()
        {
            EnsureSpriteRenderer();
        }

        public void SetSprite(Sprite sprite)
        {
            EnsureSpriteRenderer();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        public void FaceCamera(Camera targetCamera)
        {
            if (targetCamera == null)
            {
                return;
            }

            transform.rotation = targetCamera.transform.rotation;
        }

        private void EnsureSpriteRenderer()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (spriteRenderer == null)
            {
                GameObject visualObject = new GameObject("Sprite");
                visualObject.transform.SetParent(transform, false);
                visualObject.transform.localPosition = localVisualOffset;
                spriteRenderer = visualObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 10;
            }
        }
    }
}
