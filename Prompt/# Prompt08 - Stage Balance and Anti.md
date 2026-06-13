# Prompt08 - Stage Balance and Anti-Instant-Clear Rules

This Unity project is a continuation of an existing Tactical Chess Roguelike MVP.

Use the attached project documents as the source of truth.

Prompt01 through Prompt07 are complete and accepted.

Implement Prompt08 only.

Do not redesign existing systems.

---

## Current Project State

Implemented and verified:

* Core architecture
* Board system
* Piece system
* Movement system
* Capture system
* Cooldown system
* Enemy AI
* Preparation phase
* Placement system
* Retry system
* HP system
* First Move Phase
* Stage progression
* Victory state
* Stage 1 → Stage 2 → Stage 3 flow
* Automatic enemy spawning before Preparation
* Prototype HUD improvements

Current status:

* Console Errors = 0
* Console Warnings = 0
* Scene validation issues = 0

---

## Development Philosophy

Keep changes minimal.

Reuse existing systems whenever possible.

Do not redesign completed systems.

Extend existing architecture instead of replacing it.

Do not introduce new gameplay systems unless strictly required.

---

## Prompt08 Goal

Improve stage balance and prevent instant stage clear exploits.

Current issue:

A player can place a Rook directly in line with the Enemy King, start battle, move first, and immediately capture the Enemy King.

This makes some stages too easy and reduces tactical value.

Prompt08 should solve this through:

1. Stage-based player loadout cost limits
2. Stage-aware Setup Player MVP button behavior
3. Enemy King protection through Enemy Pattern data
4. Spawn validation for Enemy King protection
5. Minimal HUD display updates

---

# Required Changes

## 1. Stage-Based Loadout Cost Limits

Implement stage-specific player max cost:

```txt
Stage 1 = Max Cost 3
Stage 2 = Max Cost 5
Stage 3 = Max Cost 7
```

Expected behavior:

* Stage 1 only allows a small early loadout.
* Stage 2 allows a moderate loadout.
* Stage 3 allows the existing full MVP loadout.
* Existing loadout validation must use the current stage max cost.
* Do not create economy, currency, shop, reward, or meta progression systems.

Preferred implementation:

* Add a public read-only property to `GameManager`, for example:

```csharp
public int CurrentStageMaxLoadoutCost
{
    get
    {
        return currentStage switch
        {
            1 => 3,
            2 => 5,
            3 => 7,
            _ => 7
        };
    }
}
```

Use the actual existing current stage field name from `GameManager`.

* Update `PreparationManager` so all cost validation uses the current stage max cost instead of a fixed cost of 7.

---

## 2. Setup Player MVP Must Respect Stage Cost

The existing `Setup Player MVP` button must become stage-aware.

It must create a valid loadout for the current stage.

Use these loadouts:

### Stage 1 MVP Loadout

```txt
King
Rook
```

Cost:

```txt
0 + 3 = 3
```

### Stage 2 MVP Loadout

```txt
King
Rook
Knight
```

Cost:

```txt
0 + 3 + 2 = 5
```

### Stage 3 MVP Loadout

Use the existing full MVP loadout:

```txt
King
Rook
Knight
Pawn
Pawn
```

Cost:

```txt
0 + 3 + 2 + 1 + 1 = 7
```

Expected behavior:

* Clicking `Setup Player MVP` in Stage 1 creates and places only the Stage 1 valid loadout.
* Clicking `Setup Player MVP` in Stage 2 creates and places only the Stage 2 valid loadout.
* Clicking `Setup Player MVP` in Stage 3 creates and places the existing full MVP loadout.
* The button must not create an over-budget loadout.
* It must reuse existing Preparation and placement logic as much as possible.

---

## 3. Enemy King Protection Rule

Do not modify movement rules.

Do not modify Rook movement.

Do not add check, checkmate, threat map, or advanced tactical analysis.

Enemy King protection must be solved through:

```txt
Enemy Pattern data
+
Spawn validation
```

Expected behavior:

* Enemy King should never start directly exposed to a first-move Player Rook capture.
* Each stage enemy setup should place at least one enemy piece in front of or near the Enemy King as a blocker.
* The blocker should prevent direct open-file Rook capture from the player side.
* Prefer modifying existing `PatternA`, `PatternB`, and `PatternC` enemy setup assets.

Suggested 6x6 pattern layout:

### Pattern A / Stage 1

```txt
Enemy King: (2, 5)
Enemy Pawn: (2, 4)
Enemy Rook: (4, 5)
```

### Pattern B / Stage 2

```txt
Enemy King: (3, 5)
Enemy Pawn: (3, 4)
Enemy Knight: (1, 5)
Enemy Pawn: (4, 4)
```

### Pattern C / Stage 3

```txt
Enemy King: (2, 5)
Enemy Knight: (2, 4)
Enemy Rook: (4, 5)
```

If these exact coordinates conflict with existing serialized data or validation, choose equivalent valid enemy-row coordinates that satisfy the same rule.

Enemy placement rows must remain:

```txt
y = boardHeight - 2
y = boardHeight - 1
```

For the current 6x6 board:

```txt
y = 4
y = 5
```

---

## 4. Enemy King Protection Spawn Validation

Add minimal spawn validation inside the existing `EnemySetupManager`.

Validation should check:

1. The setup contains exactly one Enemy King.
2. The Enemy King has a protecting enemy piece in front of it from the player side.
3. The setup should not allow direct open-file first-move Rook capture.

