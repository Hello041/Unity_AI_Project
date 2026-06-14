using System.Text;
using TacticalRoguelike.Core;
using TacticalRoguelike.Gameplay.Cooldown;
using TacticalRoguelike.Gameplay.EnemyAI;
using TacticalRoguelike.Gameplay.Interaction;
using TacticalRoguelike.Gameplay.Pieces;
using TacticalRoguelike.Gameplay.Preparation;
using TacticalRoguelike.Gameplay.Stage;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TacticalRoguelike.Presentation
{
    public sealed class CanvasHud : MonoBehaviour
    {
        [SerializeField] private PrototypeHud prototypeHud;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PreparationManager preparationManager;
        [SerializeField] private BoardInputController boardInputController;
        [SerializeField] private EnemySetupManager enemySetupManager;
        [SerializeField] private PlayerGlobalCooldown playerGlobalCooldown;
        [SerializeField] private EnemyAIController enemyAIController;

        private Font font;
        private GameObject titlePanel;
        private GameObject preparationPanel;
        private GameObject battlePanel;
        private GameObject resultPanel;
        private GameObject firstMovePanel;
        private Text preparationText;
        private Text stageText;
        private Text enemyText;
        private Text battleStatusText;
        private Text selectedPieceText;
        private Text cooldownText;
        private Image cooldownFill;
        private Text resultTitleText;
        private Text resultBodyText;

        private Button kingButton;
        private Button rookButton;
        private Button knightButton;
        private Button pawnButton;
        private Text preparationSelectionText;
private GameObject resultButtons;

        private static readonly Color PanelColor = new Color(0.055f, 0.075f, 0.11f, 0.92f);
        private static readonly Color AccentColor = new Color(0.22f, 0.62f, 0.92f, 1f);

        private static readonly Color ButtonColor = new Color(0.18f, 0.48f, 0.72f, 1f);
        private static readonly Color SelectedButtonColor = new Color(0.95f, 0.68f, 0.2f, 1f);
private static readonly Color ReadyColor = new Color(0.25f, 0.8f, 0.45f, 1f);

        private void Awake()
        {
            EnsureReferences();
            BuildUi();
            prototypeHud.SetHudVisible(false);
        }

private void OnEnable()
        {
            if (titlePanel == null)
            {
                RebuildUi();
            }

            BindButtons();
        }

private void RebuildUi()
        {
            EnsureReferences();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            BuildUi();
        }



private void OnDestroy()
        {
            CancelInvoke(nameof(RefreshUi));
            if (prototypeHud != null)
            {
                prototypeHud.SetHudVisible(true);
            }
        }

private void Start()
        {
            InvokeRepeating(nameof(RefreshUi), 0f, 0.1f);
        }

        private void EnsureReferences()
        {
            if (prototypeHud == null) prototypeHud = FindFirstObjectByType<PrototypeHud>();
            if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
            if (preparationManager == null) preparationManager = FindFirstObjectByType<PreparationManager>();
            if (boardInputController == null) boardInputController = FindFirstObjectByType<BoardInputController>();
            if (enemySetupManager == null) enemySetupManager = FindFirstObjectByType<EnemySetupManager>();
            if (playerGlobalCooldown == null) playerGlobalCooldown = FindFirstObjectByType<PlayerGlobalCooldown>();
            if (enemyAIController == null) enemyAIController = FindFirstObjectByType<EnemyAIController>();
        }

        private void BuildUi()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            EnsureEventSystem();
            BuildTitlePanel();
            BuildPreparationPanel();
            BuildBattlePanel();
            BuildResultPanel();
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;

            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
        }

private void BuildTitlePanel()
        {
            titlePanel = CreatePanel("TitlePanel", transform, new Vector2(0.5f, 0.5f), new Vector2(620f, 280f));
            CreateText("Title", titlePanel.transform, "로그라이크 체스", 30, TextAnchor.MiddleCenter,
                new Vector2(24f, -28f), new Vector2(-24f, -86f));
            CreateText("Subtitle", titlePanel.transform, "6x6 실시간 체스 전투", 20, TextAnchor.MiddleCenter,
                new Vector2(24f, -92f), new Vector2(-24f, -132f));
            CreateButton("StartGameButton", titlePanel.transform, "게임 시작", new Vector2(0.5f, 0f),
                new Vector2(280f, 54f), new Vector2(0f, 42f), StartGame);
        }

private void BuildPreparationPanel()
        {
            preparationPanel = CreateStretchPanel("PreparationPanel", transform);

            GameObject infoPanel = CreatePanel("PreparationInfoPanel", preparationPanel.transform,
                new Vector2(0f, 1f), new Vector2(350f, 430f));
            SetCorner(infoPanel, new Vector2(18f, -18f), new Vector2(0f, 1f));

            CreateText("Header", infoPanel.transform, "전투 준비", 27, TextAnchor.MiddleLeft,
                new Vector2(18f, -12f), new Vector2(-18f, -52f));
            preparationSelectionText = CreateText("PreparationSelection", infoPanel.transform, string.Empty, 18,
                TextAnchor.MiddleLeft, new Vector2(18f, -56f), new Vector2(-18f, -90f));
            preparationText = CreateText("PreparationStatus", infoPanel.transform, string.Empty, 17,
                TextAnchor.UpperLeft, new Vector2(18f, -98f), new Vector2(-18f, -410f));

            GameObject actions = CreateStretchPanel("PreparationActions", preparationPanel.transform);

            kingButton = CreateButton("KingButton", actions.transform, "킹", new Vector2(0.5f, 0f),
                new Vector2(120f, 42f), new Vector2(-195f, 120f), SelectKing);
            rookButton = CreateButton("RookButton", actions.transform, "룩", new Vector2(0.5f, 0f),
                new Vector2(120f, 42f), new Vector2(-65f, 120f), SelectRook);
            knightButton = CreateButton("KnightButton", actions.transform, "나이트", new Vector2(0.5f, 0f),
                new Vector2(120f, 42f), new Vector2(65f, 120f), SelectKnight);
            pawnButton = CreateButton("PawnButton", actions.transform, "폰", new Vector2(0.5f, 0f),
                new Vector2(120f, 42f), new Vector2(195f, 120f), SelectPawn);

            CreateButton("QuickSetupButton", actions.transform, "기본 배치", new Vector2(0.5f, 0f),
                new Vector2(250f, 50f), new Vector2(-130f, 58f), SetupPlayer);
            CreateButton("StartBattleButton", actions.transform, "전투 시작", new Vector2(0.5f, 0f),
                new Vector2(250f, 50f), new Vector2(130f, 58f), StartBattle);
        }

private void BuildBattlePanel()
        {
            battlePanel = CreateStretchPanel("BattlePanel", transform);

            GameObject topLeft = CreatePanel("StageEnemyPanel", battlePanel.transform, new Vector2(0f, 1f), new Vector2(320f, 280f));
            SetCorner(topLeft, new Vector2(18f, -18f), new Vector2(0f, 1f));
            stageText = CreateText("StageInfo", topLeft.transform, string.Empty, 22, TextAnchor.UpperLeft,
                new Vector2(18f, -16f), new Vector2(-18f, -84f));
            enemyText = CreateText("EnemyInfo", topLeft.transform, string.Empty, 18, TextAnchor.UpperLeft,
                new Vector2(18f, -92f), new Vector2(-18f, -264f));

            GameObject cooldownPanel = CreatePanel("CooldownPanel", battlePanel.transform, new Vector2(1f, 1f), new Vector2(330f, 132f));
            SetCorner(cooldownPanel, new Vector2(-18f, -18f), new Vector2(1f, 1f));
            cooldownText = CreateText("CooldownText", cooldownPanel.transform, string.Empty, 19, TextAnchor.MiddleLeft,
                new Vector2(18f, -12f), new Vector2(-18f, -62f));
            CreateCooldownBar(cooldownPanel.transform);

            GameObject bottomLeft = CreatePanel("BattleStatusPanel", battlePanel.transform, new Vector2(0f, 0f), new Vector2(330f, 158f));
            SetCorner(bottomLeft, new Vector2(18f, 18f), new Vector2(0f, 0f));
            battleStatusText = CreateText("BattleStatus", bottomLeft.transform, string.Empty, 19, TextAnchor.UpperLeft,
                new Vector2(18f, -16f), new Vector2(-18f, -16f));

            GameObject bottomRight = CreatePanel("SelectedPiecePanel", battlePanel.transform, new Vector2(1f, 0f), new Vector2(340f, 158f));
            SetCorner(bottomRight, new Vector2(-18f, 18f), new Vector2(1f, 0f));
            selectedPieceText = CreateText("SelectedPiece", bottomRight.transform, string.Empty, 19, TextAnchor.UpperLeft,
                new Vector2(18f, -16f), new Vector2(-18f, -16f));

            firstMovePanel = CreatePanel("FirstMovePanel", battlePanel.transform, new Vector2(0.5f, 1f), new Vector2(520f, 90f));
            SetCorner(firstMovePanel, new Vector2(0f, -18f), new Vector2(0.5f, 1f));
            CreateText("FirstMoveText", firstMovePanel.transform, "\n\n전투 시작\n첫 이동을 완료하면 적 AI가 활성화됩니다.", 21,
                TextAnchor.MiddleCenter, new Vector2(14f, -10f), new Vector2(-14f, -10f));
        }

private void BuildResultPanel()
        {
            resultPanel = CreatePanel("ResultPanel", transform, new Vector2(0.5f, 0.5f), new Vector2(540f, 330f));
            resultTitleText = CreateText("ResultTitle", resultPanel.transform, string.Empty, 36, TextAnchor.MiddleCenter,
                new Vector2(24f, -28f), new Vector2(-24f, -86f));
            resultBodyText = CreateText("ResultBody", resultPanel.transform, string.Empty, 20, TextAnchor.MiddleCenter,
                new Vector2(24f, -92f), new Vector2(-24f, -160f));
            resultButtons = new GameObject("ResultButtons", typeof(RectTransform));
            resultButtons.transform.SetParent(resultPanel.transform, false);
            Stretch(resultButtons.GetComponent<RectTransform>());
            CreateButton("RestartButton", resultButtons.transform, "다시 시작", new Vector2(0.5f, 0f),
                new Vector2(340f, 48f), new Vector2(0f, 80f), RestartSession);
            CreateButton("ReturnButton", resultButtons.transform, "타이틀로", new Vector2(0.5f, 0f),
                new Vector2(340f, 48f), new Vector2(0f, 24f), ReturnToTitle);
        }

        private void RefreshUi()
        {
            if (gameManager == null) return;

            GameState state = gameManager.CurrentState;
            titlePanel.SetActive(state == GameState.Boot);
            preparationPanel.SetActive(state == GameState.Preparation);
            battlePanel.SetActive(state == GameState.Playing);
            resultPanel.SetActive(state == GameState.StageClear || state == GameState.GameOver || state == GameState.Victory);

            if (state == GameState.Preparation) RefreshPreparation();
            if (state == GameState.Playing) RefreshBattle();
            if (resultPanel.activeSelf) RefreshResult(state);
        }

private void RefreshPreparation()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("현재 " + LocalizeStageLabel(gameManager.CurrentStageLabel));

            if (preparationManager != null)
            {
                builder.AppendLine("편성 비용: " + preparationManager.CurrentCost + " / " + preparationManager.MaxLoadoutCost);
                builder.AppendLine("배치 완료: " + preparationManager.PlacedCount + " / " + preparationManager.SelectedCount);
            }

            if (gameManager.PlayerHealth != null)
            {
                builder.AppendLine("플레이어 HP: " + gameManager.PlayerHealth.CurrentHp + " / " + gameManager.PlayerHealth.MaxHp);
            }

            if (enemySetupManager != null)
            {
                EnemySetupDefinition setup = enemySetupManager.ActiveSetup;
                builder.AppendLine("적 편성: " + (setup != null ? LocalizePatternName(setup.PatternName) : "없음"));
                builder.AppendLine("남은 적: " + enemySetupManager.SpawnedEnemyCount);

                if (setup != null && setup.SpawnEntries != null)
                {
                    builder.AppendLine("적 팀 구성:");
                    EnemySpawnEntry[] entries = setup.SpawnEntries;
                    for (int i = 0; i < entries.Length; i++)
                    {
                        PieceDefinition piece = entries[i] != null ? entries[i].PieceDefinition : null;
                        if (piece != null)
                        {
                            builder.AppendLine("- " + LocalizePieceName(piece));
                        }
                    }
                }
            }

            string selection = GetPreparationSelectionText();
            preparationSelectionText.text = "선택한 말: " + selection;
            UpdatePreparationButtonColors();

            builder.AppendLine();
            builder.Append("안내: " + (prototypeHud != null ? prototypeHud.LastMessage : "준비되었습니다."));
            preparationText.text = builder.ToString();
        }

