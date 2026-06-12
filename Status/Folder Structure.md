# Folder Structure

Current Unity project root:

```txt
C:/Unity_AI_Project/Project
```

This document reflects the current implemented MVP state after Prompt05.

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
│  └─ slice09_hud_screen_check.png
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
GameOver
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

Handles simple weighted enemy pattern spawning.

### Enemy AI

```txt
EnemyAIController
EnemyMoveSelector
PieceValueTable
EnemyActionDelay
```

Handles lightweight reactive enemy movement during Playing state.

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
PrototypeHud
```

Handles prototype status display, setup buttons, enemy spawn button, battle start button, reset button, and result banner.

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
Max Loadout Cost = 7
```

### Enemy Setups

```txt
PatternA_KingRookPawn
PatternB_KingKnightPawnPawn
PatternC_KingRookKnight
```

Enemy spawn positions use serialized `x` and `y` fields in `EnemySpawnEntry`, then expose runtime `GridPosition` through a property.

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
