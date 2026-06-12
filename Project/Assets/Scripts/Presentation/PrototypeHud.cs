using TacticalRoguelike.Core;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Cooldown;
using TacticalRoguelike.Gameplay.EnemyAI;
using TacticalRoguelike.Gameplay.Interaction;
using TacticalRoguelike.Gameplay.Pieces;
using TacticalRoguelike.Gameplay.Preparation;
using TacticalRoguelike.Gameplay.Stage;
using UnityEngine;

namespace TacticalRoguelike.Presentation
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private PreparationManager preparationManager;

        [SerializeField]
        private ManualPlacementController manualPlacementController;

        [SerializeField]
        private BoardInputController boardInputController;

        [SerializeField]
        private EnemySetupManager enemySetupManager;

        [SerializeField]
        private PlayerGlobalCooldown playerGlobalCooldown;

        [SerializeField]
        private EnemyAIController enemyAIController;

        [SerializeField]
        private Transform piecesRoot;

        [Header("MVP Piece Definitions")]
        [SerializeField]
        private PieceDefinition kingDefinition;

        [SerializeField]
        private PieceDefinition rookDefinition;

        [SerializeField]
        private PieceDefinition knightDefinition;

        [SerializeField]
        private PieceDefinition pawnDefinition;

        [SerializeField]
        private bool showHud = true;

        private string lastMessage = "Ready.";

        private void Awake()
        {
            EnsureReferences();
        }

private void OnEnable()
        {
            EnsureReferences();
            if (gameManager != null)
            {
                gameManager.OnBattleResetRequested += HandleBattleResetRequested;
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.OnBattleResetRequested -= HandleBattleResetRequested;
            }
        }

        
private void OnGUI()
        {
            if (!showHud)
            {
                return;
            }

            EnsureReferences();
            GameState state = gameManager != null ? gameManager.CurrentState : GameState.Boot;

            if (state == GameState.Boot)
            {
                DrawTitleScreen();
                return;
            }

            if (state == GameState.StageClear || state == GameState.GameOver)
            {
                DrawResultScreen(state);
                return;
            }

            DrawGameplayHud();
        }

        public void SetupPlayerMvpLoadoutAndPlacement()
        {
            EnsureReferences();
            if (preparationManager == null)
            {
                lastMessage = "PreparationManager missing.";
                return;
            }

            ClearAllPieces();
            preparationManager.ClearPreparation();

            bool added = preparationManager.TryAddPieceToLoadout(kingDefinition)
                && preparationManager.TryAddPieceToLoadout(rookDefinition)
                && preparationManager.TryAddPieceToLoadout(knightDefinition)
                && preparationManager.TryAddPieceToLoadout(pawnDefinition)
                && preparationManager.TryAddPieceToLoadout(pawnDefinition);

            bool placed = preparationManager.TryPlaceSelectedPiece(0, new GridPosition(2, 0))
                && preparationManager.TryPlaceSelectedPiece(1, new GridPosition(0, 0))
                && preparationManager.TryPlaceSelectedPiece(2, new GridPosition(1, 0))
                && preparationManager.TryPlaceSelectedPiece(3, new GridPosition(3, 1))
                && preparationManager.TryPlaceSelectedPiece(4, new GridPosition(4, 1));

            lastMessage = added && placed
                ? "Player MVP loadout placed."
                : "Player setup failed.";
        }

        public void SpawnRandomEnemies()
        {
            EnsureReferences();
            if (enemySetupManager == null)
            {
                lastMessage = "EnemySetupManager missing.";
                return;
            }

            bool spawned = enemySetupManager.SpawnRandomSetup();
            lastMessage = spawned && enemySetupManager.ActiveSetup != null
                ? "Enemy setup: " + enemySetupManager.ActiveSetup.PatternName
                : "Enemy setup failed.";
        }

public void StartBattle()
        {
            EnsureReferences();
            if (preparationManager == null)
            {
                lastMessage = "PreparationManager missing.";
                return;
            }

            if (enemySetupManager != null && enemySetupManager.SpawnedEnemyCount == 0)
            {
                if (enemySetupManager.ActiveSetup != null)
                {
                    enemySetupManager.SpawnSetup(enemySetupManager.ActiveSetup);
                }
                else
                {
                    enemySetupManager.SpawnRandomSetup();
                }
            }

            bool started = preparationManager.TryStartBattle();
            lastMessage = started ? "Battle started." : "Cannot start battle yet.";
        }

public void ResetPrototype()
        {
            RestartCurrentSession();
        }

