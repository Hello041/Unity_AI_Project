# Folder Structure

Current Unity project root:

```txt
C:/Unity_AI_Project/Project
```

This document reflects the Feature Complete MVP state after Prompt09 completion.

## Assets

```txt
Assets/
├─ Scenes/
│  └─ SampleScene.unity
│
├─ Data/
│  ├─ MvpStageDefinition.asset
│  │
│  ├─ Pieces/
│  │  ├─ KingDefinition.asset
│  │  ├─ RookDefinition.asset
│  │  ├─ KnightDefinition.asset
│  │  └─ PawnDefinition.asset
│  │
│  └─ EnemySetups/
│     ├─ PatternA_KingRookPawn.asset
│     ├─ PatternB_KingKnightPawnPawn.asset
│     └─ PatternC_KingRookKnight.asset
│
├─ Scripts/
│  ├─ Core/
│  │  ├─ GameManager.cs
│  │  ├─ GameState.cs
│  │  └─ StageEventData.cs
│  │
│  ├─ Data/
│  │  └─ StageDefinition.cs
│  │
│  ├─ Gameplay/
│  │  ├─ Board/
│  │  │  ├─ GridPosition.cs
│  │  │  ├─ BoardManager.cs
│  │  │  ├─ BoardView.cs
│  │  │  ├─ BoardTileView.cs
│  │  │  └─ BoardHighlightType.cs
│  │  │
│  │  ├─ Pieces/
│  │  │  ├─ PieceDefinition.cs
│  │  │  ├─ PieceController.cs
│  │  │  ├─ PieceView.cs
│  │  │  ├─ PieceType.cs
│  │  │  └─ PieceOwner.cs
│  │  │
│  │  ├─ Movement/
│  │  │  ├─ IMovementBehaviour.cs
│  │  │  ├─ MovementBehaviourResolver.cs
│  │  │  ├─ MovementRuleUtility.cs
│  │  │  ├─ PieceMovementService.cs
│  │  │  ├─ KingMovement.cs
│  │  │  ├─ RookMovement.cs
│  │  │  ├─ KnightMovement.cs
│  │  │  └─ PawnMovement.cs
│  │  │
│  │  ├─ Interaction/
│  │  │  ├─ PieceActionController.cs
│  │  │  └─ BoardInputController.cs
│  │  │
│  │  ├─ Preparation/
│  │  │  ├─ PreparationManager.cs
│  │  │  ├─ PlacementValidator.cs
│  │  │  ├─ ManualPlacementController.cs
│  │  │  ├─ LoadoutEventData.cs
│  │  │  └─ PlacementEventData.cs
│  │  │
│  │  ├─ Cooldown/
│  │  │  ├─ PieceCooldown.cs
│  │  │  └─ PlayerGlobalCooldown.cs
│  │  │
│  │  ├─ Health/
│  │  │  ├─ PlayerHealthService.cs
│  │  │  └─ HealthEventData.cs
│  │  │
│  │  ├─ Stage/
│  │  │  ├─ EnemySetupDefinition.cs
│  │  │  ├─ EnemySetupManager.cs
│  │  │  └─ EnemySpawnEntry.cs
│  │  │
│  │  └─ EnemyAI/
│  │     ├─ EnemyAIController.cs
│  │     ├─ EnemyMoveSelector.cs
│  │     ├─ PieceValueTable.cs
│  │     └─ EnemyActionDelay.cs
│  │
│  └─ Presentation/
│     ├─ CanvasHud.cs
│     └─ PrototypeHud.cs
│
├─ Sprite/
│  ├─ 16x16 pieces/
│  │  ├─ BlackPieces.png
│  │  ├─ BlackPieces_Simplified.png
│  │  ├─ BlackPieces_Wood.png
│  │  ├─ BlackPieces_WoodSimplified.png
│  │  ├─ WhitePieces.png
│  │  ├─ WhitePieces_Simplified.png
│  │  ├─ WhitePieces_Wood.png
│  │  └─ WhitePieces_WoodSimplified.png
│  │
│  ├─ 16x32 pieces/
│  │  ├─ W_King.png
│  │  ├─ W_Rook.png
│  │  ├─ W_Knight.png
│  │  ├─ W_Pawn.png
│  │  ├─ W_Bishop.png
│  │  ├─ W_Queen.png
│  │  ├─ B_King.png
│  │  ├─ B_Rook.png
│  │  ├─ B_Knight.png
│  │  ├─ B_Pawn.png
│  │  ├─ B_Bishop.png
│  │  ├─ B_Queen.png
│  │  ├─ WhitePieces-Sheet.png
│  │  ├─ WhitePiecesWood-Sheet.png
│  │  ├─ BlackPieces-Sheet.png
│  │  └─ BlackPiecesWood-Sheet.png
│  │
│  ├─ boards/
│  │  ├─ 6x6.png
│  │  ├─ board_persp_01.png
│  │  ├─ board_persp_02.png
│  │  ├─ board_persp_03.png
│  │  ├─ board_persp_04.png
│  │  ├─ board_persp_05.png
│  │  ├─ board_plain_01.png
│  │  ├─ board_plain_02.png
│  │  ├─ board_plain_03.png
│  │  ├─ board_plain_04.png
│  │  └─ board_plain_05.png
│  │
│  ├─ cursor.png
│  └─ README.txt
│
├─ Screenshots/
│  ├─ slice02_board_centered.png
│  ├─ slice02_board_check.png
│  ├─ slice03_pieces_check.png
│  ├─ slice03_pieces_height_raised.png
│  ├─ slice09_hud_check.png
│  ├─ slice09_hud_screen_check.png
│  ├─ prompt06_title_screen.png
│  ├─ prompt06_preparation_hud.png
│  ├─ prompt06_stage_clear.png
│  ├─ prompt06_game_over.png
│  ├─ prompt06_ux_first_move_verified.png
│  ├─ prompt06_ux_active_hud_final.png
│  └─ prompt07_stage1_preparation.png
│
├─ Settings/
│  ├─ DefaultVolumeProfile.asset
│  ├─ Mobile_Renderer.asset
│  ├─ Mobile_RPAsset.asset
│  ├─ PC_Renderer.asset
│  ├─ PC_RPAsset.asset
│  ├─ SampleSceneProfile.asset
│  └─ UniversalRenderPipelineGlobalSettings.asset
│
├─ TutorialInfo/
│  ├─ Icons/
│  │  └─ URP.png
│  ├─ Scripts/
│  │  ├─ Readme.cs
│  │  └─ Editor/
│  │     └─ ReadmeEditor.cs
│  └─ Layout.wlt
│
├─ InputSystem_Actions.inputactions
└─ Readme.asset
```

