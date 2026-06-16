# Architecture Summary (Finalized MVP)

## 理쒖쥌 MVP 踰붿쐞

* Unity 6.3 LTS (6000.3.11f1) ???
* ??媛쒖쓽 ?뚮젅??媛?ν븳 Tactical Stage
* ?ㅼ떆媛?泥댁뒪???대룞
* 媛쒕퀎 荑⑤떎??+ ?뚮젅?댁뼱 湲濡쒕쾶 荑⑤떎??
* 寃쎈웾 Roguelike ?붿냼???ъ쟾 ?뺤쓽?????⑦꽩 ?쒕뜡 ?좏깮?쇰줈 ?쒗븳
* 3D ???蹂대뱶 + 2D 16x32 留??ㅽ봽?쇱씠???ъ슜

---

## ?듭떖 寃뚯엫 洹쒖튃

* ?뚮젅?댁뼱??King, Rook, Knight, Pawn ?ъ슜
* ?곷룄 King, Rook, Knight, Pawn ?ъ슜
* Enemy King 罹≪쿂 ??StageClear

### Player HP

* Player HP??3
* MVP?먯꽌????AI, ???? 泥댄겕, 泥댄겕硫붿씠???쒖뒪?쒖쓣 援ы쁽?섏? ?딅뒗??
* ???Player King ?대룞 吏곹썑 Enemy Capture ?먯젙???섑뻾?쒕떎.
* Player King??Enemy 留먯쓽 ?대룞 洹쒖튃 湲곗??쇰줈 罹≪쿂 媛?ν븳 ??쇱뿉 吏꾩엯??寃쎌슦, Enemy?먭쾶 罹≪쿂??寃껋쑝濡?媛꾩＜?쒕떎.

```txt
Player King Captured
??HP -1
```

* ?대뒗 ?⑥닚 ?꾪뿕 ????⑤꼸?곌? ?꾨땲??Enemy Capture ?대깽?몃줈 泥섎━?쒕떎.
* HP媛 0???섎㈃ GameOver

---

## GameState ?먮쫫

```txt
Boot
??StageStart
??Preparation
??Playing

Playing + EnemyKingCaptured
??StageClear

Playing + PlayerHealthDepleted
??GameOver
```

* GameOver??Terminal State
* HUD Reset ??Preparation ?곹깭濡?蹂듦? 媛??

---

## Board 洹쒖튃

* 湲곕낯 蹂대뱶 ?ш린: 6x6
* BoardManager??Board Size, 醫뚰몴 寃利? World Position 蹂?? Tile Occupancy???⑥씪 梨낆엫??
* 醫뚰몴 (0,0)? ?뚮젅?댁뼱 湲곗? 醫뚰븯??
* ?대룞 諛?諛곗튂 濡쒖쭅? 6x6???섎뱶肄붾뵫?섏? ?딄퀬 BoardManager 湲곗??쇰줈 泥섎━

---

## Loadout 洹쒖튃

```txt
Max Cost = 7

King = 0 (Required)
Pawn = 1
Knight = 2
Rook = 3
```

* King? ?꾩닔
* ?덉궛 珥덇낵 ???좏깮 諛?諛곗튂 遺덇?

---

## Placement 洹쒖튃

### ?뚮젅?댁뼱 諛곗튂 ?곸뿭

```txt
y = 0
y = 1
```

### ??諛곗튂 ?곸뿭

```txt
y = boardHeight - 2
y = boardHeight - 1
```

湲곕낯 6x6 湲곗?:

```txt
y = 4
y = 5
```

### 諛곗튂 ?쒖빟

* 以묐났 諛곗튂 遺덇?
* King ?놁씠 ?쒖옉 遺덇?
* ?덉궛 珥덇낵 ???쒖옉 遺덇?

### 諛곗튂 ?먮쫫

```txt
Piece Button Click
??Valid Tile Click
??Piece Placed
```

* ?먮룞 MVP 諛곗튂 踰꾪듉? ?꾨줈?좏????뚯뒪?몄슜?쇰줈 ?좎?

