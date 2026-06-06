# Prompt 01 - 6x6 Tactical Roguelike Architecture and MVP Planning

You are a senior Unity and C# game developer, as well as a technical director.

I am building a Unity game prototype for a university final project. I plan to use AI coding tools during development. I want a realistic, playable prototype with a strong core loop, reduced debugging burden, and an architecture that remains easy to explain during a final presentation.

For this request, do not implement the whole game. Based on the game design below, propose the MVP architecture and implementation plan.

==================================================
Highest Priority Rule
==================================================

Compile-safe code is the highest priority.

If any requirement is ambiguous, prefer a smaller working implementation over a larger incomplete architecture.

==================================================
Core Identity Rule
==================================================

The project must preserve its core identity:

```txt
real-time chess gameplay
+
cooldown management
+
lightweight roguelike strategy
```

Scope reduction should simplify implementation, not remove the game's identity.

==================================================
Final MVP Scope
==================================================

Implement ONE playable tactical stage.

The MVP should include:

- Configurable grid board, default 6x6
- Pre-stage loadout selection using a point budget
- Pre-stage placement on restricted player rows
- Real-time chess-like movement
- Capture rules
- Individual piece cooldowns
- Player global cooldown
- HP system
- Stage clear
- Game over
- Lightweight randomized enemy setup using predefined patterns

Do not include:

- RoundManager
- ShopManager
- RewardManager
- Full AugmentSystem
- Economy loop
- Campaign progression
- Enemy AI
- Save/load
- Audio pipeline
- Presentation framework

Future versions may restore 8x8, multiple stages, campaign progression, shop systems, rewards, enemy AI, and a full augment architecture.

==================================================
AI Implementation Guardrails
==================================================

- Prefer simpler architecture.
- Prefer explicit responsibilities.
- Avoid speculative future systems.
- Implement only the requested vertical slice at each stage.
- Avoid scope creep.
- Avoid unnecessary generic frameworks, service locators, factories, or enterprise patterns.
- Do not use external Dependency Injection frameworks such as Zenject or VContainer.
- Use Unity's built-in component-based design, C# Actions, ScriptableObjects, and simple Inspector references.
- Keep each class reasonably small. Prefer approximately 80 to 200 lines per class when practical.
- Architecture should remain explainable within a 5 to 10 minute university final presentation.

==================================================
Project Nature
==================================================

- Purpose: Unity game prototype for a university final project
- Goal: A working playable prototype, not a commercial-scale game
- MVP target: One tactical roguelike stage with preparation, placement, real-time chess movement, cooldowns, and win/lose conditions
- Development style: Use AI coding tools, but develop in small vertical slices that can be tested in Unity
- Priorities:
  1. Playable core loop
  2. Tactical loadout and placement strategy
  3. Cooldown-based decision making
  4. Simple win/lose flow
  5. Readable presentation layer

==================================================
Development Standards
==================================================

- Use Unity 6.3 LTS, version 6000.3.11f1, as the target version.
- Use modern Unity C# only when it is stable, readable, and compatible with Unity serialization and Inspector workflows.
- Prioritize compile-safe and presentation-explainable code over language-feature-heavy code.
- The visual direction is a 3D-based board with 2D sprite pieces.
- Use a 3D board made of tiles, planes, or simple meshes.
- Represent chess pieces with 2D sprites or billboard-style views.
- Use a fixed top-down or slightly angled isometric camera.
- Keep core gameplay logic independent from SpriteRenderer, MeshRenderer, Animator, and camera-specific code.
- Use simple tile highlights for selection, placement, valid movement, and capture targets.
- Do not spend early implementation time on complex VFX, animations, sound, or detailed UI.

==================================================
Folder Structure Convention
==================================================

Use a simple project folder structure similar to:

```txt
Scripts/
 ├── Core/
 ├── Gameplay/
 │    ├── Board/
 │    ├── Pieces/
 │    ├── Cooldown/
 │    ├── Stage/
 │    ├── Health/
 │    └── Preparation/
 ├── Data/
 └── Presentation/
```

