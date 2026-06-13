# Prompt07 - Stage Structure Vertical Slice

## Goal

Replace the manual enemy spawning workflow with a simple stage progression structure.

The game should automatically generate the enemy encounter for the current stage before Preparation begins.

The player should see the enemy composition and board layout before starting the battle.

Do not implement Reward, Shop, Campaign, Save/Load, or Meta Progression systems.

---

## Scope

Implement:

* Stage 1
* Stage 2
* Stage 3
* Victory state

Stage flow:

```txt
Title
→ Stage 1
→ Preparation
→ Battle
→ StageClear

→ Stage 2
→ Preparation
→ Battle
→ StageClear

→ Stage 3
→ Preparation
→ Battle
→ Victory

→ Return To Title
or
Restart Session
```

---

## Stage Rules

Stage 1

```txt
PatternA_KingRookPawn
```

Stage 2

```txt
PatternB_KingKnightPawnPawn
```

Stage 3

```txt
PatternC_KingRookKnight
```

Do not randomize stage order.

The stage sequence must always be:

```txt
Stage1
→ Stage2
→ Stage3
```

---

## Enemy Spawn Behavior

Current workflow:

```txt
Preparation
→ Spawn Random Enemies
→ Start Battle
```

Replace with:

```txt
Enter Stage
→ Automatically spawn stage enemy setup
→ Enter Preparation
→ Start Battle
```

Requirements:

* Remove dependency on Spawn Random Enemies button.
* Enemy setup should exist before Preparation begins.
* Enemy composition should already be visible during Preparation.
* Enemy board positions should already be visible during Preparation.
* StageClear should advance to the next stage automatically.

---

## UI Requirements

Preparation HUD should display:

```txt
Current Stage

Example:

Stage 1
Stage 2
Stage 3
```

Enemy Team panel remains.

Spawn Random Enemies button should be removed or disabled.

Current stage information should be visible during:

```txt
Preparation
Playing
StageClear
```

A simple IMGUI label is sufficient.

Do not redesign the UI system.

---

## StageClear Behavior

Current:

```txt
Enemy King Captured
→ StageClear
```

New:

```txt
Enemy King Captured

If Stage < 3:
    Advance Stage
    Generate next encounter
    Return to Preparation

If Stage == 3:
    Victory
```

---

## Victory Behavior

Implement a simple final Victory state.

Example:

```txt
Victory

All Stages Cleared

[Return To Title]
[Restart Session]
```

No campaign progression.

No reward selection.

No unlock system.

---

## Architecture Requirements

Reuse existing systems whenever possible.

Prefer modifying:

```txt
GameManager
EnemySetupManager
PrototypeHud
StageDefinition
```

Do not redesign completed Prompt01~Prompt06 systems.

Keep integration minimal.

---

## Validation

Verify:

* Stage 1 loads PatternA automatically
* Stage 2 loads PatternB automatically
* Stage 3 loads PatternC automatically
* Enemy setup exists before Preparation begins
* Enemy composition visible during Preparation
* StageClear advances stage
* Victory occurs after Stage 3 clear
* Restart Session resets to Stage 1
* Return To Title works correctly
* Console Errors = 0
* Console Warnings = 0

---

## Out Of Scope

Do NOT implement:

```txt
Reward
Shop
Campaign
Meta Progression
Save / Load
Economy
Stage Map
Per-piece cooldown bars
UI redesign
Animation
Audio
VFX
```

Only implement the Stage Structure vertical slice.
