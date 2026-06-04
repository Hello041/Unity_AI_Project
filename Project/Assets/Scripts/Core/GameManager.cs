using System;
using TacticalRoguelike.Data;
using TacticalRoguelike.Gameplay.Health;
using UnityEngine;

namespace TacticalRoguelike.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField]
        private string stageId = "MVP_STAGE_01";

        [SerializeField]
        private int playerStartingHp = 3;

        [SerializeField]
        private PlayerHealthService playerHealthService;

        [SerializeField]
        private StageDefinition stageDefinition;

        [SerializeField]
        private bool autoEnterPreparationOnStart = true;

        private GameState currentState = GameState.Boot;

        public event Action<StageEventData> OnStageStarted;
        public event Action<StageEventData> OnPreparationStarted;
        public event Action<StageEventData> OnBattleStarted;
        public event Action<StageEventData> OnStageCleared;
        public event Action<StageEventData> OnGameOver;
        public event Action<GameState> OnStateChanged;

        public GameState CurrentState
        {
            get { return currentState; }
        }

        public PlayerHealthService PlayerHealth
        {
            get { return playerHealthService; }
        }

        private void Awake()
        {
            if (playerHealthService == null)
            {
                playerHealthService = GetComponent<PlayerHealthService>();
            }

            if (playerHealthService == null)
            {
                playerHealthService = gameObject.AddComponent<PlayerHealthService>();
            }
        }

        private void OnEnable()
        {
            if (playerHealthService != null)
            {
                playerHealthService.OnHealthDepleted += HandlePlayerHealthDepleted;
            }
        }

        private void OnDisable()
        {
            if (playerHealthService != null)
            {
                playerHealthService.OnHealthDepleted -= HandlePlayerHealthDepleted;
            }
        }

private void Start()
        {
            EnterBoot();

            if (autoEnterPreparationOnStart)
            {
                StartStage();
            }
        }

        public void EnterBoot()
        {
            ChangeState(GameState.Boot);
        }

public void StartStage()
        {
            if (currentState != GameState.Boot)
            {
                Debug.LogWarning("StartStage can only be called from Boot state.");
                return;
            }

            ChangeState(GameState.StageStart);

            int maxHealth = stageDefinition != null ? stageDefinition.PlayerMaxHealth : playerStartingHp;
            playerHealthService.ResetForStage(maxHealth);
            PublishStageEvent(OnStageStarted, "Stage started.");

            StartPreparation();
        }

        public void StartPreparation()
        {
            if (currentState != GameState.StageStart)
            {
                Debug.LogWarning("StartPreparation can only be called from StageStart state.");
                return;
            }

            ChangeState(GameState.Preparation);
            PublishStageEvent(OnPreparationStarted, "Preparation started.");
        }

        public void StartBattle()
        {
            if (currentState != GameState.Preparation)
            {
                Debug.LogWarning("StartBattle can only be called from Preparation state.");
                return;
            }

            ChangeState(GameState.Playing);
            PublishStageEvent(OnBattleStarted, "Battle started.");
        }

        public void NotifyEnemyKingCaptured()
        {
            if (currentState != GameState.Playing)
            {
                Debug.LogWarning("Enemy king capture is only valid while Playing.");
                return;
            }

            ChangeState(GameState.StageClear);
            PublishStageEvent(OnStageCleared, "Enemy king captured. Stage clear.");
        }

public void NotifyPlayerKingCaptured()
        {
            if (currentState != GameState.Playing)
            {
                Debug.LogWarning("Player king capture is only valid while Playing.");
                return;
            }

            playerHealthService.TakeDamage();
        }

        [ContextMenu("Debug/Start Battle")]
        private void DebugStartBattle()
        {
            StartBattle();
        }

        [ContextMenu("Debug/Player King Captured")]
        private void DebugPlayerKingCaptured()
        {
            NotifyPlayerKingCaptured();
        }

        [ContextMenu("Debug/Enemy King Captured")]
        private void DebugEnemyKingCaptured()
        {
            NotifyEnemyKingCaptured();
        }

private void HandlePlayerHealthDepleted(HealthEventData data)
        {
            if (currentState != GameState.Playing)
            {
                return;
            }

            ChangeState(GameState.GameOver);
            PublishStageEvent(OnGameOver, "Player HP depleted. Game over.");
        }

        private void ChangeState(GameState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }

            currentState = nextState;
            Debug.Log("GameState changed: " + currentState);

            Action<GameState> handler = OnStateChanged;
            if (handler != null)
            {
                handler(currentState);
            }
        }

private void PublishStageEvent(Action<StageEventData> stageEvent, string message)
        {
            string activeStageId = stageDefinition != null && !string.IsNullOrWhiteSpace(stageDefinition.StageName)
                ? stageDefinition.StageName
                : stageId;
            StageEventData data = new StageEventData(currentState, activeStageId, message);
            Debug.Log(message);

            if (stageEvent != null)
            {
                stageEvent(data);
            }
        }
    }
}
