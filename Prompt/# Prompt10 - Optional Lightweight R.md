# Prompt10 - Optional Lightweight Reward Extension

Use all attached documents as the source of truth.

Prompt01 through Prompt09 are complete and accepted.

Implement this only if the current project is stable and there is enough time.

Do not implement Shop or full economy.

## Goal

Add a very small reward choice after StageClear to make the stage progression feel more roguelike.

## Scope

Implement only a simple reward selection between stages.

Stage flow:

```txt
Stage Clear
→ Reward Choice
→ Next Stage Preparation
```

## Reward Options

Implement 2~3 simple rewards.

Examples:

```txt
+1 Max HP
+1 Loadout Cost for future stages
Reduce global cooldown slightly
```

Keep rewards simple and data-light.

## Requirements

* Reward appears after Stage 1 and Stage 2 clear.
* No reward after Stage 3; Stage 3 should lead to Victory.
* Reward choice should apply immediately.
* Reward should persist only during the current session.
* Restart Session resets rewards.
* Return To Title resets rewards.
* UI can be simple.

## Out of Scope

Do NOT implement:

* Shop
* Gold
* Currency
* Inventory
* Permanent upgrades
* Save / Load
* Campaign map
* Complex reward rarity
* Random reward pools unless trivial
* Animation
* Audio
* VFX

## Acceptance Criteria

1. StageClear opens reward choice after Stage 1.
2. StageClear opens reward choice after Stage 2.
3. Selected reward applies correctly.
4. Next stage starts after reward selection.
5. Stage 3 clear leads to Victory.
6. Restart resets rewards.
7. No Console Errors.
8. No Console Warnings.

## Output Format

Respond in Korean.

Before implementation:

1. Explain whether this feature is safe to add.
2. Identify affected systems.
3. Explain minimal implementation plan.

After implementation:

1. Provide modified files.
2. Provide validation results.
3. Provide known limitations.
