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

        

        public string LastMessage
        {
            get { return lastMessage; }
        }

        public bool IsHudVisible
        {
            get { return showHud; }
        }

        public void SetHudVisible(bool visible)
        {
            showHud = visible;
        }
private string lastMessage = "준비되었습니다.";

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
                gameManager.OnStageAdvanceRequested += HandleStageAdvanceRequested;
            }
        }

private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.OnBattleResetRequested -= HandleBattleResetRequested;
                gameManager.OnStageAdvanceRequested -= HandleStageAdvanceRequested;
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

            if (state == GameState.StageClear || state == GameState.Victory || state == GameState.GameOver)
            {
                DrawResultScreen(state);
                return;
            }

            DrawGameplayHud();
        }

        public void SetupPlayerMvpLoadoutAndPlacement()
        {
            EnsureReferences();
            if (preparationManager == null || gameManager == null)
            {
                lastMessage = "Preparation setup is unavailable.";
                return;
            }

            preparationManager.ClearPreparation();
            if (enemySetupManager != null && enemySetupManager.ActiveSetup != null)
            {
                enemySetupManager.RespawnActiveSetup();
            }

            int stage = gameManager.CurrentStage;
            bool added = preparationManager.TryAddPieceToLoadout(kingDefinition)
                && preparationManager.TryAddPieceToLoadout(rookDefinition);

            if (added && stage >= 2)
            {
                added = preparationManager.TryAddPieceToLoadout(knightDefinition);
            }

            if (added && stage >= 3)
            {
                added = preparationManager.TryAddPieceToLoadout(pawnDefinition)
                    && preparationManager.TryAddPieceToLoadout(pawnDefinition);
            }

            bool placed = added
                && preparationManager.TryPlaceSelectedPiece(0, new GridPosition(2, 0))
                && preparationManager.TryPlaceSelectedPiece(1, new GridPosition(0, 0));

            if (placed && stage >= 2)
            {
                placed = preparationManager.TryPlaceSelectedPiece(2, new GridPosition(1, 0));
            }

            if (placed && stage >= 3)
            {
                placed = preparationManager.TryPlaceSelectedPiece(3, new GridPosition(3, 1))
                    && preparationManager.TryPlaceSelectedPiece(4, new GridPosition(4, 1));
            }

            lastMessage = added && placed
                ? LocalizeStageLabel(gameManager.CurrentStageLabel) + " 추천 편성을 배치했습니다."
                : "플레이어 배치에 실패했습니다.";
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

            if (enemySetupManager == null || gameManager == null)
            {
                lastMessage = "Stage encounter is unavailable.";
                return;
            }

            if (enemySetupManager.SpawnedEnemyCount == 0
                && !enemySetupManager.RespawnActiveSetup())
            {
                lastMessage = "Stage encounter failed to spawn.";
                return;
            }

            bool started = preparationManager.TryStartBattle();
            lastMessage = started ? "전투를 시작했습니다." : "아직 전투를 시작할 수 없습니다.";
        }

public void ResetPrototype()
        {
            RestartCurrentSession();
        }

private void DrawStatus()
        {
            GameState state = gameManager != null ? gameManager.CurrentState : GameState.Boot;

            if (gameManager != null && gameManager.CurrentStage > 0)
            {
                GUILayout.Label("현재 " + gameManager.CurrentStageLabel);
            }
            GUILayout.Label("상태: " + LocalizeState(state));

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
                string selected = "없음";
                if (manualPlacementController.SelectedPieceDefinition != null)
                {
                    selected = LocalizePieceName(manualPlacementController.SelectedPieceDefinition);
                }
                else if (manualPlacementController.SelectedPlacedPiece != null
                    && manualPlacementController.SelectedPlacedPiece.Definition != null)
                {
                    selected = LocalizePieceName(manualPlacementController.SelectedPlacedPiece.Definition) + " (위치 변경)";
                }

                GUILayout.Label("현재 선택: " + selected);
            }
            else if (state == GameState.Playing && boardInputController != null)
            {
                PieceController selectedPiece = boardInputController.SelectedPiece;
                string selected = selectedPiece != null && selectedPiece.Definition != null
                    ? LocalizePieceName(selectedPiece.Definition)
                    : "없음";
                GUILayout.Label("현재 선택: " + selected);
            }

            if (preparationManager != null)
            {
                GUILayout.Label("편성 비용: " + preparationManager.CurrentCost + " / " + preparationManager.MaxLoadoutCost
                    + " | 배치: " + preparationManager.PlacedCount + " / " + preparationManager.SelectedCount);
            }

            if (enemySetupManager != null)
            {
                string setupName = enemySetupManager.ActiveSetup != null ? LocalizePatternName(enemySetupManager.ActiveSetup.PatternName) : "없음";
                GUILayout.Label("적 편성: " + setupName);
                GUILayout.Label("남은 적: " + enemySetupManager.SpawnedEnemyCount);
            }

            if (playerGlobalCooldown != null)
            {
                GUILayout.Label("글로벌 쿨다운: " + (playerGlobalCooldown.IsReady ? "준비 완료" : playerGlobalCooldown.RemainingTime.ToString("0.00") + "초"));
            }

            GUILayout.Label(state == GameState.Preparation ? "상태: 전투 준비" : "상태: 전투 진행 중");
        }