## Current MVP Script Groups

### Core Flow

```txt
GameManager
GameState
StageEventData
StageDefinition
```

Handles:

```txt
Boot
StageStart
Preparation
Playing
StageClear
Victory
GameOver
```

Prompt08 modification:

```txt
GameManager exposes the current stage max loadout cost:
Stage 1 = 3, Stage 2 = 5, Stage 3 = 7.
```

Prompt06 modification:

```txt
GameManager publishes a battle reset request when the Player King is captured and HP remains.
The retry returns to Preparation without resetting stage HP or the selected encounter.
```

Prompt07 modification:

```txt
GameManager tracks the deterministic three-stage session.
Each stage encounter is generated before Preparation.
StageClear advances automatically, and Stage 3 clear enters Victory.
```

### Health

```txt
PlayerHealthService
HealthEventData
```

Handles player HP reset, damage, and HP depletion event.

### Board

```txt
GridPosition
BoardManager
BoardView
BoardTileView
BoardHighlightType
```

Handles configurable board size, coordinate conversion, occupancy, tile creation, and highlight colors.

### Pieces

```txt
PieceDefinition
PieceController
PieceView
PieceType
PieceOwner
```

Handles piece data, ownership, board registration, capture state, cooldown reference, and sprite display.

### Movement

```txt
IMovementBehaviour
MovementBehaviourResolver
MovementRuleUtility
PieceMovementService
KingMovement
RookMovement
KnightMovement
PawnMovement
```

Handles implemented MVP movement rules for King, Rook, Knight, and Pawn.

### Interaction

```txt
PieceActionController
BoardInputController
```

Handles selected piece movement, capture, cooldown start, and guarded board click flow.

Note:

