# Prompt 02 - First 6x6 Tactical Roguelike Vertical Slice C# Implementation

You are a senior Unity and C# game developer.

Based on the 6x6 Tactical Roguelike MVP architecture from the previous prompt, implement only the first gameplay-flow vertical slice. Do not build the whole game yet.

The goal of this step is to create a small, compile-safe Unity C# foundation for one-stage lifecycle, preparation state, player health, stage clear, and game over.

==================================================
Highest Priority Rule
==================================================

Compile-safe code is the highest priority.

If any requirement is ambiguous, prefer a smaller working implementation over a larger incomplete architecture.

==================================================
Core Identity Rule
==================================================

The project should preserve:

```txt
real-time chess gameplay
+
cooldown management
+
lightweight roguelike strategy
```

This first slice does not implement board movement yet, but its state flow must be ready to connect to loadout, placement, movement, capture, and cooldown systems later.

==================================================
AI Implementation Guardrails
==================================================

- Prefer simpler architecture.
- Prefer explicit responsibilities.
- Avoid speculative future systems.
- Implement only this requested vertical slice.
- Avoid scope creep.
- Keep each class reasonably small. Prefer approximately 80 to 200 lines per class when practical.
- Do not introduce unnecessary generic frameworks, service locators, factories, or enterprise patterns.
- Do not use external Dependency Injection frameworks such as Zenject or VContainer.
- Architecture should remain explainable within a 5 to 10 minute university final presentation.

==================================================
Project Context
==================================================

This is a Unity game prototype for a university final project.

The final MVP is a 6x6 tactical roguelike-lite game:

- One playable stage
- Configurable board, default 6x6
- Pre-stage loadout budget later
- Pre-stage placement later
- Real-time piece cooldowns later
- Enemy king capture means StageClear
- Player HP depletion means GameOver
- No campaign progression
- No shop loop
- No full reward economy
- No full augment architecture

This first slice does not need to implement board movement, actual chess pieces, placement UI, cooldown UI, shop, augments, multiple stages, or full roguelike systems.

The final visual direction is a 3D board with 2D sprite or billboard-style pieces and a fixed top-down or slightly angled isometric camera. This first implementation should remain presentation-independent because it focuses only on state flow and health.

==================================================
Implementation Scope
==================================================

Write Unity C# code for the following files:

1. GameState.cs
2. GameManager.cs
3. PlayerHealthService.cs
4. StageDefinition.cs
5. StageEventData.cs
6. HealthEventData.cs

Optional if useful:

7. ResultData.cs
8. ResultCalculator.cs

The code should be directly usable in a Unity project. Each file should include all required using statements.

Use modern C# syntax only when it is stable in Unity and does not conflict with Unity serialization or Inspector workflows.

Recommended folder placement:

```txt
Scripts/
 ├── Core/GameState.cs
 ├── Gameplay/Stage/GameManager.cs
 ├── Gameplay/Health/PlayerHealthService.cs
 ├── Gameplay/Results/ResultData.cs
 ├── Gameplay/Results/ResultCalculator.cs
 └── Data/StageDefinition.cs
```

==================================================
Detailed Requirements
==================================================

1. GameState

- Create an enum with:
  - Boot
  - StageStart
  - Preparation
  - Playing
  - StageClear
  - GameOver
  - Result
- Result is optional but allowed for a lightweight post-stage summary.
- Do not include Reward, Shop, RoundClear, or campaign states.

2. StageDefinition

- Create a ScriptableObject named StageDefinition.
- Use it only for immutable configuration data.
- Do not store mutable runtime gameplay state inside it.
- Include:
  - string stageName
  - int boardWidth = 6
  - int boardHeight = 6
  - int playerMaxHealth = 3
  - int maxLoadoutCost = 7
  - bool showResultAfterClear
- Use private fields with SerializeField.
- Expose read-only public properties.
- Add CreateAssetMenu.
- Clamp or validate board size and health values defensively.