private void DrawStatus()
        {
            GameState state = gameManager != null ? gameManager.CurrentState : GameState.Boot;
            GUILayout.Label("State: " + state);

            if (gameManager != null && gameManager.PlayerHealth != null)
            {
                GUILayout.Label("HP: " + gameManager.PlayerHealth.CurrentHp + " / " + gameManager.PlayerHealth.MaxHp);
            }
            else
            {
                GUILayout.Label("HP: -");
            }

            if (state == GameState.Preparation && manualPlacementController != null)
            {
                string selected = "None";
                if (manualPlacementController.SelectedPieceDefinition != null)
                {
                    selected = manualPlacementController.SelectedPieceDefinition.DisplayName;
                }
                else if (manualPlacementController.SelectedPlacedPiece != null
                    && manualPlacementController.SelectedPlacedPiece.Definition != null)
                {
                    selected = manualPlacementController.SelectedPlacedPiece.Definition.DisplayName + " (Reposition)";
                }

                GUILayout.Label("Current Selection: " + selected);
            }
            else if (state == GameState.Playing && boardInputController != null)
            {
                PieceController selectedPiece = boardInputController.SelectedPiece;
                string selected = selectedPiece != null && selectedPiece.Definition != null
                    ? selectedPiece.Definition.DisplayName
                    : "None";
                GUILayout.Label("Current Selection: " + selected);
            }

            if (preparationManager != null)
            {
                GUILayout.Label("Loadout: " + preparationManager.CurrentCost + " / " + preparationManager.MaxLoadoutCost
                    + " | Placed: " + preparationManager.PlacedCount + " / " + preparationManager.SelectedCount);
            }

            if (enemySetupManager != null)
            {
                string setupName = enemySetupManager.ActiveSetup != null ? enemySetupManager.ActiveSetup.PatternName : "None";
                GUILayout.Label("Enemy Pattern: " + setupName);
                GUILayout.Label("Enemy Count: " + enemySetupManager.SpawnedEnemyCount);
            }

            if (playerGlobalCooldown != null)
            {
                GUILayout.Label("Global Cooldown: " + playerGlobalCooldown.RemainingTime.ToString("0.00"));
            }

            if (state == GameState.Preparation)
            {
                GUILayout.Label("Status: Preparation Ready");
            }
            else if (state == GameState.Playing)
            {
                GUILayout.Label("Status: Battle In Progress");
            }
        }

private void DrawButtons()
        {
            GameState state = gameManager != null ? gameManager.CurrentState : GameState.Boot;
            if (state != GameState.Preparation)
            {
                GUILayout.Label("Battle started. Select a player piece on the board.");
                return;
            }

            GUILayout.Label("Manual Placement");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("King"))
            {
                SelectManualPlacementPiece(kingDefinition);
            }

            if (GUILayout.Button("Rook"))
            {
                SelectManualPlacementPiece(rookDefinition);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Knight"))
            {
                SelectManualPlacementPiece(knightDefinition);
            }

            if (GUILayout.Button("Pawn"))
            {
                SelectManualPlacementPiece(pawnDefinition);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("Quick Setup");
            if (GUILayout.Button("1. Setup Player MVP"))
            {
                SetupPlayerMvpLoadoutAndPlacement();
            }

            if (GUILayout.Button("2. Spawn Random Enemies"))
            {
                SpawnRandomEnemies();
            }

            if (GUILayout.Button("3. Start Battle"))
            {
                StartBattle();
            }

            if (GUILayout.Button("Reset Preparation"))
            {
                RestartCurrentSession();
            }
        }

        private void ClearAllPieces()
        {
            if (piecesRoot == null)
            {
                return;
            }

            for (int i = piecesRoot.childCount - 1; i >= 0; i--)
            {
                GameObject child = piecesRoot.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }

            if (boardManager != null)
            {
                boardManager.InitializeBoard();
            }
        }

private void EnsureReferences()
        {
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }

            if (manualPlacementController == null)
            {
                manualPlacementController = FindFirstObjectByType<ManualPlacementController>();
            }

            if (boardInputController == null)
            {
                boardInputController = FindFirstObjectByType<BoardInputController>();
            }

            if (preparationManager == null)
            {
                preparationManager = FindFirstObjectByType<PreparationManager>();
            }

            if (enemySetupManager == null)
            {
                enemySetupManager = FindFirstObjectByType<EnemySetupManager>();
            }

            if (playerGlobalCooldown == null)
            {
                playerGlobalCooldown = FindFirstObjectByType<PlayerGlobalCooldown>();
            }

            if (enemyAIController == null)
            {
                enemyAIController = FindFirstObjectByType<EnemyAIController>();
            }

            if (piecesRoot == null)
            {
                GameObject rootObject = GameObject.Find("PiecesRoot");
                if (rootObject != null)
                {
                    piecesRoot = rootObject.transform;
                }
            }
        }
    

