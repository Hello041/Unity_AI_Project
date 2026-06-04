Assets/
├─ Scenes/
│  └─ SampleScene.unity
│
├─ Data/
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
│  ├─ Gameplay/
│  │  ├─ Board/
│  │  │  ├─ BoardManager.cs
│  │  │  ├─ BoardView.cs
│  │  │  ├─ BoardTileView.cs
│  │  │  ├─ BoardHighlightType.cs
│  │  │  └─ GridPosition.cs
│  │  │
│  │  ├─ Pieces/
│  │  │  ├─ PieceController.cs
│  │  │  ├─ PieceView.cs
│  │  │  ├─ PieceDefinition.cs
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
│  │  │  ├─ BoardInputController.cs
│  │  │  └─ PieceActionController.cs
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
│  │  └─ Stage/
│  │     ├─ EnemySetupDefinition.cs
│  │     ├─ EnemySetupManager.cs
│  │     └─ EnemySpawnEntry.cs
│  │
│  └─ Presentation/
│     └─ PrototypeHud.cs
│
├─ Sprite/
│  ├─ 16x32 pieces/
│  │  ├─ W_King.png
│  │  ├─ W_Rook.png
│  │  ├─ W_Knight.png
│  │  ├─ W_Pawn.png
│  │  ├─ B_King.png
│  │  ├─ B_Rook.png
│  │  ├─ B_Knight.png
│  │  └─ B_Pawn.png
│  │
│  └─ boards/
│     └─ 6x6.png
│
└─ Screenshots/
   ├─ slice02_board_centered.png
   ├─ slice03_pieces_check.png
   ├─ slice03_pieces_height_raised.png
   └─ slice09_hud_check.png