private void RefreshBattle()
        {
            stageText.text = LocalizeStageLabel(gameManager.CurrentStageLabel) + "\n상태: 전투 진행 중";
            enemyText.text = BuildEnemyTeamText();

            bool waiting = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove;
            firstMovePanel.SetActive(waiting);
            int hp = gameManager.PlayerHealth != null ? gameManager.PlayerHealth.CurrentHp : 0;
            int maxHp = gameManager.PlayerHealth != null ? gameManager.PlayerHealth.MaxHp : 0;
            battleStatusText.text = "전투 상태\nHP: " + hp + " / " + maxHp + "\n적 AI: "
                + (waiting ? "첫 이동 대기 중" : "활성화");

            PieceController selected = boardInputController != null ? boardInputController.SelectedPiece : null;
            if (selected == null || selected.Definition == null)
            {
                selectedPieceText.text = "선택한 말\n없음";
            }
            else
            {
                string cooldown = selected.Cooldown == null || selected.Cooldown.IsReady
                    ? "준비 완료"
                    : selected.Cooldown.RemainingTime.ToString("0.00") + "초 남음";
                selectedPieceText.text = "선택한 말\n이름: " + LocalizePieceName(selected.Definition)
                    + "\n종류: " + LocalizePieceType(selected.Definition.PieceType) + "\n쿨다운: " + cooldown;
            }

            float duration = playerGlobalCooldown != null ? playerGlobalCooldown.CooldownDuration : 0f;
            float remaining = playerGlobalCooldown != null ? playerGlobalCooldown.RemainingTime : 0f;
            float progress = duration > 0f ? 1f - Mathf.Clamp01(remaining / duration) : 1f;
            int percentage = Mathf.RoundToInt(progress * 100f);
            cooldownText.text = remaining > 0f
                ? "글로벌 쿨다운: 대기 중\n" + remaining.ToString("0.00") + "초 남음  |  " + percentage + "%"
                : "글로벌 쿨다운: 준비 완료\n즉시 이동 가능  |  100%";
            RectTransform cooldownRect = cooldownFill.rectTransform;
            cooldownRect.anchorMax = new Vector2(progress, 1f);
            cooldownRect.offsetMax = Vector2.zero;
            cooldownFill.color = remaining > 0f ? AccentColor : ReadyColor;
        }