private void SelectManualPlacementPiece(PieceDefinition pieceDefinition)
        {
            EnsureReferences();
            if (manualPlacementController == null)
            {
                lastMessage = "ManualPlacementController missing.";
                return;
            }

            manualPlacementController.SelectPieceForPlacement(pieceDefinition);
            lastMessage = manualPlacementController.LastMessage;
        }





private void DrawGameplayHud()
        {
            GameState state = gameManager != null ? gameManager.CurrentState : GameState.Boot;
            if (state == GameState.Playing)
            {
                DrawEnemyTeamPanel();
                DrawGlobalCooldownPanel();
                DrawSelectedPiecePanel();
                DrawBattleStatusPanel();

                if (enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove)
                {
                    DrawFirstMoveNotice();
                }

                return;
            }

            GUILayout.BeginArea(new Rect(12f, 12f, 340f, 460f), GUI.skin.box);
            GUILayout.Label("Tactical Roguelike MVP");
            GUILayout.Space(4f);
            DrawStatus();
            GUILayout.Space(8f);
            DrawButtons();
            GUILayout.Space(8f);
            GUILayout.Label("Feedback: " + lastMessage);
            GUILayout.EndArea();
        }


private void DrawResultScreen(GameState state)
        {
            const float width = 420f;
            const float height = 300f;
            Rect area = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            GUILayout.BeginArea(area, GUI.skin.window);
            GUILayout.FlexibleSpace();
            GUILayout.Label(state == GameState.StageClear ? "STAGE CLEAR" : "GAME OVER", GUI.skin.box);
            GUILayout.Space(12f);

            if (gameManager != null && gameManager.PlayerHealth != null)
            {
                GUILayout.Label("Current HP: " + gameManager.PlayerHealth.CurrentHp + " / " + gameManager.PlayerHealth.MaxHp);
            }

            GUILayout.Space(16f);
            if (GUILayout.Button("Restart", GUILayout.Height(40f)))
            {
                RestartCurrentSession();
            }

            if (GUILayout.Button("Return to Title", GUILayout.Height(40f)))
            {
                ReturnToTitle();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }


private void DrawTitleScreen()
        {
            const float width = 420f;
            const float height = 220f;
            Rect area = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            GUILayout.BeginArea(area, GUI.skin.window);
            GUILayout.FlexibleSpace();
            GUILayout.Label("TACTICAL ROGUELIKE MVP", GUI.skin.box);
            GUILayout.Space(16f);
            GUILayout.Label("6x6 Real-Time Chess Battle");
            GUILayout.Space(16f);
            if (GUILayout.Button("Start Game", GUILayout.Height(44f)))
            {
                StartGame();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }


private void ResetSessionState(bool enterPreparation)
        {
            ResetBattleRuntimeState();

            if (gameManager != null)
            {
                gameManager.EnterBoot();
                if (enterPreparation)
                {
                    gameManager.StartStage();
                }
            }
        }


public void ReturnToTitle()
        {
            ResetSessionState(false);
            lastMessage = "Ready.";
        }


public void RestartCurrentSession()
        {
            ResetSessionState(true);
            lastMessage = "Session restarted. Preparation ready.";
        }


public void StartGame()
        {
            ResetSessionState(true);
            lastMessage = "Preparation ready.";
        }


private void ResetBattleRuntimeState()
        {
            EnsureReferences();

            if (enemySetupManager != null)
            {
                enemySetupManager.ClearSpawnedEnemies();
            }

            if (preparationManager != null)
            {
                preparationManager.ClearPreparation();
            }

            ClearAllPieces();

            if (manualPlacementController != null)
            {
                manualPlacementController.ClearSelection();
            }

            if (boardInputController != null)
            {
                boardInputController.ClearSelection();
            }

            if (playerGlobalCooldown != null)
            {
                playerGlobalCooldown.ResetCooldown();
            }
        }


private void HandleBattleResetRequested(StageEventData data)
        {
            ResetBattleAttemptPreservingEncounter();
            lastMessage = "Player King captured. Re-place the same loadout for another attempt.";
        }


private void ResetBattleAttemptPreservingEncounter()
        {
            EnsureReferences();

            if (enemySetupManager != null)
            {
                enemySetupManager.ClearSpawnedEnemiesForRetry();
            }

            if (preparationManager != null)
            {
                preparationManager.ClearPlacedPiecesForRetry();
            }

            if (manualPlacementController != null)
            {
                manualPlacementController.ClearSelection();
            }

            if (boardInputController != null)
            {
                boardInputController.ClearSelection();
            }

            if (playerGlobalCooldown != null)
            {
                playerGlobalCooldown.ResetCooldown();
            }

            if (boardManager != null)
            {
                boardManager.InitializeBoard();
            }
        }


private void DrawBattleStatusPanel()
        {
            const float width = 250f;
            const float height = 120f;
            GUILayout.BeginArea(new Rect(12f, Screen.height - height - 12f, width, height), GUI.skin.box);
            GUILayout.Label("Battle Status");
            GUILayout.Label("State: " + (gameManager != null ? gameManager.CurrentState.ToString() : "Unknown"));

            if (gameManager != null && gameManager.PlayerHealth != null)
            {
                GUILayout.Label("HP: " + gameManager.PlayerHealth.CurrentHp + " / " + gameManager.PlayerHealth.MaxHp);
            }

            string aiState = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove
                ? "Waiting for first move"
                : "Enemy AI active";
            GUILayout.Label(aiState);
            GUILayout.EndArea();
        }


private void DrawSelectedPiecePanel()
        {
            const float width = 260f;
            const float height = 145f;
            GUILayout.BeginArea(new Rect(Screen.width - width - 12f, Screen.height - height - 12f, width, height), GUI.skin.box);
            GUILayout.Label("Selected Piece");

            PieceController selected = boardInputController != null ? boardInputController.SelectedPiece : null;
            if (selected == null || selected.Definition == null)
            {
                GUILayout.Label("None");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label("Name: " + selected.Definition.DisplayName);
            GUILayout.Label("Type: " + selected.Definition.PieceType);

            if (selected.Cooldown == null || selected.Cooldown.IsReady)
            {
                GUILayout.Label("Cooldown: Ready");
            }
            else
            {
                GUILayout.Label("Cooldown: " + selected.Cooldown.RemainingTime.ToString("0.00") + "s");
            }

            GUILayout.EndArea();
        }


private void DrawGlobalCooldownPanel()
        {
            const float width = 260f;
            float top = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove ? 92f : 12f;
            GUILayout.BeginArea(new Rect(Screen.width - width - 12f, top, width, 105f), GUI.skin.box);
            GUILayout.Label("Global Cooldown");

            float duration = playerGlobalCooldown != null ? playerGlobalCooldown.CooldownDuration : 0f;
            float remaining = playerGlobalCooldown != null ? playerGlobalCooldown.RemainingTime : 0f;
            float progress = duration > 0f ? 1f - Mathf.Clamp01(remaining / duration) : 1f;
            string state = remaining > 0f ? remaining.ToString("0.00") + "s" : "Ready";
            GUILayout.Label(state);

            Rect barRect = GUILayoutUtility.GetRect(width - 24f, 22f);
            GUI.Box(barRect, GUIContent.none);
            Rect fillRect = new Rect(barRect.x + 2f, barRect.y + 2f, (barRect.width - 4f) * progress, barRect.height - 4f);
            Color previousColor = GUI.color;
            GUI.color = remaining > 0f ? new Color(0.35f, 0.75f, 1f) : new Color(0.35f, 0.9f, 0.45f);
            GUI.Box(fillRect, GUIContent.none);
            GUI.color = previousColor;

            GUILayout.EndArea();
        }


private void DrawEnemyTeamPanel()
        {
            float top = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove ? 92f : 12f;
            GUILayout.BeginArea(new Rect(12f, top, 220f, 190f), GUI.skin.box);
            GUILayout.Label("Enemy Team");

            EnemySetupDefinition setup = enemySetupManager != null ? enemySetupManager.ActiveSetup : null;
            if (setup == null)
            {
                GUILayout.Label("No active setup");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label(setup.PatternName);
            EnemySpawnEntry[] entries = setup.SpawnEntries;
            if (entries != null)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    PieceDefinition piece = entries[i] != null ? entries[i].PieceDefinition : null;
                    if (piece != null)
                    {
                        GUILayout.Label("- " + piece.DisplayName);
                    }
                }
            }

            GUILayout.EndArea();
        }


private void DrawFirstMoveNotice()
        {
            const float width = 460f;
            Rect area = new Rect((Screen.width - width) * 0.5f, 12f, width, 70f);
            GUILayout.BeginArea(area, GUI.skin.window);
            GUILayout.Label("BATTLE START", GUI.skin.box);
            GUILayout.Label("Make your first move to begin the battle.", GUILayout.Height(24f));
            GUILayout.EndArea();
        }
}
}
