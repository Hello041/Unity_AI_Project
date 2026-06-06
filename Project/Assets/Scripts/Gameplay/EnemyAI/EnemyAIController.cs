using TacticalRoguelike.Core;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Interaction;
using TacticalRoguelike.Gameplay.Movement;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.EnemyAI
{
    public sealed class EnemyAIController : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private PieceMovementService movementService;

        [SerializeField]
        private PieceActionController pieceActionController;

        [SerializeField]
        private EnemyActionDelay actionDelay = new EnemyActionDelay();

        [SerializeField]
        private bool logActions = true;

        private readonly EnemyMoveSelector moveSelector = new EnemyMoveSelector();
        private bool wasPlaying;

        private void Awake()
        {
            EnsureReferences();
        }

private void Update()
        {
            EnsureReferences();

            bool isPlaying = gameManager != null && gameManager.CurrentState == GameState.Playing;
            if (!isPlaying)
            {
                if (wasPlaying)
                {
                    if (actionDelay != null)
                    {
                        actionDelay.Reset();
                    }

                    moveSelector.ResetOpeningMemory();
                }

                wasPlaying = false;
                return;
            }

            if (!wasPlaying)
            {
                moveSelector.ResetOpeningMemory();
                if (actionDelay != null)
                {
                    actionDelay.TriggerSoon();
                }
            }

            wasPlaying = true;

            if (actionDelay == null || !actionDelay.Tick(Time.deltaTime))
            {
                return;
            }

            TryPerformEnemyAction();
        }

[ContextMenu("Debug/Perform Enemy Action")]
        public void TryPerformEnemyAction()
        {
            EnsureReferences();

            if (gameManager == null || gameManager.CurrentState != GameState.Playing)
            {
                return;
            }

            if (boardManager == null || movementService == null || pieceActionController == null)
            {
                Debug.LogError("EnemyAIController requires GameManager, BoardManager, PieceMovementService, and PieceActionController.");
                return;
            }

            EnemyMoveSelector.EnemyMove selectedMove = moveSelector.SelectMove(boardManager, movementService);
            if (!selectedMove.IsValid)
            {
                Log("Enemy AI waits. No valid enemy move exists.");
                return;
            }

            LogSelectedMove(selectedMove);
            bool moved = pieceActionController.TryMoveSelectedPiece(selectedMove.Piece, selectedMove.TargetPosition);
            if (moved)
            {
                moveSelector.MarkMoveCompleted(selectedMove);
                return;
            }

            LogFormat("Enemy AI selected move failed at execution: {0}", selectedMove.TargetPosition);
        }

private void LogSelectedMove(EnemyMoveSelector.EnemyMove selectedMove)
        {
            if (!logActions)
            {
                return;
            }

            string pieceName = selectedMove.Piece != null ? selectedMove.Piece.name : "Unknown";
            if (selectedMove.IsCapture && selectedMove.TargetPiece != null)
            {
                LogFormat("Enemy AI capture: {0} -> {1} at {2}", pieceName, selectedMove.TargetPiece.name, selectedMove.TargetPosition);
            }
            else if (selectedMove.MoveKind == EnemyMoveSelector.EnemyMoveKind.OpeningPawnMove)
            {
                LogFormat("Enemy AI opening pawn move: {0} -> {1}", pieceName, selectedMove.TargetPosition);
            }
            else
            {
                LogFormat("Enemy AI random move: {0} -> {1}", pieceName, selectedMove.TargetPosition);
            }
        }

        private void Log(string message)
        {
            if (logActions)
            {
                Debug.Log(message);
            }
        }

private void LogFormat(string format, params object[] args)
        {
            if (logActions)
            {
                Debug.LogFormat(format, args);
            }
        }


        private void EnsureReferences()
        {
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }

            if (movementService == null)
            {
                movementService = FindFirstObjectByType<PieceMovementService>();
            }

            if (pieceActionController == null)
            {
                pieceActionController = FindFirstObjectByType<PieceActionController>();
            }
        }
    }
}