Use namespaces when appropriate, but keep them lightweight and easy to explain.

==================================================
Board Scope
==================================================

Replace the original 8x8 board with a configurable board.

Default MVP board:

```txt
6x6 configurable board
```

Implementation rule:

```csharp
[SerializeField]
private int boardWidth = 6;

[SerializeField]
private int boardHeight = 6;
```

BoardManager must support configurable board size. Do not hardcode 6x6 logic into movement systems. Future versions may restore 8x8.

Coordinates:

- Coordinate (0,0) is the bottom-left from the player's perspective
- x means file
- y means rank

==================================================
Core Gameplay Systems
==================================================

These systems are mandatory:

- Grid board
- Piece selection
- Movement validation
- Capture rules
- Individual cooldown
- Player global cooldown
- HP system
- Stage clear
- Game over

Piece scope:

- King
- Rook
- Knight
- Pawn

Other pieces are future expansion.

Win condition:

```txt
Enemy King captured
-> StageClear
```

Loss condition:

```txt
Player HP = 3
Player King captured -> HP decreases
HP 0 -> GameOver
```

==================================================
Pre-Stage Loadout System
==================================================

Add a lightweight strategy layer before battle.

The player builds a team using a point budget.

Rules:

- Configurable max loadout cost
- Each piece has loadoutCost
- King is required
- King costs 0
- Total cost cannot exceed budget

Recommended values:

```txt
Max Cost = 7
King = 0
Pawn = 1
Knight = 2
Rook = 3
```

Example valid loadout:

```txt
King + Rook + Knight + Pawn + Pawn = 7
```

Do not implement inventory systems, shop systems, or persistent economy.

==================================================
Pre-Stage Placement
==================================================

Before Playing, the player places selected pieces.

Allowed player placement rows:

```txt
y = 0
y = 1
```

Allowed enemy rows:

```txt
y = boardHeight - 2
y = boardHeight - 1
```

For the default 6x6 board, enemy rows are:

```txt
y = 4
y = 5
```

Placement rules:

- Cannot exceed loadout cost
- Cannot overlap placement
- Cannot start without King
- Player placement area is restricted to y=0 and y=1
- Enemy placement area is restricted to the top two rows

Recommended MVP interaction:

```txt
click piece button
-> click valid tile
-> piece placed
```

Avoid drag-and-drop, inventory systems, and advanced UI frameworks.

==================================================
Random Enemy Setup
==================================================

Keep a lightweight roguelike feeling with predefined patterns or simple random enemy compositions.

Allowed examples:

```txt
Pattern A:
King
Rook
Pawn

Pattern B:
King
Knight
Pawn
Pawn

Pattern C:
King
Rook
Knight
```

Do not implement procedural generation systems.

==================================================
Architecture Design Request
==================================================

Propose responsibilities for these MVP systems:

- GameManager
  - Manages Boot, StageStart, Preparation, Playing, StageClear, GameOver, and optional Result
  - Orchestrates one-stage lifecycle
  - Starts preparation, starts battle, receives capture notifications

- BoardManager
  - Manages configurable board width and height
  - Manages GridPosition validation and tile occupancy
  - Converts between board coordinates and world positions

- BoardView
  - Displays simple 3D board tiles
  - Handles placement, selection, valid movement, and capture highlights
  - Does not own gameplay rules

- PieceController
  - Manages piece type, owner, position, capture status, definition, cooldown, and view

- PieceView
  - Displays a piece using a 2D sprite or billboard in the 3D scene
  - Leaves hooks for later animation and VFX

- IMovementBehaviour
  - Calculates valid movement positions for one piece type
  - Use KingMovement, RookMovement, KnightMovement, and PawnMovement
  - Avoid one large switch statement

- PieceCooldown
  - Component attached to each piece
  - Manages individual cooldown with simple float timers

- PlayerGlobalCooldown
  - Lightweight handler for player global cooldown
  - Avoid a large centralized cooldown manager