private void DrawButtons()
        {
            GameState state = gameManager != null ? gameManager.CurrentState : GameState.Boot;
            if (state != GameState.Preparation)
            {
                GUILayout.Label("전투가 시작되었습니다. 보드에서 플레이어 말을 선택하세요.");
                return;
            }

            GUILayout.Label("수동 배치");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("킹")) SelectManualPlacementPiece(kingDefinition);
            if (GUILayout.Button("룩")) SelectManualPlacementPiece(rookDefinition);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("나이트")) SelectManualPlacementPiece(knightDefinition);
            if (GUILayout.Button("폰")) SelectManualPlacementPiece(pawnDefinition);
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("빠른 설정");
            if (GUILayout.Button("1. 추천 배치")) SetupPlayerMvpLoadoutAndPlacement();
            if (GUILayout.Button("2. 전투 시작")) StartBattle();
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

            GUILayout.BeginArea(new Rect(12f, 12f, 360f, 490f), GUI.skin.box);
            GUILayout.Label("택티컬 로그라이크 체스");
            GUILayout.Space(4f);
            DrawButtons();
            GUILayout.Space(8f);
            DrawStatus();
            GUILayout.Space(8f);
            GUILayout.Label("안내: " + lastMessage);
            GUILayout.EndArea();
        }


private void DrawResultScreen(GameState state)
        {
            const float width = 440f;
            const float height = 310f;
            Rect area = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            GUILayout.BeginArea(area, GUI.skin.window);
            GUILayout.FlexibleSpace();

            if (state == GameState.StageClear)
            {
                GUILayout.Label(gameManager != null ? LocalizeStageLabel(gameManager.CurrentStageLabel) + " 클리어" : "스테이지 클리어", GUI.skin.box);
                GUILayout.Space(12f);
                GUILayout.Label("다음 스테이지로 이동합니다...");
                GUILayout.FlexibleSpace();
                GUILayout.EndArea();
                return;
            }

            bool isVictory = state == GameState.Victory;
            GUILayout.Label(isVictory ? "승리" : "게임 오버", GUI.skin.box);
            GUILayout.Space(12f);
            GUILayout.Label(isVictory ? "모든 스테이지를 클리어했습니다." : "도전이 종료되었습니다.");

            if (gameManager != null && gameManager.PlayerHealth != null)
            {
                GUILayout.Label("현재 HP: " + gameManager.PlayerHealth.CurrentHp + " / " + gameManager.PlayerHealth.MaxHp);
            }

            GUILayout.Space(16f);
            if (GUILayout.Button("다시 시작", GUILayout.Height(40f))) RestartCurrentSession();
            if (GUILayout.Button("타이틀로", GUILayout.Height(40f))) ReturnToTitle();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }


private void DrawTitleScreen()
        {
            const float width = 440f;
            const float height = 230f;
            Rect area = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            GUILayout.BeginArea(area, GUI.skin.window);
            GUILayout.FlexibleSpace();
            GUILayout.Label("택티컬 로그라이크 체스", GUI.skin.box);
            GUILayout.Space(16f);
            GUILayout.Label("6x6 실시간 체스 전투");
            GUILayout.Space(16f);
            if (GUILayout.Button("게임 시작", GUILayout.Height(44f))) StartGame();
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
            lastMessage = "준비되었습니다.";
        }


public void RestartCurrentSession()
        {
            ResetSessionState(true);
            lastMessage = "세션을 다시 시작했습니다. 전투 준비가 완료되었습니다.";
        }


public void StartGame()
        {
            ResetSessionState(true);
            lastMessage = "전투 준비가 완료되었습니다.";
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
            lastMessage = "플레이어 킹이 잡혔습니다. 같은 편성을 다시 배치하세요.";
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
            const float width = 270f;
            const float height = 125f;
            GUILayout.BeginArea(new Rect(12f, Screen.height - height - 12f, width, height), GUI.skin.box);
            GUILayout.Label("전투 상태");

            if (gameManager != null) GUILayout.Label("현재 " + gameManager.CurrentStageLabel);
            if (gameManager != null && gameManager.PlayerHealth != null)
            {
                GUILayout.Label("HP: " + gameManager.PlayerHealth.CurrentHp + " / " + gameManager.PlayerHealth.MaxHp);
            }

            string aiState = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove
                ? "적 AI: 첫 이동 대기 중"
                : "적 AI: 활성화";
            GUILayout.Label(aiState);
            GUILayout.EndArea();
        }


