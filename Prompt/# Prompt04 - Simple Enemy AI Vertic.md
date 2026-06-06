# Prompt04 - Simple Enemy AI Vertical Slice

You are a senior Unity and C# game developer.

This project already completed Prompt01, Prompt02, and Prompt03.

Use the existing implementation as the source of truth.

Do not redesign previously implemented systems.

---

## Existing Systems (Do Not Rewrite)

Unless a minimal integration change is strictly required, do not rewrite, replace, or refactor:

* GameManager
* GameState
* StageDefinition
* PlayerHealthService
* HealthEventData
* StageEventData
* BoardManager
* GridPosition
* PieceDefinition
* PieceController
* PieceMovementService
* PieceCooldown
* PlayerGlobalCooldown
* PreparationManager
* PlacementValidator
* EnemySetupManager

Build on top of the existing implementation.

---

## Goal

Implement a lightweight reactive enemy AI.

The goal is to make the MVP feel like an actual playable game while keeping implementation simple and compile-safe.

Do not implement a chess engine.

Do not implement advanced decision-making systems.

---

## Scope

Implement:

```txt
EnemyAIController

EnemyMoveSelector

PieceValueTable

EnemyActionDelay
```

Only implement what is required for basic enemy behavior.

---

## AI Rules

Enemy AI must follow this priority order:

### Priority 1

Capture Player King.

If a valid move can capture the Player King:

```txt
Capture immediately.
```

---

### Priority 2

Capture the highest-value player piece.

Suggested values:

```txt
King = 100
Rook = 5
Knight = 3
Pawn = 1
```

If multiple captures are possible:

```txt
Choose the highest-value target.
```

---

### Priority 3

Perform a random valid movement.

Requirements:

* Movement must be legal.
* Movement must respect occupancy.
* Movement must respect movement rules.
* Movement must respect cooldown rules.

---

### Priority 4

If no valid movement exists:

```txt
Do nothing.
```

---

## Cooldown Rules

Enemy pieces must:

* Respect individual cooldowns
* Ignore player global cooldown
* Never move while their own cooldown is active

---

## Integration Rules

Use existing systems whenever possible.

Reuse:

```txt
BoardManager
PieceMovementService
PieceController
PieceCooldown
```

Do not duplicate movement logic.

Do not create separate AI movement rules.

---

## Battle Flow

Enemy AI should only run when:

```txt
GameState == Playing
```

Enemy AI should stop immediately when:

```txt
StageClear
```

or

```txt
GameOver
```

occurs.

---

## Editor Testability

Provide simple debug support.

Examples:

```txt
Debug.Log for selected actions
Debug.Log for chosen target
Debug.Log for random movement
```

No advanced UI required.

---

## Explicitly Out of Scope

Do NOT implement:

```txt
Minimax

Chess Engine

Check

Checkmate

Turn Prediction

Threat Maps

Advanced Tactical Analysis

Campaign

Shop

Reward Systems

Save / Load

Animation Systems

Audio Systems
```

---

## Acceptance Criteria

1. Enemy pieces can move automatically.
2. Enemy pieces obey movement rules.
3. Enemy pieces obey cooldown rules.
4. Enemy pieces prioritize Player King captures.
5. Enemy pieces prioritize high-value captures.
6. Enemy pieces can perform random legal movement.
7. Enemy AI only runs during Playing.
8. StageClear and GameOver stop AI behavior.
9. No Console Errors.
10. No Console Warnings.

---

## Output Format

Please answer in this order:

1. Architecture risks and integration notes
2. File-by-file implementation plan
3. C# code
4. Unity Editor setup instructions
5. Manual test procedure
6. Known limitations