- PlayerHealthService
  - Starts at HP 3
  - Reduces HP when player king is captured
  - Publishes health changed and depleted events

- PreparationManager
  - Manages selected loadout before battle
  - Enforces max loadout cost
  - Ensures King is included
  - Requests placement through PlacementValidator

- PlacementValidator
  - Validates player and enemy placement rows
  - Prevents overlap
  - Checks whether battle can start

- StartingLoadoutDefinition
  - Optional ScriptableObject for piece options, loadout cost, and default budget

- EnemySetupDefinition
  - Optional ScriptableObject for predefined enemy patterns

Do not make RoundManager, ShopManager, RewardManager, or AugmentSystem part of the MVP.

==================================================
GameState Flow Request
==================================================

Use this simplified state flow:

- Boot
- StageStart
- Preparation
- Playing
- StageClear
- GameOver

Optional:

- Result

Recommended transitions:

```txt
Boot
-> StageStart
-> Preparation
-> Playing

Preparation + ValidLoadoutAndPlacement
-> Playing

Playing + EnemyKingCaptured
-> StageClear
-> Result (optional)

Playing + PlayerHealthDepleted
-> GameOver
```

GameOver is terminal.

==================================================
Event Structure Request
==================================================

Propose an event-based structure so UI, board, pieces, preparation, and stage logic are not tightly coupled.

Example events:

- OnStageStarted(StageEventData data)
- OnPreparationStarted(StageEventData data)
- OnLoadoutChanged(LoadoutEventData data)
- OnPiecePlaced(PlacementEventData data)
- OnBattleStarted(StageEventData data)
- OnPlayerHealthChanged(HealthEventData data)
- OnPlayerKingCaptured(CombatEventData data)
- OnEnemyKingCaptured(CombatEventData data)
- OnStageCleared(StageEventData data)
- OnGameOver(StageEventData data)

Prefer small event payload data structures instead of long primitive parameter lists.

==================================================
ScriptableObject Structure Request
==================================================

ScriptableObject Usage Policy:

- Use ScriptableObjects for immutable configuration data.
- Do not store mutable runtime gameplay state inside ScriptableObjects.
- Runtime state should remain in MonoBehaviours or pure runtime C# objects.

Judge whether these data types should be ScriptableObjects:

- PieceDefinition
  - Piece type
  - Base cooldown
  - Movement behaviour type
  - Display name
  - Icon or prefab reference
  - loadoutCost

- StageDefinition
  - Stage ID
  - Stage name
  - Board width
  - Board height
  - Player HP
  - Max loadout cost
  - Enemy setup options

- StartingLoadoutDefinition
  - Available player piece options
  - Max loadout cost
  - Required King rule

- EnemySetupDefinition
  - Enemy pattern name
  - Enemy composition
  - Enemy spawn layout
  - Optional random weight

Do not include RoundDefinition, BossDefinition, RewardPolicy, Shop data, or full AugmentDefinition as mandatory MVP data.

==================================================
Recommended Timeline
==================================================

- Day 1: Architecture, GameManager, health flow
- Day 2: Board and configurable 6x6 setup
- Day 3: Loadout and placement
- Day 4: Movement rules and capture
- Day 5: Cooldowns and global cooldown
- Day 6: Random enemy setup and UI
- Optional Day 7 to 9: cleanup and presentation polish

Target workload: approximately 25 to 45 focused hours.

==================================================
Output Format
==================================================

Please answer in this order:

1. Realistic 6x6 Tactical Roguelike MVP scope
2. Overall architecture and core data flow
3. Responsibility map for the main classes
4. Simplified GameState flow explanation
5. ScriptableObject data structure proposal
6. First vertical slice scope and acceptance criteria
7. C# files to implement in the next prompt
8. Unity Editor setup strategy, without implementation code
9. Visual and feedback policy for the MVP and later polish
10. Future expansion list
11. Points that would be appealing in a final project presentation
