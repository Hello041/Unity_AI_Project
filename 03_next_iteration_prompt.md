Prompt02 is complete and accepted.

Do not rewrite, replace, or refactor:

- GameManager
- GameState
- StageDefinition
- PlayerHealthService
- HealthEventData
- StageEventData

unless a minimal integration change is strictly required.

Build only the systems requested in Prompt03.

# Prompt 03 - Board, Loadout, Placement, Pieces, Capture, and Cooldown Next Step

You are a senior Unity and C# game developer.

Continue from the previous implementation where GameManager, PlayerHealthService, StageDefinition, StageEventData, HealthEventData, and GameState already exist.

Now design and implement the next small, testable vertical slice: configurable 6x6 board support, pre-stage loadout validation, pre-stage placement validation, piece placement, movement calculation, capture, and the first version of real-time cooldown logic.

Do not implement the full game yet. Keep this step focused and directly testable inside the Unity Editor.

The visual direction is a 3D-based board with 2D sprite pieces. The camera can be fixed because this is a tactical roguelike board game. Prefer a fixed top-down or slightly angled isometric view.

For this step, implement only simple MVP visuals: 3D board tiles, 2D sprite or billboard pieces, placement highlights, movement highlights, and capture highlights. Do not implement complex VFX, sound, animation, or detailed UI yet.

==================================================
Highest Priority Rule
==================================================

Compile-safe code is the highest priority.

If any requirement is ambiguous, prefer a smaller working implementation over a larger incomplete architecture.

==================================================
Core Identity Rule
==================================================

The project must preserve:

```txt
real-time chess gameplay
+
cooldown management
+
lightweight roguelike strategy
```

This step is where the tactical chess identity becomes visible.

==================================================
AI Implementation Guardrails
==================================================

- Prefer simpler architecture.
- Prefer explicit responsibilities.
- Avoid speculative future systems.
- Implement only this requested vertical slice.
- Avoid scope creep.
- Keep each class reasonably small. Prefer approximately 80 to 200 lines per class when practical.
- Do not introduce unnecessary generic frameworks, service locators, factories, or enterprise patterns.
- Do not use external Dependency Injection frameworks such as Zenject or VContainer.
- Architecture should remain explainable within a 5 to 10 minute university final presentation.

==================================================
Existing Game Context
==================================================

The game is a 6x6 tactical roguelike-lite MVP.

- One playable stage
- Configurable board, default 6x6
- Coordinate (0,0) is the bottom-left from the player's perspective
- x means file
- y means rank
- Pre-stage loadout using a point budget
- Pre-stage placement on restricted rows
- The game is real-time once Playing starts
- Each piece has an individual cooldown
- When the player moves any allied piece, all allied pieces receive a global cooldown
- Enemy king capture notifies GameManager.NotifyEnemyKingCaptured()
- Player king capture notifies GameManager.NotifyPlayerKingCaptured()
- Player HP depletion means GameOver
- No campaign progression
- No shop
- No full reward economy
- No full augment architecture
- No enemy AI

==================================================
Implementation Scope
==================================================

Implement or propose code for the following systems:

1. GridPosition value type
2. BoardManager
3. BoardView
4. PieceType enum
5. PieceOwner enum
6. PieceDefinition ScriptableObject
7. PieceController
8. PieceView
9. IMovementBehaviour
10. KingMovement
11. RookMovement
12. KnightMovement
13. PawnMovement
14. PieceCooldown component
15. PlayerGlobalCooldown lightweight handler
16. PreparationManager
17. PlacementValidator
18. Optional StartingLoadoutDefinition
19. Optional EnemySetupDefinition

Keep the implementation simple enough for a university prototype.

Recommended folder placement:

```txt
Scripts/
 ├── Gameplay/Board/
 ├── Gameplay/Pieces/
 ├── Gameplay/Cooldown/
 ├── Gameplay/Preparation/
 ├── Data/
 └── Presentation/
```

==================================================
GridPosition Requirements
==================================================

GridPosition must be an immutable readonly struct containing int x and int y.

It must:

- Implement IEquatable<GridPosition>
- Override Equals
- Override GetHashCode
- Provide useful helpers if needed, such as ToString
- Be safe for dictionaries and coordinate comparisons

==================================================
Board Requirements
==================================================

- BoardManager must support configurable width and height.
- Default board size is 6x6.
- Use serialized values:

```csharp
[SerializeField]
private int boardWidth = 6;

[SerializeField]
private int boardHeight = 6;
```

- Do not hardcode 6x6 logic into movement systems.
- Store tile occupancy.
- Convert board coordinates to world positions.
- Validate whether a coordinate is inside the current board.
- Allow a piece to be placed at a board coordinate.
- Allow a piece to move from one coordinate to another.
- Prevent movement to invalid coordinates.
- Prevent movement onto allied occupied tiles.
- Allow movement onto enemy occupied tiles and treat that as capture.

==================================================
Loadout Requirements
==================================================

Add a lightweight strategy layer before battle.

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

Do not implement inventory systems, shops, persistent economy, or advanced loadout UI.

==================================================
Placement Requirements
==================================================

Before Playing, the player places selected pieces.

Allowed player rows:

```txt
y = 0
y = 1
```

Allowed enemy rows:

```txt
y = boardHeight - 2
y = boardHeight - 1
```

For the default 6x6 board, enemy rows are y=4 and y=5.

Rules:

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

Avoid drag-and-drop, inventory systems, hover frameworks, camera control systems, and advanced UI frameworks.

==================================================
Random Enemy Setup
==================================================

Keep a lightweight roguelike feeling.