private void DrawSelectedPiecePanel()
        {
            const float width = 280f;
            const float height = 150f;
            GUILayout.BeginArea(new Rect(Screen.width - width - 12f, Screen.height - height - 12f, width, height), GUI.skin.box);
            GUILayout.Label("선택한 말");

            PieceController selected = boardInputController != null ? boardInputController.SelectedPiece : null;
            if (selected == null || selected.Definition == null)
            {
                GUILayout.Label("없음");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label("이름: " + LocalizePieceName(selected.Definition));
            GUILayout.Label("종류: " + LocalizePieceType(selected.Definition.PieceType));
            GUILayout.Label(selected.Cooldown == null || selected.Cooldown.IsReady
                ? "쿨다운: 준비 완료"
                : "쿨다운: " + selected.Cooldown.RemainingTime.ToString("0.00") + "초 남음");
            GUILayout.EndArea();
        }


private void DrawGlobalCooldownPanel()
        {
            const float width = 280f;
            float top = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove ? 96f : 12f;
            GUILayout.BeginArea(new Rect(Screen.width - width - 12f, top, width, 118f), GUI.skin.box);
            GUILayout.Label("글로벌 쿨다운");

            float duration = playerGlobalCooldown != null ? playerGlobalCooldown.CooldownDuration : 0f;
            float remaining = playerGlobalCooldown != null ? playerGlobalCooldown.RemainingTime : 0f;
            float progress = duration > 0f ? 1f - Mathf.Clamp01(remaining / duration) : 1f;
            int percentage = Mathf.RoundToInt(progress * 100f);
            GUILayout.Label(remaining > 0f
                ? remaining.ToString("0.00") + "초 남음 | " + percentage + "%"
                : "준비 완료 | 100%");

            Rect barRect = GUILayoutUtility.GetRect(width - 24f, 24f);
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
            float top = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove ? 96f : 12f;
            GUILayout.BeginArea(new Rect(12f, top, 240f, 200f), GUI.skin.box);
            GUILayout.Label("적 팀 구성");

            EnemySetupDefinition setup = enemySetupManager != null ? enemySetupManager.ActiveSetup : null;
            if (setup == null)
            {
                GUILayout.Label("활성 편성 없음");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label(LocalizePatternName(setup.PatternName));
            EnemySpawnEntry[] entries = setup.SpawnEntries;
            if (entries != null)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    PieceDefinition piece = entries[i] != null ? entries[i].PieceDefinition : null;
                    if (piece != null) GUILayout.Label("- " + LocalizePieceName(piece));
                }
            }
            GUILayout.EndArea();
        }


private void DrawFirstMoveNotice()
        {
            const float width = 480f;
            Rect area = new Rect((Screen.width - width) * 0.5f, 12f, width, 74f);
            GUILayout.BeginArea(area, GUI.skin.window);
            GUILayout.Label("전투 시작", GUI.skin.box);
            GUILayout.Label("첫 이동을 완료하면 적 AI가 활성화됩니다.", GUILayout.Height(24f));
            GUILayout.EndArea();
        }


private void HandleStageAdvanceRequested(StageEventData data)
        {
            ResetBattleRuntimeState();
            lastMessage = "다음 스테이지 준비가 완료되었습니다.";
        }


public void SelectKingForPlacement()
        {
            SelectManualPlacementPiece(kingDefinition);
        }

        public void SelectRookForPlacement()
        {
            SelectManualPlacementPiece(rookDefinition);
        }

        public void SelectKnightForPlacement()
        {
            SelectManualPlacementPiece(knightDefinition);
        }

        public void SelectPawnForPlacement()
        {
            SelectManualPlacementPiece(pawnDefinition);
        }


private static string LocalizeState(GameState state)
        {
            switch (state)
            {
                case GameState.Boot: return "타이틀";
                case GameState.StageStart: return "스테이지 시작";
                case GameState.Preparation: return "전투 준비";
                case GameState.Playing: return "전투 중";
                case GameState.StageClear: return "스테이지 클리어";
                case GameState.Victory: return "승리";
                case GameState.GameOver: return "게임 오버";
                default: return state.ToString();
            }
        }

        private static string LocalizePieceName(PieceDefinition definition)
        {
            return definition == null ? "없음" : LocalizePieceType(definition.PieceType);
        }

        private static string LocalizePieceType(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.King: return "킹";
                case PieceType.Rook: return "룩";
                case PieceType.Knight: return "나이트";
                case PieceType.Pawn: return "폰";
                default: return pieceType.ToString();
            }
        }

        private static string LocalizeStageLabel(string stageLabel)
        {
            if (string.IsNullOrEmpty(stageLabel)) return "스테이지";
            return stageLabel.Replace("Stage", "스테이지");
        }


private static string LocalizePatternName(string patternName)
        {
            if (string.IsNullOrEmpty(patternName)) return "없음";
            if (patternName.Contains("PatternA")) return "패턴 A - 킹/룩 방어 진형";
            if (patternName.Contains("PatternB")) return "패턴 B - 킹/나이트 방어 진형";
            if (patternName.Contains("PatternC")) return "패턴 C - 정예 방어 진형";
            return patternName;
        }
}
}