```txt
BoardInputController supports New Input System mouse clicks.
Mouse position is converted to a board tile through the existing camera and Physics raycast.
Preparation clicks continue to route through ManualPlacementController.
Playing clicks support player selection, movement, and capture through existing systems.
Legacy Input Manager support remains available through conditional compilation.
BoardInputController exposes the current selected player piece for the Prompt06 HUD.
```

### Preparation

```txt
PreparationManager
PlacementValidator
ManualPlacementController
LoadoutEventData
PlacementEventData
```

Handles loadout budget, King requirement, placement rows, overlap prevention, and battle start validation.

Prompt08 modifications:

```txt
All loadout cost validation uses the active stage maximum.
Loadout events and battle-start checks report and enforce the same stage budget.
```

Prompt06 modifications:

```txt
Exactly one Player King is allowed.
Placed player pieces can be repositioned during Preparation.
The selected loadout is preserved after a non-terminal Player King capture.
Preserved loadout entries are reused during retry placement.
```

### Cooldown

```txt
PieceCooldown
PlayerGlobalCooldown
```

Handles individual piece cooldowns and player global cooldown.

### Enemy Setup

```txt
EnemySetupDefinition
EnemySetupManager
EnemySpawnEntry
```

Handles deterministic stage encounter spawning while retaining the older random setup API for backward compatibility.

Prompt06 modifications:

```txt
Enemy runtime instances can be cleared without clearing ActiveSetup.
The same active setup is respawned for a Player King capture retry.
SpawnedEnemyCount reflects active, non-captured enemies.
```

Prompt07 modifications:

```txt
SpawnSetupForStage maps Stage 1, Stage 2, and Stage 3 to fixed patterns.
The random setup API remains backward compatible but is no longer used by the player workflow.
```

Prompt08 modifications:

```txt
Pattern assets define canonical King/Pawn, Rook/Pawn, and Knight/Pawn pairs.
Each setup must contain exactly one Enemy King.
Every King, Rook, and Knight must have a Pawn directly below it.
SpawnSetupForStage randomizes pair columns once for each new stage.
EnemySetupManager caches the generated EnemySpawnEntry positions.
RespawnActiveSetup reuses the cached positions for retry and Quick Setup.
Invalid pair data logs an error and does not spawn.
```

### Enemy AI

```txt
EnemyAIController
EnemyMoveSelector
PieceValueTable
EnemyActionDelay
```

Handles lightweight reactive enemy movement during Playing state.

The current controller also owns the lightweight player-first-move gate:

```txt
Start Battle
→ Wait for first successful Player move
→ Enable Enemy AI
```

Invalid movement attempts and elapsed time do not activate Enemy AI.

Current AI priority:

```txt
1. Capture Player King immediately
2. Move each Enemy Pawn once during opening if possible
3. Capture the highest-value player piece using non-King enemy pieces
4. Perform a random valid movement using non-King enemy pieces
5. Use Enemy King capture or random movement only as fallback
6. Wait if no valid movement exists
```

### Presentation

```txt
CanvasHud
PrototypeHud
```

`CanvasHud` provides the active Canvas-based Title, Preparation, Battle, and Result presentation. It reads existing systems and delegates commands through established public APIs without owning gameplay state.

`PrototypeHud` remains available as the IMGUI fallback. Canvas UI hides it while active and restores it if the Canvas UI is unavailable.

Prompt06 presentation additions:

```txt
Title screen
StageClear / GameOver result screen
Restart and Return to Title
First move notice
Global cooldown progress bar
Selected piece information panel
Enemy team composition panel
Battle / Enemy AI status panel
```

Prompt07 presentation additions:

```txt
Current stage label
Automatic StageClear transition display
Victory result screen
Restart Session and Return To Title stage reset
Spawn Random Enemies button removed from the Preparation workflow
Preparation buttons render before the expanded status block so Start Battle remains visible
```

Prompt08 presentation additions:

```txt
Setup Player MVP selects and places the stage-valid 3 / 5 / 7 cost loadout.
Preparation displays Loadout Cost using the current stage maximum.
```

Prompt09 presentation additions:

