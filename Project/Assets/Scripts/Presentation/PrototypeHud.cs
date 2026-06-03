using TacticalRoguelike.Core;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Cooldown;
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
        private EnemySetupManager enemySetupManager;

        [SerializeField]
        private PlayerGlobalCooldown playerGlobalCooldown;

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

        private void OnGUI()
        {
            if (!showHud)
            {
                return;
            }

            EnsureReferences();

            GUILayout.BeginArea(new Rect(12f, 12f, 320f, 420f), GUI.skin.box);
            GUILayout.Label("Tactical Roguelike MVP");
            GUILayout.Space(4f);

            DrawResultBanner();
            DrawStatus();
            GUILayout.Space(8f);
            DrawButtons();
            GUILayout.Space(8f);
            GUILayout.Label(lastMessage);
            GUILayout.EndArea();
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
                enemySetupManager.SpawnRandomSetup();
            }

            bool started = preparationManager.TryStartBattle();
            lastMessage = started ? "Battle started." : "Cannot start battle yet.";
        }

public void ResetPrototype()
        {
            EnsureReferences();
            ClearAllPieces();

            if (gameManager != null)
            {
                gameManager.EnterBoot();
                gameManager.StartStage();
                gameManager.StartPreparation();
            }

            if (preparationManager != null)
            {
                preparationManager.ClearPreparation();
            }

            if (manualPlacementController != null)
            {
                manualPlacementController.ClearSelection();
            }

            if (playerGlobalCooldown != null)
            {
                playerGlobalCooldown.ResetCooldown();
            }

            lastMessage = "Prototype reset to Preparation.";
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



            if (manualPlacementController != null)
            {
                string selected = manualPlacementController.SelectedPieceDefinition != null
                    ? manualPlacementController.SelectedPieceDefinition.DisplayName
                    : "None";
                GUILayout.Label("Manual Place: " + selected);
            }
            if (preparationManager != null)
            {
                GUILayout.Label("Loadout: " + preparationManager.CurrentCost + " / " + preparationManager.MaxLoadoutCost
                    + " | Placed: " + preparationManager.PlacedCount + " / " + preparationManager.SelectedCount);
            }

            if (enemySetupManager != null)
            {
                string setupName = enemySetupManager.ActiveSetup != null ? enemySetupManager.ActiveSetup.PatternName : "None";
                GUILayout.Label("Enemy: " + setupName + " | Count: " + enemySetupManager.SpawnedEnemyCount);
            }

            if (playerGlobalCooldown != null)
            {
                GUILayout.Label("Global CD: " + playerGlobalCooldown.RemainingTime.ToString("0.00"));
            }
        }

private void DrawButtons()
        {
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

            if (GUILayout.Button("Reset Prototype"))
            {
                ResetPrototype();
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


private void DrawResultBanner()
        {
            if (gameManager == null)
            {
                return;
            }

            if (gameManager.CurrentState == GameState.StageClear)
            {
                GUILayout.Label("RESULT: STAGE CLEAR");
            }
            else if (gameManager.CurrentState == GameState.GameOver)
            {
                GUILayout.Label("RESULT: GAME OVER");
            }
        }
}
}