private string BuildEnemyTeamText()
        {
            if (enemySetupManager == null || enemySetupManager.ActiveSetup == null) return "적 팀 구성\n활성 편성 없음";

            StringBuilder builder = new StringBuilder("적 팀 구성\n");
            builder.AppendLine(LocalizePatternName(enemySetupManager.ActiveSetup.PatternName));
            EnemySpawnEntry[] entries = enemySetupManager.ActiveSetup.SpawnEntries;
            if (entries != null)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    PieceDefinition piece = entries[i] != null ? entries[i].PieceDefinition : null;
                    if (piece != null) builder.AppendLine("- " + LocalizePieceName(piece));
                }
            }
            return builder.ToString();
        }

private void RefreshResult(GameState state)
        {
            bool stageClear = state == GameState.StageClear;
            bool victory = state == GameState.Victory;
            resultTitleText.text = stageClear ? LocalizeStageLabel(gameManager.CurrentStageLabel) + " 클리어" : victory ? "승리" : "게임 오버";
            resultBodyText.text = stageClear ? "다음 스테이지로 이동합니다..." : victory ? "모든 스테이지를 클리어했습니다." : "도전이 종료되었습니다.";
            resultButtons.SetActive(!stageClear);
        }

        private void StartGame() { if (prototypeHud != null) prototypeHud.StartGame(); }
        private void SelectKing() { if (prototypeHud != null) prototypeHud.SelectKingForPlacement(); }
        private void SelectRook() { if (prototypeHud != null) prototypeHud.SelectRookForPlacement(); }
        private void SelectKnight() { if (prototypeHud != null) prototypeHud.SelectKnightForPlacement(); }
        private void SelectPawn() { if (prototypeHud != null) prototypeHud.SelectPawnForPlacement(); }
        private void SetupPlayer() { if (prototypeHud != null) prototypeHud.SetupPlayerMvpLoadoutAndPlacement(); }
        private void StartBattle() { if (prototypeHud != null) prototypeHud.StartBattle(); }
        private void RestartSession() { if (prototypeHud != null) prototypeHud.RestartCurrentSession(); }
        private void ReturnToTitle() { if (prototypeHud != null) prototypeHud.ReturnToTitle(); }

