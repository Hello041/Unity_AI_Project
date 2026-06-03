using UnityEngine;

namespace TacticalRoguelike.Gameplay.Cooldown
{
    public sealed class PlayerGlobalCooldown : MonoBehaviour
    {
        [SerializeField]
        private float cooldownDuration = 0.4f;

        [SerializeField]
        private float remainingTime;

        public float CooldownDuration
        {
            get { return cooldownDuration; }
        }

        public float RemainingTime
        {
            get { return remainingTime; }
        }

        public bool IsReady
        {
            get { return remainingTime <= 0f; }
        }

        private void Update()
        {
            if (remainingTime <= 0f)
            {
                return;
            }

            remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
        }

        public void StartCooldown()
        {
            remainingTime = Mathf.Max(0f, cooldownDuration);
        }

        public void ResetCooldown()
        {
            remainingTime = 0f;
        }
    }
}
