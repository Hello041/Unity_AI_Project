using System;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.EnemyAI
{
    [Serializable]
    public sealed class EnemyActionDelay
    {
        [SerializeField]
        private float actionInterval = 1.25f;

        [SerializeField]
        private float remainingTime;

        public float ActionInterval
        {
            get { return actionInterval; }
        }

        public float RemainingTime
        {
            get { return remainingTime; }
        }

        public bool Tick(float deltaTime)
        {
            actionInterval = Mathf.Max(0.1f, actionInterval);
            remainingTime -= deltaTime;
            if (remainingTime > 0f)
            {
                return false;
            }

            remainingTime = actionInterval;
            return true;
        }

        public void Reset()
        {
            remainingTime = actionInterval;
        }

        public void TriggerSoon()
        {
            remainingTime = 0f;
        }
    }
}