Allowed:

- Predefined patterns
- Simple random enemy compositions

Examples:

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

Do not implement procedural generation or enemy AI.

==================================================
Piece Requirements
==================================================

- Each piece should know:
  - PieceType
  - PieceOwner
  - current GridPosition
  - PieceDefinition
  - loadoutCost through PieceDefinition
  - whether it is captured
  - whether it is ready to move
- Implement:
  - King
  - Rook
  - Knight
  - Pawn
- Other pieces are future expansion only.

Ownership Rules:

- Player pieces belong to PieceOwner.Player.
- Enemy pieces belong to PieceOwner.Enemy.
- Movement rules must block allied occupied tiles.
- Movement rules must allow enemy capture.
- Sliding movement, such as Rook movement, must stop after collision.

==================================================
Movement Rule Requirements
==================================================

- Movement calculation should not directly depend on rendering.
- Define IMovementBehaviour and separate movement classes:
  - KingMovement
  - RookMovement
  - KnightMovement
  - PawnMovement
- Avoid putting every movement rule into one large switch statement.
- Movement behaviours should expose:

```csharp
List<GridPosition> GetValidMoves(
    PieceController piece,
    BoardManager board
)
```

- Use List<GridPosition> for MVP simplicity.
- Avoid premature optimization.

==================================================
Cooldown Requirements
==================================================

Cooldown Ownership Guideline:

- Prefer a PieceCooldown component attached to each piece.
- Use a lightweight player global cooldown handler.
- Avoid large centralized cooldown managers.

Rules:

- Each piece has an individual cooldown.
- Cooldown values come from PieceDefinition.
- A piece can move only when:
  - its individual cooldown is ready
  - the player global cooldown is not active, if the piece belongs to the player
- When the player moves an allied piece:
  - start that piece's individual cooldown
  - start the player global cooldown
- The global cooldown duration should be configurable between 1 and 3 seconds.
- Use simple float timers with Time.deltaTime.
- Avoid LINQ, lambda expressions, foreach over temporary collections, or creating new collections inside any Update loop.
- Cache references during initialization instead of calling GetComponent repeatedly in Update.

Update Usage Guideline:

- Only systems that require real-time timer progression, such as cooldown updates, should use Update.
- Avoid placing gameplay orchestration, selection logic, movement validation, or board scanning inside Update.

==================================================
Visual and Camera Requirements
==================================================

- Use a 3D scene as the base.
- The board can be built from simple cube or plane tiles.
- Pieces should use 2D sprites placed above the board.
- If needed, pieces can use a billboard behavior so they face the fixed camera.
- The camera should be fixed in a top-down or slightly angled isometric position.
- Do not make gameplay rules depend on SpriteRenderer, MeshRenderer, or Camera.
- BoardView should handle tile visuals and highlights.
- PieceView should handle sprite display and visual feedback.
- Use simple colors or materials for placement tiles, selected tiles, valid movement tiles, and capture target tiles.

==================================================
Integration Requirements
==================================================

- If an enemy king is captured, call GameManager.NotifyEnemyKingCaptured().
- If the player king is captured, call GameManager.NotifyPlayerKingCaptured().
- Do not implement full enemy AI yet.
- Movement can be triggered through Inspector test methods, temporary keyboard input, or a minimal mouse selection flow.
- PreparationManager should call GameManager.StartBattle() only when loadout and placement are valid.

==================================================
Code Style Requirements
==================================================

- Target Unity 6.3 LTS, version 6000.3.11f1.
- Use Unity C#.
- Use modern Unity C# only when it remains compile-safe and inspector-friendly.
- Use SerializeField instead of public fields.
- Keep MonoBehaviour and pure logic classes separated when it helps readability.
- Avoid excessive abstraction.
- Avoid calling GetComponent repeatedly in Update.
- Avoid allocations inside Update loops.
- Use Unity Inspector references, ScriptableObjects, and simple C# Actions for dependency wiring.
- Use ScriptableObjects only for immutable configuration data.
- Do not store mutable runtime gameplay state inside ScriptableObjects.
- Include null checks and basic defensive code.
- Include short comments only where they explain important decoupling choices.

==================================================
Acceptance Criteria
==================================================

The next step is complete when:

1. A configurable board can be represented in code, defaulting to 6x6.
2. Movement systems use board width and height instead of hardcoded 6x6 logic.
3. Loadout cost can be validated.
4. King is required in the player loadout.
5. Pieces can be placed only on valid placement rows.
6. Placement overlap is prevented.
7. A selected piece can calculate valid movement positions.
8. King, Rook, Knight, and Pawn have basic movement rules.
9. A piece can move to an empty tile.
10. A piece can capture an enemy piece.
11. Capturing an enemy king notifies GameManager.
12. Capturing the player king notifies GameManager.
13. Individual cooldown prevents repeated movement before recharge.
14. Global cooldown prevents other allied pieces from moving immediately after one allied piece moves.
15. Simple tile highlights show placement, selected, valid move, and capture target states.
16. No full input architecture, shop, reward economy, round progression, enemy AI, save/load, or full augment framework is created.

==================================================
Output Format
==================================================

Please answer in this order:

1. Short explanation of this vertical slice
2. Class responsibility summary
3. File-by-file C# code
4. Unity Editor setup guide
5. Manual test procedure
6. Fixed camera setup guide for the 3D board and 2D sprite pieces
7. Notes on future feedback extension points for VFX, sound, animation, cooldown UI, result UI, and stage transition UI
8. How this connects to the next step: random enemy pattern selection, result display, and presentation polish