```txt
Canvas-based Title, Preparation, Battle, StageClear, GameOver, and Victory UI
King / Rook / Knight / Pawn Preparation selection buttons
Manual placement through existing placement systems
Stage, loadout cost, HP, placed count, enemy setup, enemy count, and enemy team composition
Global cooldown, Enemy AI status, selected piece, and First Move display
Bottom-center Preparation actions
Information graphics with raycast disabled
No player-facing enemy reroll or Preparation reset control
PrototypeHud fallback
```

## Prompt06 Modified Files

```txt
Assets/Scenes/SampleScene.unity
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Gameplay/EnemyAI/EnemyAIController.cs
Assets/Scripts/Gameplay/Interaction/BoardInputController.cs
Assets/Scripts/Gameplay/Preparation/ManualPlacementController.cs
Assets/Scripts/Gameplay/Preparation/PreparationManager.cs
Assets/Scripts/Gameplay/Stage/EnemySetupManager.cs
Assets/Scripts/Gameplay/Stage/EnemySpawnEntry.cs
Assets/Scripts/Presentation/PrototypeHud.cs
```

Prompt06 did not add new C# script files or scene root GameObjects.

## Prompt07 Modified Files

```txt
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Core/GameState.cs
Assets/Scripts/Gameplay/Stage/EnemySetupManager.cs
Assets/Scripts/Presentation/PrototypeHud.cs
Assets/Screenshots/prompt07_stage1_preparation.png
```

Prompt07 did not add new C# script files or scene root GameObjects.

Prompt07 post-fix:

```txt
Assets/Scripts/Presentation/PrototypeHud.cs
```

The post-fix only changed Preparation HUD rendering order.

## Prompt08 Modified Files

```txt
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Gameplay/Preparation/PreparationManager.cs
Assets/Scripts/Gameplay/Stage/EnemySetupManager.cs
Assets/Scripts/Presentation/PrototypeHud.cs
Assets/Data/EnemySetups/PatternA_KingRookPawn.asset
Assets/Data/EnemySetups/PatternB_KingKnightPawnPawn.asset
Assets/Data/EnemySetups/PatternC_KingRookKnight.asset
```

Prompt08 did not add new C# scripts or scene root GameObjects.

## Prompt09 Modified Files

```txt
Assets/Scenes/SampleScene.unity
Assets/Scripts/Presentation/CanvasHud.cs
Assets/Scripts/Presentation/PrototypeHud.cs
```

Prompt09 added:

```txt
Assets/Scripts/Presentation/CanvasHud.cs
CanvasUIRoot scene object
```

No gameplay system files were modified by Prompt09.

## Current Data Assets

### Stage

```txt
Assets/Data/MvpStageDefinition.asset
```

### Piece Definitions

```txt
Assets/Data/Pieces/KingDefinition.asset
Assets/Data/Pieces/RookDefinition.asset
Assets/Data/Pieces/KnightDefinition.asset
Assets/Data/Pieces/PawnDefinition.asset
```

Current MVP costs:

```txt
King = 0
Pawn = 1
Knight = 2
Rook = 3
Stage 1 Max Loadout Cost = 3
Stage 2 Max Loadout Cost = 5
Stage 3 Max Loadout Cost = 7
```

### Enemy Setups

```txt
PatternA_KingRookPawn
PatternB_KingKnightPawnPawn
PatternC_KingRookKnight
```

Enemy setup assets use serialized `x` and `y` fields to define canonical protected pairs. `EnemySpawnEntry.WithPosition(...)` creates runtime entries for randomized columns without modifying the assets.

Current protected structures:

```txt
PatternA_KingRookPawn:
King/Pawn + Rook/Pawn
4 enemies

PatternB_KingKnightPawnPawn:
King/Pawn + Knight/Pawn + Extra Pawn
5 enemies

PatternC_KingRookKnight:
King/Pawn + Rook/Pawn + Knight/Pawn
6 enemies
```

Pair columns are randomized only when entering a new stage. The generated layout is cached and reused unchanged during Player King retries and Quick Setup.

## Out of Current Scope

Not implemented:

```txt
Minimax
Chess engine
Check
Checkmate
Threat maps
Future turn prediction
Advanced tactical analysis
King safety evaluation
Castling
Pawn first double move
Promotion
En Passant
Shop
Reward economy
Campaign progression
Save / Load
Full input architecture
Drag-and-drop interaction
Touch controls
Keyboard navigation
Advanced UI
VFX
Sound
Animation polish
```
