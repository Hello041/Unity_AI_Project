# Prompt09 - Canvas UI Conversion

Use all attached documents as the source of truth.

Prompt01 through Prompt08 are complete and accepted.

Implement only Prompt09 scope.

Do not redesign gameplay systems.

## Goal

Replace or wrap the current lightweight IMGUI-based PrototypeHud with a more readable Unity Canvas-based UI.

The purpose is readability and usability, not visual polish.

## Scope

Implement a simple Canvas UI for:

1. Title screen
2. Preparation HUD
3. Battle HUD
4. StageClear / GameOver / Victory result UI
5. Buttons for Start Game, Start Battle, Restart, Return To Title
6. Status panels

## UI Layout

Recommended layout:

### Top-left

```txt
Stage Info
Enemy Team
```

### Top-center

```txt
First Move Notice
StageClear / GameOver / Victory message
```

### Top-right

```txt
Global Cooldown Bar
```

### Bottom-left

```txt
Battle Status
HP
Enemy AI Status
```

### Bottom-right

```txt
Selected Piece Info
Piece Type
Cooldown State
```

### Center / Bottom

```txt
Main action buttons
Start Game
Start Battle
Restart
Return To Title
```

## Requirements

* Keep existing gameplay logic unchanged.
* UI should read from existing systems.
* Do not duplicate GameManager logic.
* Do not duplicate EnemyAI logic.
* Do not duplicate cooldown logic.
* Maintain existing title, preparation, playing, result, and victory flow.
* Existing IMGUI PrototypeHud can remain as fallback if needed, but duplicated buttons should be avoided.
* UI must work in Play Mode without manual scene setup beyond required Canvas references.

## Out of Scope

Do NOT implement:

* Full visual design polish
* Custom art
* Animation
* Sound
* VFX
* Reward UI
* Shop UI
* Save / Load UI
* Mobile/touch optimization
* AR UI
* Advanced input architecture

## Acceptance Criteria

1. Title UI appears correctly.
2. Preparation UI is readable.
3. Start Battle button is visible and usable.
4. Stage info is visible.
5. Enemy team info is visible.
6. Global cooldown is visible.
7. Selected piece info is visible.
8. Result UI appears for StageClear, GameOver, and Victory.
9. Existing game flow remains functional.
10. No Console Errors.
11. No Console Warnings.

## Output Format

Respond in Korean.

Before modifying code:

1. Explain UI architecture choice.
2. Identify affected files and scene objects.
3. Explain how existing systems will be reused.

After implementation:

1. Provide modified files.
2. Provide editor setup instructions.
3. Provide validation results.
