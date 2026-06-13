# Prompt08 Implementation Summary

## Goal

Balance the three-stage MVP and prevent trivial Enemy King clears without changing movement, capture, cooldown, AI, retry, stage progression, or victory rules.

## Final Implementation

### Stage Loadout Limits

```txt
Stage 1: Max Cost 3
Stage 2: Max Cost 5
Stage 3: Max Cost 7
```

`PreparationManager` uses `GameManager.CurrentStageMaxLoadoutCost` for piece addition, battle-start validation, loadout events, and HUD display.

### Stage-Aware Quick Setup

```txt
Stage 1: King + Rook = 3
Stage 2: King + Rook + Knight = 5
Stage 3: King + Rook + Knight + Pawn + Pawn = 7
```

Quick Setup uses the existing Preparation placement APIs and cannot create an over-budget loadout.

### Protected Pair Encounters

```txt
Stage 1: King/Pawn + Rook/Pawn
Enemy Count: 4

Stage 2: King/Pawn + Knight/Pawn + Extra Pawn
Enemy Count: 5

Stage 3: King/Pawn + Rook/Pawn + Knight/Pawn
Enemy Count: 6
```

Every important enemy piece has a supporting Pawn directly below it. An aligned Player Rook can target the Pawn first, but not the protected King, Rook, or Knight.

### Randomized Pair Placement

The Pattern assets store canonical pair relationships. When a new stage begins:

```txt
Select the deterministic stage pattern
-> Shuffle available enemy columns
-> Move each protected piece together with its Pawn
-> Place extra Pawns in separate columns
-> Cache the generated runtime entries
-> Spawn the encounter
```

Randomization occurs only when entering a new stage.

### Retry Layout Preservation

```txt
Player King Captured
-> Clear runtime enemy instances
-> Preserve ActiveSetup and generated entry cache
-> Return to Preparation
-> Respawn the exact cached coordinates
```

Quick Setup and Start Battle also reuse the cached layout. They do not reroll the encounter.

### Spawn Validation

`EnemySetupManager` rejects an encounter when:

* It does not contain exactly one Enemy King.
* A King, Rook, or Knight has no Pawn directly below it.
* Protected pairs and extra Pawns exceed the available board columns.
* An entry is missing its piece definition or fails normal placement validation.

### HUD

The existing IMGUI Preparation HUD displays:

```txt
Current Stage: Stage N
Loadout Cost: current / stage maximum
```

The Prompt07 button-first order remains unchanged, so `Start Battle` stays visible.

## Modified Gameplay Files

```txt
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Gameplay/Preparation/PreparationManager.cs
Assets/Scripts/Gameplay/Stage/EnemySetupManager.cs
Assets/Scripts/Gameplay/Stage/EnemySpawnEntry.cs
Assets/Scripts/Presentation/PrototypeHud.cs
Assets/Data/EnemySetups/PatternA_KingRookPawn.asset
Assets/Data/EnemySetups/PatternB_KingKnightPawnPawn.asset
Assets/Data/EnemySetups/PatternC_KingRookKnight.asset
```

No new script files or scene root GameObjects were added.

## Validation Results

```txt
Stage max costs 3 / 5 / 7: PASS
Stage-aware Quick Setup loadouts: PASS
Stage 1 and Stage 2 over-budget rejection: PASS
Stage 3 full loadout acceptance: PASS

Stage 1 protected pairs and 4-enemy count: PASS
Stage 2 protected pairs, extra Pawn, and 5-enemy count: PASS
Stage 3 three protected pairs and 6-enemy count: PASS
All protected pieces require Pawn removal or bypass: PASS

Quick Setup preserves generated layout and board occupancy: PASS
Player King retry preserves generated coordinates: PASS
New stage creates a fresh runtime layout cache: PASS
Randomized column variety across new generations: PASS

First Move Phase: PASS
Player King retry flow: PASS
Stage 1 -> Stage 2 -> Stage 3 progression: PASS
Stage 3 clear -> Victory: PASS

Script validation errors: 0
Script validation warnings: 0
Console Errors: 0
Console Warnings: 0
Scene validation issues: 0
Missing scripts: 0
Broken prefabs: 0
```

## Issues Fixed

* The initial spawn-validation insertion order caused `CS0841`; the call was moved after entry retrieval.
* Quick Setup clears board occupancy through Preparation reset, so it now respawns the same cached encounter to restore occupancy without rerolling.

## Scope Protection

Prompt08 did not add or change advanced chess rules, AI strategy, movement, capture, cooldown, economy, save/load, Canvas UI, animation, audio, or VFX.

## Conclusion

Prompt08 is complete and accepted. Player power increases through the `3 / 5 / 7` stage budgets, while enemy board presence increases through `4 / 5 / 6`-piece protected structures. Each new stage receives one randomized protected-pair layout, and every retry reuses that exact encounter.