Minimum acceptable validation:

```txt
Enemy King exists
AND
there is an enemy piece at (king.x, king.y - 1)
```

Because enemies spawn on top rows and the player side is below, `(king.x, king.y - 1)` is the most important blocker position.

Expected behavior:

* Valid protected setups spawn normally.
* Invalid unprotected setups should not silently pass.
* Prefer logging a clear error and preventing invalid spawn.
* Do not produce warnings in the final accepted state.
* The final configured PatternA/B/C assets must pass validation.

Example error message:

```txt
Invalid enemy setup: Enemy King is not protected by a blocker.
```

---

## 5. HUD Update

Update the existing `PrototypeHud`.

During Preparation, show current stage cost information.

Example:

```txt
Stage 1
Loadout Cost: 3 / 3
```

```txt
Stage 2
Loadout Cost: 5 / 5
```

```txt
Stage 3
Loadout Cost: 7 / 7
```

Requirements:

* Keep IMGUI.
* Do not convert to Canvas UI.
* Do not redesign HUD layout.
* Keep the Prompt07 fix where Preparation buttons render before expanded status if needed.
* Ensure `Start Battle` remains visible.

---

# Important Restrictions

Do NOT implement:

* Shop
* Economy
* Currency
* Save / Load
* Campaign Map
* Meta Progression
* Permanent Unlocks
* New Piece Types
* Check / Checkmate Rules
* Threat Map System
* Advanced AI
* Minimax
* Canvas UI conversion
* Animation
* Audio
* VFX
* Multiplayer
* AR systems

---

# Preserve Existing Behavior

Do not break:

* Prompt06 First Move Phase
* Prompt06 Player King capture retry
* Prompt06 preserved loadout retry flow
* Prompt06 preserved enemy setup retry flow
* Prompt07 deterministic Stage 1 → Stage 2 → Stage 3 flow
* Prompt07 Victory after Stage 3
* Restart Session
* Return To Title
* Existing movement rules
* Existing capture rules
* Existing cooldown rules
* Existing Enemy AI behavior

Prompt07 Stage Flow must remain unchanged.

---

# Likely Modified Files

Expected modified files:

```txt
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Gameplay/Preparation/PreparationManager.cs
Assets/Scripts/Gameplay/Stage/EnemySetupManager.cs
Assets/Scripts/Presentation/PrototypeHud.cs
Assets/Data/EnemySetups/PatternA_KingRookPawn.asset
Assets/Data/EnemySetups/PatternB_KingKnightPawnPawn.asset
Assets/Data/EnemySetups/PatternC_KingRookKnight.asset
```

Do not add new C# scripts unless absolutely necessary.

Do not add new scene root GameObjects.

---

# Validation Checklist

After implementation, verify:

```txt
Stage 1 max loadout cost is 3
Stage 2 max loadout cost is 5
Stage 3 max loadout cost is 7

Stage 1 Setup Player MVP creates King + Rook only
Stage 2 Setup Player MVP creates King + Rook + Knight only
Stage 3 Setup Player MVP creates King + Rook + Knight + Pawn + Pawn

Stage 1 over-budget loadout is rejected
Stage 2 over-budget loadout is rejected
Stage 3 full MVP loadout is accepted

Preparation HUD shows current cost / current stage max cost

Pattern A Enemy King is protected
Pattern B Enemy King is protected
Pattern C Enemy King is protected

Player Rook cannot capture Enemy King immediately on first move
Player Rook can only capture the blocker first if aligned

Prompt06 First Move Phase still works
Prompt06 retry flow still works
Prompt07 Stage 1 → Stage 2 → Stage 3 still works
Stage 3 clear still enters Victory

Console Errors = 0
Console Warnings = 0
Scene validation issues = 0
```

---

# Required Output

After implementation, report:

1. Summary of changes
2. Modified files
3. Validation results
4. Any issues found and fixed

Respond in Korean.


## Additional Design Intent

The goal is not only to satisfy a technical blocker condition.

The resulting stage should require at least one meaningful tactical interaction before the Enemy King becomes capturable.

In other words:

* Do not solve the problem by placing a single disposable blocker that can be trivially removed with no decision making.
* Prefer enemy layouts that require the player to interact with at least one enemy piece before a direct Enemy King capture becomes available.
* The purpose is to improve tactical gameplay, not merely satisfy validation rules.
* Validation requirements remain important, but gameplay quality takes priority when choosing final enemy layouts.

Examples of meaningful interaction:

* Capturing or bypassing a protecting Pawn.
* Dealing with a protecting Knight.
* Navigating around a protecting Rook.
* Choosing between multiple capture targets before the Enemy King becomes exposed.

The final PatternA, PatternB, and PatternC layouts should satisfy both:

1. Technical protection validation.
2. Meaningful tactical protection during actual gameplay.

---

## Implementation Requirement

Do not only provide implementation guidance.

Apply Prompt08 directly to the current Unity project files.

Use the actual project files and existing architecture.

Modify the required files.

After implementation:

1. Provide a summary of changes.
2. List all modified files.
3. Report validation results.
4. Report any issues found and fixed.
5. Generate Prompt08_Implementation_Summary.md.
6. Update:

   * # Current Project Status.md
   * Folder Structure.md
   * Scene Hierarchy.md

Use the updated project state as the new source of truth.