3. PlayerHealthService

- Create a MonoBehaviour.
- At stage start, health should reset based on StageDefinition.playerMaxHealth.
- Default health should be 3 if no valid value is provided.
- Provide:

```csharp
void ResetForStage(int maxHealth)
void TakeDamage(int amount = 1)
```

- Health cannot go below 0.
- OnHealthDepleted should fire once when health reaches 0.
- Expose:

```csharp
public event Action<HealthEventData> OnHealthChanged;
public event Action<HealthEventData> OnHealthDepleted;
```

4. GameManager

- Create a MonoBehaviour.
- Use SerializeField references:
  - PlayerHealthService playerHealthService
  - StageDefinition stageDefinition
- Manage the current GameState.
- Subscribe to PlayerHealthService.OnHealthDepleted.
- Unsubscribe properly in OnDisable or OnDestroy.
- Use a dedicated state transition method, such as SetState(GameState nextState), so all state changes are logged and centralized.
- Implement explicit state preconditions.
- GameOver is terminal. Once GameOver is entered, ignore stage clear and gameplay notifications.
- Provide methods:

```csharp
void StartStage()
void StartBattle()
void NotifyEnemyKingCaptured()
void NotifyPlayerKingCaptured()
```

- StartStage should enter StageStart, reset health, then enter Preparation.
- StartBattle should enter Playing only after preparation is considered valid.
- In this first slice, preparation validity may be a simple bool or placeholder method.
- NotifyPlayerKingCaptured should only work in Playing and call playerHealthService.TakeDamage().
- NotifyEnemyKingCaptured should only work in Playing and trigger StageClear.

Reference state transition table:

```txt
Boot
-> StageStart
-> Preparation

Preparation + StartBattle
-> Playing

Playing + EnemyKingCaptured
-> StageClear
-> Result (optional)

Playing + HealthDepleted
-> GameOver
```

- No next stage transition.
- No RoundClear transition.
- No Reward or Shop transition.
- Add Debug.Log messages for state transitions so the behavior can be tested without UI.

==================================================
Code Style Requirements
==================================================

- Target Unity 6.3 LTS, version 6000.3.11f1.
- Use Unity C#.
- Separate MonoBehaviour classes and pure C# classes appropriately.
- Use SerializeField for inspector-visible values.
- Do not use public fields.
- Use C# Action events.
- Include null checks and basic defensive code.
- Avoid excessive abstraction.
- Avoid Update unless absolutely necessary.
- Do not add rendering-specific code in this first slice.
- Do not implement VFX, animation, sound, detailed UI, board movement, loadout UI, placement UI, shop, reward economy, or augment framework.
- Use Inspector references and simple constructor injection only for pure C# helper classes when appropriate.
- Include short comments only where they help explain architectural choices for a final presentation.

==================================================
Acceptance Criteria
==================================================

The code should support these manual tests inside Unity:

1. When Play starts, one stage starts automatically.
2. StartStage resets HP to StageDefinition.playerMaxHealth and enters Preparation.
3. StartBattle moves from Preparation to Playing.
4. During Playing, calling NotifyEnemyKingCaptured causes StageClear.
5. During Playing, calling NotifyPlayerKingCaptured until HP reaches 0 causes GameOver.
6. Calling NotifyEnemyKingCaptured or NotifyPlayerKingCaptured outside Playing does not change the game state.
7. After GameOver, gameplay notifications are ignored.
8. StageDefinition is used as immutable configuration data only.
9. No ShopManager, RewardManager, RoundManager, EnemyAI, or AugmentSystem is created.

==================================================
Output Format
==================================================

Please answer in this order:

1. Short explanation of what this implementation covers
2. File-by-file C# code
3. Unity Editor setup guide
4. Manual test procedure
5. Notes for how this code will connect later to board size, loadout, placement, pieces, capture, cooldowns, random enemy setup, and presentation feedback