private GameObject CreatePanel(string objectName, Transform parent, Vector2 anchor, Vector2 size)
        {
            GameObject panel = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            Image image = panel.GetComponent<Image>();
            image.color = PanelColor;
            image.raycastTarget = false;
            return panel;
        }

        private GameObject CreateStretchPanel(string objectName, Transform parent)
        {
            GameObject panel = new GameObject(objectName, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            Stretch(panel.GetComponent<RectTransform>());
            return panel;
        }

        private Text CreateText(string objectName, Transform parent, string value, int fontSize, TextAnchor alignment,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(offsetMin.x, offsetMax.y);
            rect.offsetMax = new Vector2(offsetMax.x, offsetMin.y);
            Text text = textObject.GetComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            
            text.raycastTarget = false;
text.text = value;
            return text;
        }

private Button CreateButton(string objectName, Transform parent, string label, Vector2 anchor, Vector2 size,
            Vector2 position, UnityEngine.Events.UnityAction onClick)
        {
            GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            Image image = buttonObject.GetComponent<Image>();
            image.color = ButtonColor;
            image.raycastTarget = true;
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(0.3f, 0.7f, 0.95f, 1f);
            colors.pressedColor = new Color(0.12f, 0.38f, 0.62f, 1f);
            button.colors = colors;
            button.onClick.AddListener(onClick);

            Text text = CreateText("Label", buttonObject.transform, label, 20, TextAnchor.MiddleCenter,
                new Vector2(8f, -4f), new Vector2(-8f, -4f));
            Stretch(text.rectTransform);
            return button;
        }

private void CreateCooldownBar(Transform parent)
        {
            GameObject background = new GameObject("CooldownBar", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(parent, false);
            RectTransform rect = background.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(18f, 18f);
            rect.offsetMax = new Vector2(-18f, 42f);
            background.GetComponent<Image>().raycastTarget = false;
            background.GetComponent<Image>().color = new Color(0.12f, 0.15f, 0.2f, 1f);

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(background.transform, false);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            cooldownFill = fill.GetComponent<Image>();
            cooldownFill.raycastTarget = false;
            cooldownFill.color = ReadyColor;
        }

        private static void SetCorner(GameObject target, Vector2 position, Vector2 pivot)
        {
            RectTransform rect = target.GetComponent<RectTransform>();
            rect.pivot = pivot;
            rect.anchoredPosition = position;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    

private void BindButtons()
        {
            BindButton("TitlePanel/StartGameButton", StartGame);
            BindButton("PreparationPanel/PreparationActions/KingButton", SelectKing);
            BindButton("PreparationPanel/PreparationActions/RookButton", SelectRook);
            BindButton("PreparationPanel/PreparationActions/KnightButton", SelectKnight);
            BindButton("PreparationPanel/PreparationActions/PawnButton", SelectPawn);
            BindButton("PreparationPanel/PreparationActions/QuickSetupButton", SetupPlayer);
            BindButton("PreparationPanel/PreparationActions/StartBattleButton", StartBattle);
            BindButton("ResultPanel/ResultButtons/RestartButton", RestartSession);
            BindButton("ResultPanel/ResultButtons/ReturnButton", ReturnToTitle);
        }

        private void BindButton(string relativePath, UnityEngine.Events.UnityAction action)
        {
            Transform target = transform.Find(relativePath);
            if (target == null)
            {
                return;
            }

            Button button = target.GetComponent<Button>();
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }


private string GetPreparationSelectionText()
        {
            ManualPlacementController controller = FindFirstObjectByType<ManualPlacementController>();
            if (controller == null) return "없음";
            if (controller.SelectedPieceDefinition != null) return LocalizePieceName(controller.SelectedPieceDefinition);
            if (controller.SelectedPlacedPiece != null && controller.SelectedPlacedPiece.Definition != null)
            {
                return LocalizePieceName(controller.SelectedPlacedPiece.Definition) + " (위치 변경)";
            }
            return "없음";
        }

        private void UpdatePreparationButtonColors()
        {
            ManualPlacementController controller = FindFirstObjectByType<ManualPlacementController>();
            PieceType? selectedType = controller != null && controller.SelectedPieceDefinition != null
                ? controller.SelectedPieceDefinition.PieceType
                : (PieceType?)null;
            SetButtonSelected(kingButton, selectedType == PieceType.King);
            SetButtonSelected(rookButton, selectedType == PieceType.Rook);
            SetButtonSelected(knightButton, selectedType == PieceType.Knight);
            SetButtonSelected(pawnButton, selectedType == PieceType.Pawn);
        }

        private static void SetButtonSelected(Button button, bool selected)
        {
            if (button != null && button.targetGraphic != null)
            {
                button.targetGraphic.color = selected ? SelectedButtonColor : ButtonColor;
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
