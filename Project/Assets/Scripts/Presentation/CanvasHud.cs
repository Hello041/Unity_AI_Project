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
        private GameObject resultButtons;

        private static readonly Color PanelColor = new Color(0.055f, 0.075f, 0.11f, 0.92f);
        private static readonly Color AccentColor = new Color(0.22f, 0.62f, 0.92f, 1f);
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
            titlePanel = CreatePanel("TitlePanel", transform, new Vector2(0.5f, 0.5f), new Vector2(520f, 270f));
            CreateText("Title", titlePanel.transform, "TACTICAL ROGUELIKE MVP", 34, TextAnchor.MiddleCenter,
                new Vector2(24f, -28f), new Vector2(-24f, -86f));
            CreateText("Subtitle", titlePanel.transform, "6x6 Real-Time Chess Battle", 20, TextAnchor.MiddleCenter,
                new Vector2(24f, -92f), new Vector2(-24f, -132f));
            CreateButton("StartGameButton", titlePanel.transform, "Start Game", new Vector2(0.5f, 0f),
                new Vector2(260f, 52f), new Vector2(0f, 42f), StartGame);
        }

private void BuildPreparationPanel()
        {
            preparationPanel = CreateStretchPanel("PreparationPanel", transform);

            GameObject infoPanel = CreatePanel("PreparationInfoPanel", preparationPanel.transform,
                new Vector2(0f, 1f), new Vector2(320f, 390f));
            SetCorner(infoPanel, new Vector2(18f, -18f), new Vector2(0f, 1f));

            CreateText("Header", infoPanel.transform, "PREPARATION", 25, TextAnchor.MiddleLeft,
                new Vector2(16f, -12f), new Vector2(-16f, -50f));
            preparationText = CreateText("PreparationStatus", infoPanel.transform, string.Empty, 16,
                TextAnchor.UpperLeft, new Vector2(16f, -58f), new Vector2(-16f, -374f));

            GameObject actions = CreateStretchPanel("PreparationActions", preparationPanel.transform);

            CreateButton("KingButton", actions.transform, "King", new Vector2(0.5f, 0f),
                new Vector2(120f, 40f), new Vector2(-195f, 118f), SelectKing);
            CreateButton("RookButton", actions.transform, "Rook", new Vector2(0.5f, 0f),
                new Vector2(120f, 40f), new Vector2(-65f, 118f), SelectRook);
            CreateButton("KnightButton", actions.transform, "Knight", new Vector2(0.5f, 0f),
                new Vector2(120f, 40f), new Vector2(65f, 118f), SelectKnight);
            CreateButton("PawnButton", actions.transform, "Pawn", new Vector2(0.5f, 0f),
                new Vector2(120f, 40f), new Vector2(195f, 118f), SelectPawn);

            CreateButton("QuickSetupButton", actions.transform, "Setup Player MVP", new Vector2(0.5f, 0f),
                new Vector2(250f, 48f), new Vector2(-130f, 58f), SetupPlayer);
            CreateButton("StartBattleButton", actions.transform, "Start Battle", new Vector2(0.5f, 0f),
                new Vector2(250f, 48f), new Vector2(130f, 58f), StartBattle);
        }

        private void BuildBattlePanel()
        {
            battlePanel = CreateStretchPanel("BattlePanel", transform);

            GameObject topLeft = CreatePanel("StageEnemyPanel", battlePanel.transform, new Vector2(0f, 1f), new Vector2(300f, 270f));
            SetCorner(topLeft, new Vector2(18f, -18f), new Vector2(0f, 1f));
            stageText = CreateText("StageInfo", topLeft.transform, string.Empty, 22, TextAnchor.UpperLeft,
                new Vector2(16f, -14f), new Vector2(-16f, -82f));
            enemyText = CreateText("EnemyInfo", topLeft.transform, string.Empty, 18, TextAnchor.UpperLeft,
                new Vector2(16f, -88f), new Vector2(-16f, -14f));

            GameObject cooldownPanel = CreatePanel("CooldownPanel", battlePanel.transform, new Vector2(1f, 1f), new Vector2(300f, 120f));
            SetCorner(cooldownPanel, new Vector2(-18f, -18f), new Vector2(1f, 1f));
            cooldownText = CreateText("CooldownText", cooldownPanel.transform, string.Empty, 20, TextAnchor.MiddleLeft,
                new Vector2(16f, -12f), new Vector2(-16f, -52f));
            CreateCooldownBar(cooldownPanel.transform);

            GameObject bottomLeft = CreatePanel("BattleStatusPanel", battlePanel.transform, new Vector2(0f, 0f), new Vector2(310f, 150f));
            SetCorner(bottomLeft, new Vector2(18f, 18f), new Vector2(0f, 0f));
            battleStatusText = CreateText("BattleStatus", bottomLeft.transform, string.Empty, 19, TextAnchor.UpperLeft,
                new Vector2(16f, -14f), new Vector2(-16f, -14f));

            GameObject bottomRight = CreatePanel("SelectedPiecePanel", battlePanel.transform, new Vector2(1f, 0f), new Vector2(320f, 150f));
            SetCorner(bottomRight, new Vector2(-18f, 18f), new Vector2(1f, 0f));
            selectedPieceText = CreateText("SelectedPiece", bottomRight.transform, string.Empty, 19, TextAnchor.UpperLeft,
                new Vector2(16f, -14f), new Vector2(-16f, -14f));

            firstMovePanel = CreatePanel("FirstMovePanel", battlePanel.transform, new Vector2(0.5f, 1f), new Vector2(500f, 86f));
            SetCorner(firstMovePanel, new Vector2(0f, -18f), new Vector2(0.5f, 1f));
            CreateText("FirstMoveText", firstMovePanel.transform, "BATTLE START\nMake your first move to activate Enemy AI.", 21,
                TextAnchor.MiddleCenter, new Vector2(14f, -10f), new Vector2(-14f, -10f));
        }

        private void BuildResultPanel()
        {
            resultPanel = CreatePanel("ResultPanel", transform, new Vector2(0.5f, 0.5f), new Vector2(520f, 320f));
            resultTitleText = CreateText("ResultTitle", resultPanel.transform, string.Empty, 36, TextAnchor.MiddleCenter,
                new Vector2(24f, -28f), new Vector2(-24f, -86f));
            resultBodyText = CreateText("ResultBody", resultPanel.transform, string.Empty, 20, TextAnchor.MiddleCenter,
                new Vector2(24f, -92f), new Vector2(-24f, -160f));
            resultButtons = new GameObject("ResultButtons", typeof(RectTransform));
            resultButtons.transform.SetParent(resultPanel.transform, false);
            Stretch(resultButtons.GetComponent<RectTransform>());
            CreateButton("RestartButton", resultButtons.transform, "Restart Session", new Vector2(0.5f, 0f),
                new Vector2(330f, 46f), new Vector2(0f, 78f), RestartSession);
            CreateButton("ReturnButton", resultButtons.transform, "Return To Title", new Vector2(0.5f, 0f),
                new Vector2(330f, 46f), new Vector2(0f, 24f), ReturnToTitle);
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
            builder.AppendLine("Current Stage: " + gameManager.CurrentStageLabel);

            if (preparationManager != null)
            {
                builder.AppendLine("Loadout Cost: " + preparationManager.CurrentCost + " / " + preparationManager.MaxLoadoutCost);
                builder.AppendLine("Placed Pieces: " + preparationManager.PlacedCount + " / " + preparationManager.SelectedCount);
            }

            if (gameManager.PlayerHealth != null)
            {
                builder.AppendLine("Player HP: " + gameManager.PlayerHealth.CurrentHp + " / " + gameManager.PlayerHealth.MaxHp);
            }

            if (enemySetupManager != null)
            {
                EnemySetupDefinition setup = enemySetupManager.ActiveSetup;
                builder.AppendLine("Enemy Setup: " + (setup != null ? setup.PatternName : "None"));
                builder.AppendLine("Enemy Count: " + enemySetupManager.SpawnedEnemyCount);

                if (setup != null && setup.SpawnEntries != null)
                {
                    builder.AppendLine("Enemy Team:");
                    EnemySpawnEntry[] entries = setup.SpawnEntries;
                    for (int i = 0; i < entries.Length; i++)
                    {
                        PieceDefinition piece = entries[i] != null ? entries[i].PieceDefinition : null;
                        if (piece != null)
                        {
                            builder.AppendLine("- " + piece.DisplayName);
                        }
                    }
                }
            }

            builder.AppendLine();
            builder.Append("Feedback: " + (prototypeHud != null ? prototypeHud.LastMessage : "Ready."));
            preparationText.text = builder.ToString();
        }

        private void RefreshBattle()
        {
            stageText.text = gameManager.CurrentStageLabel + "\nState: " + gameManager.CurrentState;
            enemyText.text = BuildEnemyTeamText();

            bool waiting = enemyAIController != null && enemyAIController.IsWaitingForPlayerFirstMove;
            firstMovePanel.SetActive(waiting);
            int hp = gameManager.PlayerHealth != null ? gameManager.PlayerHealth.CurrentHp : 0;
            int maxHp = gameManager.PlayerHealth != null ? gameManager.PlayerHealth.MaxHp : 0;
            battleStatusText.text = "BATTLE STATUS\nHP: " + hp + " / " + maxHp + "\nEnemy AI: "
                + (waiting ? "Waiting for first move" : "Active");

            PieceController selected = boardInputController != null ? boardInputController.SelectedPiece : null;
            if (selected == null || selected.Definition == null)
            {
                selectedPieceText.text = "SELECTED PIECE\nNone";
            }
            else
            {
                string cooldown = selected.Cooldown == null || selected.Cooldown.IsReady
                    ? "Ready"
                    : selected.Cooldown.RemainingTime.ToString("0.00") + "s";
                selectedPieceText.text = "SELECTED PIECE\nName: " + selected.Definition.DisplayName
                    + "\nType: " + selected.Definition.PieceType + "\nCooldown: " + cooldown;
            }

            float duration = playerGlobalCooldown != null ? playerGlobalCooldown.CooldownDuration : 0f;
            float remaining = playerGlobalCooldown != null ? playerGlobalCooldown.RemainingTime : 0f;
            float progress = duration > 0f ? 1f - Mathf.Clamp01(remaining / duration) : 1f;
            cooldownText.text = "GLOBAL COOLDOWN: " + (remaining > 0f ? remaining.ToString("0.00") + "s" : "Ready");
            cooldownFill.fillAmount = progress;
            cooldownFill.color = remaining > 0f ? AccentColor : ReadyColor;
        }

        private string BuildEnemyTeamText()
        {
            if (enemySetupManager == null || enemySetupManager.ActiveSetup == null) return "ENEMY TEAM\nNo active setup";

            StringBuilder builder = new StringBuilder("ENEMY TEAM\n");
            builder.AppendLine(enemySetupManager.ActiveSetup.PatternName);
            EnemySpawnEntry[] entries = enemySetupManager.ActiveSetup.SpawnEntries;
            if (entries != null)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    PieceDefinition piece = entries[i] != null ? entries[i].PieceDefinition : null;
                    if (piece != null) builder.AppendLine("- " + piece.DisplayName);
                }
            }
            return builder.ToString();
        }

        private void RefreshResult(GameState state)
        {
            bool stageClear = state == GameState.StageClear;
            bool victory = state == GameState.Victory;
            resultTitleText.text = stageClear ? gameManager.CurrentStageLabel.ToUpperInvariant() + " CLEAR" : victory ? "VICTORY" : "GAME OVER";
            resultBodyText.text = stageClear ? "Advancing to the next stage..." : victory ? "All Stages Cleared" : "The session has ended.";
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

        private void CreateButton(string objectName, Transform parent, string label, Vector2 anchor, Vector2 size,
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
            image.color = AccentColor;
            image.raycastTarget = true;
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            Text text = CreateText("Label", buttonObject.transform, label, 20, TextAnchor.MiddleCenter,
                new Vector2(8f, -4f), new Vector2(-8f, -4f));
            Stretch(text.rectTransform);
        }

        private void CreateCooldownBar(Transform parent)
        {
            GameObject background = new GameObject("CooldownBar", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(parent, false);
            RectTransform rect = background.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(16f, 18f);
            rect.offsetMax = new Vector2(-16f, 40f);
            
            background.GetComponent<Image>().raycastTarget = false;
background.GetComponent<Image>().color = new Color(0.12f, 0.15f, 0.2f, 1f);

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(background.transform, false);
            Stretch(fill.GetComponent<RectTransform>());
            cooldownFill = fill.GetComponent<Image>();
            cooldownFill.type = Image.Type.Filled;
            cooldownFill.fillMethod = Image.FillMethod.Horizontal;
            cooldownFill.fillOrigin = 0;
            
            cooldownFill.raycastTarget = false;
cooldownFill.fillAmount = 1f;
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
}
}
