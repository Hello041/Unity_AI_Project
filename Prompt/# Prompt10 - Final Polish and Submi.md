# Prompt10 - Final Polish and Submission Validation

Use all attached documents as the source of truth.

Prompt01 through Prompt09 are complete and accepted.

The project is currently in a Feature Complete MVP state.

Prompt10 should focus on polish, usability improvements, presentation quality, and final validation.

Do not add new gameplay systems.

Do not redesign completed systems.

Do not modify stage flow, enemy AI behavior, retry behavior, encounter generation, loadout validation, or existing game rules unless strictly required for a Prompt10 feature.

---

## Goal

Improve usability and presentation quality before final submission.

Prompt10 should make the MVP feel complete without increasing project scope.

The focus is:

```txt
Usability
Readability
Presentation
Final Validation
```

---

## Priority 1 - Korean Localization

Convert player-facing UI text to Korean.

Examples:

```txt
Start Game
→ 게임 시작

Start Battle
→ 전투 시작

Setup Player MVP
→ 추천 배치

Stage Clear
→ 스테이지 클리어

Game Over
→ 게임 오버

Victory
→ 승리

Return To Title
→ 타이틀로

Restart Session
→ 다시 시작
```

Apply localization only to player-facing UI.

Do not rename internal classes, enums, methods, assets, or file names.

---

## Priority 2 - Preparation Selected Piece Feedback

Improve Preparation usability.

Problem:

The player can select a piece for placement, but the current selection is not always obvious.

Implement lightweight visual feedback.

Acceptable examples:

```txt
Selected piece button highlighted
Selected piece button color change
Selected piece label display
```

Do not add complex animations.

Do not create new gameplay systems.

Reuse existing UI where possible.

---

## Priority 3 - Cooldown Visualization Improvement

Current cooldown information is functional but minimal.

Improve readability.

Examples:

```txt
More readable global cooldown bar
Cooldown percentage
Cooldown remaining time
Ready / Cooldown state text
```

Per-piece cooldown bars are optional.

Do not redesign the cooldown system.

Presentation-only changes preferred.

---

## Priority 4 - UI Readability Pass

Review the Canvas UI layout.

Improve:

```txt
Text readability
Spacing
Alignment
Panel organization
Button consistency
Information hierarchy
```

Avoid overlapping information.

Keep board interaction unobstructed.

Do not move major gameplay systems into UI.

CanvasHud must remain a presentation layer.

---

## Priority 5 - Final Validation Pass

Perform a complete playthrough.

Validate:

```txt
Title Screen
Stage 1
Stage 2
Stage 3
Victory
GameOver
Retry
Restart Session
Return To Title
Canvas UI
PrototypeHud fallback
```

Confirm:

```txt
Console Errors = 0
Console Warnings = 0
Scene Validation Issues = 0
```

---

## Out of Scope

Do NOT implement:

```txt
New Piece Types
New Movement Rules
New AI Systems
Reward Systems
Economy
Shop
Currency
Meta Progression
Save / Load
Campaign Map
Achievements
Sound
Animation Systems
VFX Systems
Drag-and-Drop Controls
Touch Controls
```

Prompt10 is polish only.

---

## Acceptance Criteria

1. Korean UI text implemented.
2. Selected Preparation piece feedback added.
3. Cooldown display readability improved.
4. Canvas UI readability improved.
5. Full Stage 1 → Stage 2 → Stage 3 → Victory flow verified.
6. Retry verified.
7. Restart Session verified.
8. Return To Title verified.
9. Console Errors = 0.
10. Console Warnings = 0.
11. Scene Validation Issues = 0.

---

## Output Format

Respond in Korean.

Before implementation:

1. Explain implementation plan.
2. Identify affected files.
3. Identify risks.

After implementation:

1. Modified files.
2. Summary of improvements.
3. Validation results.
4. Known limitations.
5. Final submission readiness assessment.
