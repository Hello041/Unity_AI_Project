using System;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Health
{
    public sealed class PlayerHealthService : MonoBehaviour
    {
        [SerializeField]
        private int startingHp = 3;

        private int currentHp;
        private bool initialized;

        public event Action<HealthEventData> OnHealthChanged;
        public event Action<HealthEventData> OnHealthDepleted;

        public int CurrentHp
        {
            get { return currentHp; }
        }

        public int MaxHp
        {
            get { return startingHp; }
        }

        public bool IsDepleted
        {
            get { return initialized && currentHp <= 0; }
        }

        public void Initialize(int hp)
        {
            startingHp = Mathf.Max(1, hp);
            currentHp = startingHp;
            initialized = true;
            PublishHealthChanged(0);
        }

        public void ResetHealth()
        {
            Initialize(startingHp);
        }

        public void ApplyPlayerKingCapturedDamage()
        {
            ApplyDamage(1);
        }

        public void ApplyDamage(int amount)
        {
            if (!initialized)
            {
                ResetHealth();
            }

            if (currentHp <= 0)
            {
                return;
            }

            int safeAmount = Mathf.Max(0, amount);
            if (safeAmount == 0)
            {
                return;
            }

            currentHp = Mathf.Max(0, currentHp - safeAmount);
            HealthEventData data = PublishHealthChanged(safeAmount);

            if (currentHp <= 0)
            {
                Action<HealthEventData> handler = OnHealthDepleted;
                if (handler != null)
                {
                    handler(data);
                }
            }
        }

        private HealthEventData PublishHealthChanged(int damageAmount)
        {
            HealthEventData data = new HealthEventData(currentHp, startingHp, damageAmount);
            Action<HealthEventData> handler = OnHealthChanged;
            if (handler != null)
            {
                handler(data);
            }

            return data;
        }
    }
}
