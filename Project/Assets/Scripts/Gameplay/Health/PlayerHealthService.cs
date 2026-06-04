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
        private bool depletionPublished;

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
            ResetForStage(hp);
        }

public void ResetForStage(int maxHealth)
        {
            startingHp = Mathf.Max(1, maxHealth);
            currentHp = startingHp;
            initialized = true;
            depletionPublished = false;
            PublishHealthChanged(0);
        }


public void ResetHealth()
        {
            ResetForStage(startingHp);
        }

public void ApplyPlayerKingCapturedDamage()
        {
            TakeDamage();
        }

public void ApplyDamage(int amount)
        {
            TakeDamage(amount);
        }

public void TakeDamage(int amount = 1)
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

            if (currentHp == 0 && !depletionPublished)
            {
                depletionPublished = true;
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