---

## Cooldown 洹쒖튃

### Piece Cooldown

* 紐⑤뱺 留먯? PieceCooldown 蹂댁쑀

### Global Cooldown

* ?뚮젅?댁뼱 留??대룞 ?깃났 ??PlayerGlobalCooldown ?쒖옉

### ?대룞 媛??議곌굔

```txt
PieceCooldown Ready
AND
GlobalCooldown Inactive
```

### ?대룞 ?깃났 ??

```txt
Start Piece Cooldown
+
Start Global Cooldown
```

### ?대룞 ?ㅽ뙣 ??

```txt
No Cooldown Applied
```

---

## Enemy Setup 洹쒖튃

### ??AI

* 援ы쁽?섏? ?딆쓬

### Enemy Patterns

Pattern A

```txt
King
Rook
Pawn
```

Pattern B

```txt
King
Knight
Pawn
Pawn
```

Pattern C

```txt
King
Rook
Knight
```

### ?앹꽦 洹쒖튃

* EnemySetupDefinition ScriptableObject ?ъ슜
* EnemySetupManager媛 ?⑦꽩 以??섎굹瑜??쒕뜡 ?좏깮
* ??諛곗튂 ?곸뿭???먮룞 諛곗튂
* ?꾪닾 ?쒖옉 ???곸씠 ?놁쑝硫?HUD媛 ?쒕뜡 ???앹꽦 蹂댁옣

---

## ?꾩닔 ?쒖뒪??

```txt
GameManager

BoardManager
BoardView

PieceDefinition
PieceController
PieceView

PieceMovementService

KingMovement
RookMovement
KnightMovement
PawnMovement

PieceActionController
BoardInputController

PieceCooldown
PlayerGlobalCooldown

PlayerHealthService

PreparationManager
PlacementValidator
ManualPlacementController

EnemySetupDefinition
EnemySetupManager

PrototypeHud
```

---

## ?쒖쇅???쒖뒪??

```txt
RoundManager

ShopManager
RewardManager

Full Augment System

Economy Loop

Campaign Progression

Enemy AI

Save / Load

Audio Pipeline

Presentation Framework

Pawn First Double Move

Promotion

En Passant

Check

Checkmate

Castling
```
---

# Final Implementation Status (Prompt10 Complete)

## Submission State

```txt
Feature Complete MVP / Submission Ready
```

Prompt01 through Prompt10 are complete and accepted.

## Final Polish Completed

* Full Korean localization for Canvas UI
* PrototypeHud fallback localization
* Selected piece display improvements
* Button highlight improvements
* Improved cooldown display readability
* Improved UI layout and readability
* Preparation selected-piece board highlight
* Softer placement area highlight with checkerboard visibility preserved
* Proper Preparation highlight cleanup
* Team-based individual cooldown tinting
* Player blue-gray cooldown tint
* Enemy red-gray cooldown tint
* Smooth cooldown color recovery
* Player-only ready flash
* Enemy AI priority update
* Capture scale-up and fade-out feedback
* Enhanced King capture feedback
* Delayed StageClear / Retry transition after King capture feedback
* Immediate gameplay capture resolution preserved

## Final Enemy AI Priority

```txt
1. Capture Player King immediately
2. Capture the highest-value available player piece
3. Opening Enemy Pawn movement if no capture is available
4. Non-King random movement
5. Enemy King random movement fallback
6. Wait if no valid action exists
```

## Final Validation

```txt
Stage 1 -> Stage 2 -> Stage 3 -> Victory: PASS
Retry: PASS
Game Over: PASS
Return To Title: PASS
AI validation complete: PASS
UI validation complete: PASS
Console Errors: 0
Console Warnings: 0
Scene Validation Issues: 0
```

## Future Work Policy

Future work should be limited to bug fixes, documentation corrections, or minor visual polish. Do not redesign completed systems or add new gameplay architecture without explicit post-MVP approval.

