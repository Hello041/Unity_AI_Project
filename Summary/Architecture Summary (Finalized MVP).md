# Architecture Summary (Finalized MVP)

## 최종 MVP 범위

* Unity 6.3 LTS (6000.3.11f1) 대상
* 한 개의 플레이 가능한 Tactical Stage
* 실시간 체스식 이동
* 개별 쿨다운 + 플레이어 글로벌 쿨다운
* 경량 Roguelike 요소는 사전 정의된 적 패턴 랜덤 선택으로 제한
* 3D 타일 보드 + 2D 16x32 말 스프라이트 사용

---

## 핵심 게임 규칙

* 플레이어는 King, Rook, Knight, Pawn 사용
* 적도 King, Rook, Knight, Pawn 사용
* Enemy King 캡처 시 StageClear

### Player HP

* Player HP는 3
* MVP에서는 적 AI, 적 턴, 체크, 체크메이트 시스템을 구현하지 않는다.
* 대신 Player King 이동 직후 Enemy Capture 판정을 수행한다.
* Player King이 Enemy 말의 이동 규칙 기준으로 캡처 가능한 타일에 진입한 경우, Enemy에게 캡처된 것으로 간주한다.

```txt
Player King Captured
→ HP -1
```

* 이는 단순 위험 타일 패널티가 아니라 Enemy Capture 이벤트로 처리한다.
* HP가 0이 되면 GameOver

---

## GameState 흐름

```txt
Boot
→ StageStart
→ Preparation
→ Playing

Playing + EnemyKingCaptured
→ StageClear

Playing + PlayerHealthDepleted
→ GameOver
```

* GameOver는 Terminal State
* HUD Reset 시 Preparation 상태로 복귀 가능

---

## Board 규칙

* 기본 보드 크기: 6x6
* BoardManager는 Board Size, 좌표 검증, World Position 변환, Tile Occupancy의 단일 책임자
* 좌표 (0,0)은 플레이어 기준 좌하단
* 이동 및 배치 로직은 6x6을 하드코딩하지 않고 BoardManager 기준으로 처리

---

## Loadout 규칙

```txt
Max Cost = 7

King = 0 (Required)
Pawn = 1
Knight = 2
Rook = 3
```

* King은 필수
* 예산 초과 시 선택 및 배치 불가

---

## Placement 규칙

### 플레이어 배치 영역

```txt
y = 0
y = 1
```

### 적 배치 영역

```txt
y = boardHeight - 2
y = boardHeight - 1
```

기본 6x6 기준:

```txt
y = 4
y = 5
```

### 배치 제약

* 중복 배치 불가
* King 없이 시작 불가
* 예산 초과 시 시작 불가

### 배치 흐름

```txt
Piece Button Click
→ Valid Tile Click
→ Piece Placed
```

* 자동 MVP 배치 버튼은 프로토타입 테스트용으로 유지

---

## Cooldown 규칙

### Piece Cooldown

* 모든 말은 PieceCooldown 보유

### Global Cooldown

* 플레이어 말 이동 성공 시 PlayerGlobalCooldown 시작

### 이동 가능 조건

```txt
PieceCooldown Ready
AND
GlobalCooldown Inactive
```

### 이동 성공 시

```txt
Start Piece Cooldown
+
Start Global Cooldown
```

### 이동 실패 시

```txt
No Cooldown Applied
```

---

## Enemy Setup 규칙

### 적 AI

* 구현하지 않음

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

### 생성 규칙

* EnemySetupDefinition ScriptableObject 사용
* EnemySetupManager가 패턴 중 하나를 랜덤 선택
* 적 배치 영역에 자동 배치
* 전투 시작 시 적이 없으면 HUD가 랜덤 적 생성 보장

---

## 필수 시스템

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

## 제외된 시스템

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
