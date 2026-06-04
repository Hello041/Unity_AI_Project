Prompt02 Implementation Summary

Prompt02 구현 검증 요약

구현 목표
첫 gameplay-flow vertical slice 구현.

Boot
→ StageStart
→ Preparation
→ Playing
→ StageClear / GameOver
보드 이동, 체스 말, 배치, 쿨다운, 적 AI는 Prompt02 검증 범위 아님.

구현 파일
Assets/Scripts/Core/GameState.cs
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Core/StageEventData.cs

Assets/Scripts/Gameplay/Health/PlayerHealthService.cs
Assets/Scripts/Gameplay/Health/HealthEventData.cs

Assets/Scripts/Data/StageDefinition.cs
Assets/Data/MvpStageDefinition.asset
추가 통합 수정:

Assets/Scripts/Presentation/PrototypeHud.cs
StartStage()가 자동으로 Preparation 진입하므로 HUD 중복 호출 제거.

구현된 기능
GameState
Boot
StageStart
Preparation
Playing
StageClear
GameOver
Result
금지 상태 없음:

Reward
Shop
RoundClear
Campaign
StageDefinition
읽기 전용 설정 데이터:

Stage Name
Board Width = 6
Board Height = 6
Player Max Health = 3
Max Loadout Cost = 7
Show Result After Clear
값 방어 처리:

Board Size 최소 1
Player Health 최소 1
Loadout Cost 최소 0
런타임 상태 저장 안 함.

PlayerHealthService
지원 API:

ResetForStage(int maxHealth)
TakeDamage(int amount = 1)
동작:

Stage 시작 시 HP 초기화
HP는 0 아래로 내려가지 않음
OnHealthChanged 발행
OnHealthDepleted는 HP 0 도달 시 1회만 발행
잘못된 HP 값은 최소 1로 보정
기존 후속 코드 호환 API도 유지:

Initialize(int hp)
ResetHealth()
ApplyDamage(int amount)
ApplyPlayerKingCapturedDamage()
GameManager
Inspector 참조:

PlayerHealthService
StageDefinition
동작:

Play 시작
→ Boot
→ StartStage
→ StageStart
→ HP 초기화
→ Preparation
Preparation + StartBattle
→ Playing
Playing + NotifyEnemyKingCaptured
→ StageClear
Playing + NotifyPlayerKingCaptured
→ HP -1
Playing + HP 0
→ GameOver
StageClear, GameOver, Playing 외 상태에서 gameplay 알림 무시.

실제 검증 결과
컴파일
Unity C# 컴파일 오류: 0
스크립트 validation 오류: 0
자동 시작
확인 로그:

GameState changed: StageStart
Stage started.
GameState changed: Preparation
Preparation started.
결과:

Play 시작 후 Preparation 진입 성공
StageDefinition 기준 HP 3 초기화 성공
GameOver 흐름
실행:

StartBattle()
NotifyPlayerKingCaptured()
NotifyPlayerKingCaptured()
NotifyPlayerKingCaptured()
NotifyEnemyKingCaptured()
결과:

stateAfterDamage = GameOver
finalState = GameOver
hp = 0
검증:

HP 고갈 시 GameOver 성공
GameOver 후 Enemy King 캡처 무시 성공
StageClear 흐름
실행:

StartBattle()
NotifyEnemyKingCaptured()
NotifyPlayerKingCaptured()
결과:

clearState = StageClear
finalState = StageClear
hp = 3
검증:

Enemy King 캡처 시 StageClear 성공
StageClear 후 Player King 캡처 무시 성공
Prompt02 수락 기준 상태
1. Play 시작 시 Stage 자동 시작             완료
2. StartStage HP 초기화 + Preparation 진입  완료
3. StartBattle → Playing                    완료
4. Enemy King 캡처 → StageClear             완료
5. HP 0 → GameOver                          완료
6. Playing 외 알림 무시                     완료
7. GameOver 후 알림 무시                    완료
8. StageDefinition 설정 데이터 전용         완료
9. 금지 시스템 생성 안 함                   완료
범위 밖 / 미검증
프로젝트에 후속 시스템 코드 존재하지만 Prompt02 기준 검증 안 됨:

BoardManager
BoardView
PieceController
Piece Movement
Loadout
Placement
Cooldown
Enemy Setup
Prototype HUD 전체 흐름
발견된 기존 통합 문제:

BoardInputController가 UnityEngine.Input 사용
프로젝트는 Input System 전용 설정
Play 중 InvalidOperationException 발생 가능
Prompt02 범위 밖이므로 수정 안 함.

결론
Prompt02 vertical slice: 완료
Prompt02 acceptance criteria: 충족
다음 vertical slice: 새 프롬프트 필요
