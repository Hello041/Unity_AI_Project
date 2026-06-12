# Prompt06 - Playable Build Polish

You are a senior Unity and C# game developer.

This project already completed Prompt01, Prompt02, Prompt03, Prompt04, and Prompt05.

Use all existing documents as the source of truth.

Do not redesign completed systems.

---

## Goal

Transform the current prototype into a complete playable build suitable for project submission.

The player should be able to:

Title
→ Start Game
→ Preparation
→ Battle
→ StageClear / GameOver
→ Restart
→ Play Again

The implementation must remain compile-safe and editor-testable.

---

## Existing Systems (Do Not Rewrite)

Do not rewrite, replace, or refactor unless strictly required:

### Core

* GameManager
* GameState
* StageDefinition
* PlayerHealthService

### Board

* BoardManager
* GridPosition
* BoardView
* BoardTileView

### Pieces

* PieceController
* PieceDefinition
* PieceView

### Movement

* PieceMovementService
* Movement Behaviours

### Combat

* PieceActionController

### Cooldown

* PieceCooldown
* PlayerGlobalCooldown

### Enemy AI

* EnemyAIController
* EnemyMoveSelector
* EnemyActionDelay

### Preparation

* PreparationManager
* PlacementValidator
* ManualPlacementController

Build on top of existing systems.

---

## Scope

Implement only:

### Main Menu

Simple title screen.

Required:

* Game Title
* Start Game button
* Quit button (optional)

Flow:

Title
→ Start Game
→ Preparation

No advanced menu systems.

---

### Result Screen

Display result information.

Required:

StageClear

GameOver

Current HP

Restart button

Return to Title button

The result screen must appear when gameplay ends.

---

### Restart Flow

Player must be able to:

Restart Current Session

Return To Title

Reset Prototype State

Reuse existing reset systems whenever possible.

---

### HUD Improvements

Improve usability.

Display:

Current Game State

Player HP

Global Cooldown

Enemy Count

Current Enemy Pattern

Keep implementation lightweight.

Do not redesign PrototypeHud.

Extend it if necessary.

---

### Feedback Improvements

Provide simple feedback.

Examples:

Current Selection

StageClear Message

GameOver Message

Battle Started Message

Preparation Ready Message

Do not add complex UI frameworks.

---

## Explicitly Out Of Scope

Do NOT implement:

Shop

Reward

Economy

Campaign

Save / Load

Animation Systems

Audio Systems

VFX

Minimax

Advanced AI

Check

Checkmate

Threat Maps

Progression Systems

AR Features

---

## Acceptance Criteria

1. Title screen appears on startup.
2. Start Game enters Preparation.
3. StageClear screen appears when Enemy King is defeated.
4. GameOver screen appears when Player HP reaches 0.
5. Restart button resets gameplay correctly.
6. Return To Title works correctly.
7. HUD displays gameplay information correctly.
8. Existing systems remain functional.
9. No Console Errors.
10. No Console Warnings.

---

## Editor Test Flow

Expected flow:

Enter Play Mode

→ Title Screen

→ Start Game

→ Preparation

→ Spawn Enemies

→ Start Battle

→ Play

→ StageClear or GameOver

→ Restart

→ Play Again

---

## Output Format

Please answer in this order:

1. Architecture risks and integration notes
2. File-by-file implementation plan
3. C# code
4. Unity Editor setup instructions
5. Manual test procedure
6. Known limitations
