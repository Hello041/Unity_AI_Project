using System;
using System.Collections;
using TacticalRoguelike.Gameplay.Cooldown;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Pieces
{
    public sealed class PieceView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Vector3 localVisualOffset = new Vector3(0f, 0.55f, 0f);

        [SerializeField]
        [Range(0.1f, 1f)]
        private float cooldownDarkness = 0.4f;

        [SerializeField]
        private Color playerCooldownTint = new Color(0.35f, 0.55f, 0.95f, 1f);

        [SerializeField]
        private Color enemyCooldownTint = new Color(0.95f, 0.35f, 0.35f, 1f);

        [SerializeField]
        [Range(0.15f, 0.45f)]
        private float readyFlashDuration = 0.3f;

        [SerializeField]
        [Range(1f, 1.8f)]
        private float readyFlashBrightness = 1.55f;

        [SerializeField]
        [Range(0.1f, 0.4f)]
        private float captureDuration = 0.28f;

        [SerializeField]
        [Range(1.15f, 1.5f)]
        private float captureScale = 1.35f;

        [SerializeField]
        [Range(0.15f, 0.7f)]
        private float kingCaptureDuration = 0.5f;

        [SerializeField]
        [Range(1.25f, 2.0f)]
        private float kingCaptureScale = 1.75f;

        private PieceController pieceController;
        private PieceCooldown pieceCooldown;
        private Color originalColor = Color.white;

        private bool cooldownStateInitialized;
        private bool wasCooldownReady;
        private float readyFlashRemaining;

        private bool isPlayingCaptureFeedback;

        private Vector3 originalLocalScale = Vector3.one;
        private Coroutine captureFeedbackRoutine;
        private bool originalColorCaptured;

        private void Awake()
        {
            originalLocalScale = transform.localScale;
            EnsureSpriteRenderer();
            EnsurePieceController();
            EnsureCooldown();
            CaptureOriginalColor();
        }

        private void Update()
        {
            EnsureSpriteRenderer();
            EnsurePieceController();
            EnsureCooldown();
            CaptureOriginalColor();

            if (!isPlayingCaptureFeedback)
            {
                UpdateCooldownVisuals();
            }
        }

        private void OnDisable()
        {
            readyFlashRemaining = 0f;
            cooldownStateInitialized = false;
            isPlayingCaptureFeedback = false;
            captureFeedbackRoutine = null;
            transform.localScale = originalLocalScale;
            RestoreOriginalColor();
        }


        public void SetSprite(Sprite sprite)
        {
            EnsureSpriteRenderer();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        public void PlayCaptureFeedback(bool isKing, Action onCompleted)
        {
            EnsureSpriteRenderer();
            CaptureOriginalColor();

            if (captureFeedbackRoutine != null)
            {
                StopCoroutine(captureFeedbackRoutine);
            }

            captureFeedbackRoutine = StartCoroutine(CaptureFeedbackRoutine(isKing, onCompleted));
        }

        public void FaceCamera(Camera targetCamera)
        {
            if (targetCamera == null)
            {
                return;
            }

            transform.rotation = targetCamera.transform.rotation;
        }

        private IEnumerator CaptureFeedbackRoutine(bool isKing, Action onCompleted)
        {
            isPlayingCaptureFeedback = true;
            readyFlashRemaining = 0f;

            float duration = isKing ? kingCaptureDuration : captureDuration;
            float targetScaleMultiplier = isKing ? kingCaptureScale : captureScale;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = startScale * targetScaleMultiplier;
            Color startColor = spriteRenderer != null ? spriteRenderer.color : originalColor;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
                transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

                if (spriteRenderer != null)
                {
                    Color fadedColor = startColor;
                    fadedColor.a = Mathf.Lerp(startColor.a, 0f, progress);
                    spriteRenderer.color = fadedColor;
                }

                yield return null;
            }

            transform.localScale = targetScale;
            if (spriteRenderer != null)
            {
                Color fadedColor = startColor;
                fadedColor.a = 0f;
                spriteRenderer.color = fadedColor;
            }

            captureFeedbackRoutine = null;
            if (onCompleted != null)
            {
                onCompleted();
            }
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



        private void EnsurePieceController()
        {
            if (pieceController == null)
            {
                pieceController = GetComponent<PieceController>();
            }
        }

        private void EnsureCooldown()
        {
            if (pieceCooldown == null)
            {
                pieceCooldown = GetComponent<PieceCooldown>();
            }
        }

        private void CaptureOriginalColor()
        {
            if (spriteRenderer == null || originalColorCaptured)
            {
                return;
            }

            originalColor = spriteRenderer.color;
            originalColorCaptured = true;
        }

        private void UpdateCooldownVisuals()
        {
            if (spriteRenderer == null || !originalColorCaptured)
            {
                return;
            }

            bool isReady = pieceCooldown == null
                || pieceCooldown.IsReady
                || pieceCooldown.CooldownDuration <= 0f;

            if (!cooldownStateInitialized)
            {
                wasCooldownReady = isReady;
                cooldownStateInitialized = true;
            }

            if (!isReady)
            {
                wasCooldownReady = false;
                readyFlashRemaining = 0f;
                ApplyCooldownTint();
                return;
            }

            if (!wasCooldownReady && IsPlayerPiece())
            {
                readyFlashRemaining = readyFlashDuration;
            }

            wasCooldownReady = true;
            ApplyReadyFlash();
        }

        private void ApplyCooldownTint()
        {
            float recoveryProgress = 1f - Mathf.Clamp01(pieceCooldown.RemainingTime / pieceCooldown.CooldownDuration);
            Color teamTint = GetCooldownTint();
            Color cooldownColor = new Color(
                originalColor.r * cooldownDarkness * teamTint.r,
                originalColor.g * cooldownDarkness * teamTint.g,
                originalColor.b * cooldownDarkness * teamTint.b,
                originalColor.a);
            spriteRenderer.color = Color.Lerp(cooldownColor, originalColor, recoveryProgress);
        }

        private void ApplyReadyFlash()
        {
            if (readyFlashRemaining <= 0f || readyFlashDuration <= 0f)
            {
                readyFlashRemaining = 0f;
                RestoreOriginalColor();
                return;
            }

            float flashProgress = Mathf.Clamp01(readyFlashRemaining / readyFlashDuration);
            float brightness = Mathf.Lerp(1f, readyFlashBrightness, flashProgress);
            spriteRenderer.color = new Color(
                originalColor.r * brightness,
                originalColor.g * brightness,
                originalColor.b * brightness,
                originalColor.a);
            readyFlashRemaining = Mathf.Max(0f, readyFlashRemaining - Time.deltaTime);
        }

        private bool IsPlayerPiece()
        {
            return pieceController != null && pieceController.Owner == PieceOwner.Player;
        }

        private Color GetCooldownTint()
        {
            if (pieceController == null)
            {
                return Color.white;
            }

            return pieceController.Owner == PieceOwner.Player
                ? playerCooldownTint
                : enemyCooldownTint;
        }

        private void RestoreOriginalColor()
        {
            if (spriteRenderer != null && originalColorCaptured)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }
}
