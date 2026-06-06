# Prompt05 - Player Input and Playable Interaction

You are a senior Unity and C# game developer.

This project already completed Prompt01, Prompt02, Prompt03, and Prompt04.

Use the existing implementation as the source of truth.

Do not redesign previously implemented systems.

---

## Existing Systems (Do Not Rewrite)

Unless a minimal integration change is strictly required, do not rewrite, replace, or refactor:

### Core

* GameManager
* GameState
* PlayerHealthService
* StageDefinition

### Board

* BoardManager
* GridPosition
* BoardView
* BoardTileView
* BoardHighlightType

### Pieces

* PieceDefinition
* PieceController
* PieceView
* PieceType
* PieceOwner

### Movement

* PieceMovementService
* IMovementBehaviour
* MovementBehaviourResolver
* MovementRuleUtility
* KingMovement
* RookMovement
* KnightMovement
* PawnMovement

### Interaction

* PieceActionController
* BoardInputController

### Cooldown

* PieceCooldown
* PlayerGlobalCooldown

### Preparation

* PreparationManager
* PlacementValidator
* ManualPlacementController

### Enemy Setup

* EnemySetupManager
* EnemySetupDefinition
* EnemySpawnEntry

### Enemy AI

* EnemyAIController
* EnemyMoveSelector
* PieceValueTable
* EnemyActionDelay

Build on top of the existing implementation.

---

## Goal

Implement minimal player interaction so the game becomes fully playable.

The player must be able to:

```txt
Select a piece
→ View valid movement positions
→ Click a destination tile
→ Move or capture
```

The implementation must remain compile-safe and editor-testable.

---

## Scope

Implement:

```txt
Player Piece Selection

Board Tile Clicking

Move Execution

Capture Execution

Selection Feedback

Movement Highlight Feedback
```

Only implement what is required for playable interaction.

---

## Player Selection Rules

Player may select only:

```txt
PieceOwner.Player
```

Enemy pieces cannot be selected.

When a player piece is selected:

```txt
Store selected piece
Show selection feedback
Display valid movement positions
```

Selecting another player piece:

```txt
Replace the current selection
Refresh highlights
```

---

## Tile Click Rules

When a board tile is clicked:

### Valid Destination

If the tile is a valid move:

```txt
Execute movement
Execute capture if applicable
Start existing cooldown flow
Clear selection
Refresh highlights
```

### Invalid Destination

If the tile is not a valid move:

```txt
Do not move
Do not start cooldown
Keep current selection
```

---

## Input Restrictions

Player input is allowed only when:

```txt
GameState == Playing
```

Input must be ignored when:

```txt
Boot
StageStart
Preparation
StageClear
GameOver
Result
```

---

## Cooldown Integration

Player interaction must respect:

```txt
PieceCooldown

PlayerGlobalCooldown
```

A piece cannot move when:

```txt
PieceCooldown is active
```

A player piece cannot move when:

```txt
PlayerGlobalCooldown is active
```

Reuse existing cooldown systems.

Do not create new cooldown logic.

---

## Highlight Feedback

Implement minimal editor-testable feedback.

Supported highlights:

```txt
Selected Piece

Valid Movement Tile

Capture Tile
```

Reuse existing:

```txt
BoardView
BoardTileView
BoardHighlightType
```

whenever possible.

Do not create advanced VFX.

---

## Capture Integration

Player captures must continue to use:

```txt
PieceActionController
```

and existing capture flow.

Do not duplicate capture logic.

Enemy King capture must still trigger:

```txt
GameManager.NotifyEnemyKingCaptured()
```

through existing systems.

---

## Editor Testability

The system must be testable entirely in Play Mode.

Expected test flow:

```txt
Enter Play Mode
→ Setup Player MVP
→ Spawn Random Enemies
→ Start Battle
→ Select Piece
→ Click Valid Tile
→ Move
→ Capture Enemy Piece
→ Capture Enemy King
→ StageClear
```

---

## Explicitly Out of Scope

Do NOT implement:

```txt
Drag and Drop

Touch Controls

Advanced Input Architecture

Input Action Rework

Animation Systems

Audio Systems

VFX

Cooldown Bars

Shop

Reward Systems

Campaign

Save / Load

Check

Checkmate

Minimax

Advanced AI
```

---

## Acceptance Criteria

1. Player pieces can be selected.
2. Enemy pieces cannot be selected.
3. Valid movement positions are displayed.
4. Invalid destinations do not execute movement.
5. Valid destinations execute movement.
6. Captures execute correctly.
7. Existing cooldown systems are respected.
8. Input only works during Playing.
9. StageClear and GameOver disable input.
10. No Console Errors.
11. No Console Warnings.

---

## Output Format

Please answer in this order:

1. Architecture risks and integration notes
2. File-by-file implementation plan
3. C# code
4. Unity Editor setup instructions
5. Manual test procedure
6. Known limitations
