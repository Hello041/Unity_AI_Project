using System.Collections.Generic;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;
using TacticalRoguelike.Gameplay.Preparation;
using UnityEngine;

namespace TacticalRoguelike.Gameplay.Stage
{
    public sealed class EnemySetupManager : MonoBehaviour
    {
        [SerializeField]
        private EnemySetupDefinition[] enemySetups;

        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private PlacementValidator placementValidator;

        [SerializeField]
        private Transform piecesRoot;


        private readonly List<EnemySpawnEntry> activeSpawnEntries = new List<EnemySpawnEntry>();
        private readonly List<PieceController> spawnedEnemies = new List<PieceController>();


        private static readonly string[] StagePatternNames =
        {
            "PatternA_KingRookPawn",
            "PatternB_KingKnightPawnPawn",
            "PatternC_KingRookKnight"
        };
        private EnemySetupDefinition activeSetup;



        public IReadOnlyList<EnemySpawnEntry> ActiveSpawnEntries
        {
            get { return activeSpawnEntries; }
        }
        public EnemySetupDefinition ActiveSetup
        {
            get { return activeSetup; }
        }

        public int SpawnedEnemyCount
        {
            get
            {
                int activeCount = 0;
                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    PieceController enemy = spawnedEnemies[i];
                    if (enemy != null && !enemy.IsCaptured)
                    {
                        activeCount++;
                    }
                }

                return activeCount;
            }
        }

        private void Awake()
        {
            EnsureReferences();
        }

        public bool SpawnRandomSetup()
        {
            EnemySetupDefinition setup = ChooseRandomSetup();
            if (setup == null)
            {
                Debug.LogWarning("No enemy setup is available.");
                return false;
            }

            return SpawnSetup(setup);
        }

        public bool SpawnSetup(EnemySetupDefinition setup)
        {
            return CacheAndSpawn(setup, setup != null ? setup.SpawnEntries : null);
        }

        public void ClearSpawnedEnemies()
        {
            ClearSpawnedEnemyInstances();
            activeSpawnEntries.Clear();
            activeSetup = null;
        }

        private bool TrySpawnEnemy(EnemySpawnEntry entry)
        {
            if (entry == null || entry.PieceDefinition == null)
            {
                Debug.LogWarning("Enemy spawn entry is missing a piece definition.");
                return false;
            }

            string reason;
            if (!placementValidator.CanPlace(PieceOwner.Enemy, entry.Position, out reason))
            {
                Debug.LogWarning("Enemy placement failed: " + reason + " Position: " + entry.Position);
                return false;
            }

            GameObject enemyObject = new GameObject("Enemy_" + entry.PieceDefinition.DisplayName);
            enemyObject.transform.SetParent(piecesRoot, false);

            PieceController enemyPiece = enemyObject.AddComponent<PieceController>();
            if (!enemyPiece.Initialize(entry.PieceDefinition, PieceOwner.Enemy, entry.Position, boardManager))
            {
                DestroyImmediate(enemyObject);
                return false;
            }

            spawnedEnemies.Add(enemyPiece);
            return true;
        }

        private EnemySetupDefinition ChooseRandomSetup()
        {
            if (enemySetups == null || enemySetups.Length == 0)
            {
                return null;
            }

            int totalWeight = 0;
            for (int i = 0; i < enemySetups.Length; i++)
            {
                if (enemySetups[i] != null)
                {
                    totalWeight += enemySetups[i].RandomWeight;
                }
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = Random.Range(0, totalWeight);
            for (int i = 0; i < enemySetups.Length; i++)
            {
                EnemySetupDefinition setup = enemySetups[i];
                if (setup == null)
                {
                    continue;
                }

                roll -= setup.RandomWeight;
                if (roll < 0)
                {
                    return setup;
                }
            }

            return enemySetups[0];
        }

        private void EnsureReferences()
        {
            if (boardManager == null)
            {
                boardManager = FindFirstObjectByType<BoardManager>();
            }

            if (placementValidator == null)
            {
                placementValidator = FindFirstObjectByType<PlacementValidator>();
            }

            if (piecesRoot == null)
            {
                GameObject rootObject = GameObject.Find("PiecesRoot");
                if (rootObject == null)
                {
                    rootObject = new GameObject("PiecesRoot");
                }

                piecesRoot = rootObject.transform;
            }
        }
    

private void ClearSpawnedEnemyInstances()
        {
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                PieceController enemy = spawnedEnemies[i];
                if (enemy != null)
                {
                    DestroyImmediate(enemy.gameObject);
                }
            }

            spawnedEnemies.Clear();
        }


public void ClearSpawnedEnemiesForRetry()
        {
            ClearSpawnedEnemyInstances();
        }


public EnemySetupDefinition GetSetupForStage(int stageNumber)
        {
            int patternIndex = stageNumber - 1;
            if (patternIndex < 0 || patternIndex >= StagePatternNames.Length || enemySetups == null)
            {
                return null;
            }

            string requiredPatternName = StagePatternNames[patternIndex];
            for (int i = 0; i < enemySetups.Length; i++)
            {
                EnemySetupDefinition setup = enemySetups[i];
                if (setup != null && (setup.PatternName == requiredPatternName || setup.name == requiredPatternName))
                {
                    return setup;
                }
            }

            return null;
        }


        public bool SpawnSetupForStage(int stageNumber)
        {
            EnemySetupDefinition setup = GetSetupForStage(stageNumber);
            if (setup == null)
            {
                Debug.LogWarning("No enemy setup is configured for Stage " + stageNumber + ".");
                return false;
            }

            EnemySpawnEntry[] generatedEntries = GenerateRandomizedPairEntries(setup);
            return generatedEntries != null && CacheAndSpawn(setup, generatedEntries);
        }


        private bool ValidateProtectedPairs(EnemySetupDefinition setup, IReadOnlyList<EnemySpawnEntry> entries)
        {
            int kingCount = 0;

            for (int i = 0; i < entries.Count; i++)
            {
                EnemySpawnEntry protectedEntry = entries[i];
                if (protectedEntry == null || protectedEntry.PieceDefinition == null)
                {
                    Debug.LogError("Invalid enemy setup " + setup.name + ": spawn entry is missing a piece definition.");
                    return false;
                }

                PieceType pieceType = protectedEntry.PieceDefinition.PieceType;
                if (pieceType == PieceType.Pawn)
                {
                    continue;
                }

                if (pieceType == PieceType.King)
                {
                    kingCount++;
                }

                GridPosition pawnPosition = new GridPosition(protectedEntry.Position.X, protectedEntry.Position.Y - 1);
                bool hasProtectingPawn = false;
                for (int j = 0; j < entries.Count; j++)
                {
                    EnemySpawnEntry candidate = entries[j];
                    if (candidate != null && candidate.PieceDefinition != null
                        && candidate.PieceDefinition.PieceType == PieceType.Pawn
                        && candidate.Position == pawnPosition)
                    {
                        hasProtectingPawn = true;
                        break;
                    }
                }

                if (!hasProtectingPawn)
                {
                    Debug.LogError("Invalid enemy setup " + setup.name + ": " + pieceType
                        + " is not protected by a Pawn at " + pawnPosition + ".");
                    return false;
                }
            }

            if (kingCount != 1)
            {
                Debug.LogError("Invalid enemy setup " + setup.name + ": expected exactly one Enemy King.");
                return false;
            }

            return true;
        }


        private EnemySpawnEntry[] GenerateRandomizedPairEntries(EnemySetupDefinition setup)
        {
            EnsureReferences();
            EnemySpawnEntry[] sourceEntries = setup != null ? setup.SpawnEntries : null;
            if (sourceEntries == null || sourceEntries.Length == 0 || !ValidateProtectedPairs(setup, sourceEntries))
            {
                return null;
            }

            List<EnemySpawnEntry> protectedEntries = new List<EnemySpawnEntry>();
            Dictionary<EnemySpawnEntry, EnemySpawnEntry> protectingPawns = new Dictionary<EnemySpawnEntry, EnemySpawnEntry>();
            HashSet<EnemySpawnEntry> pairedPawns = new HashSet<EnemySpawnEntry>();

            for (int i = 0; i < sourceEntries.Length; i++)
            {
                EnemySpawnEntry entry = sourceEntries[i];
                if (entry.PieceDefinition.PieceType == PieceType.Pawn)
                {
                    continue;
                }

                EnemySpawnEntry protectingPawn = null;
                GridPosition expectedPawnPosition = new GridPosition(entry.Position.X, entry.Position.Y - 1);
                for (int j = 0; j < sourceEntries.Length; j++)
                {
                    EnemySpawnEntry candidate = sourceEntries[j];
                    if (candidate.PieceDefinition.PieceType == PieceType.Pawn
                        && candidate.Position == expectedPawnPosition)
                    {
                        protectingPawn = candidate;
                        break;
                    }
                }

                protectedEntries.Add(entry);
                protectingPawns.Add(entry, protectingPawn);
                pairedPawns.Add(protectingPawn);
            }

            List<int> columns = new List<int>();
            for (int column = 0; column < boardManager.BoardWidth; column++)
            {
                columns.Add(column);
            }

            for (int i = columns.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Range(0, i + 1);
                int value = columns[i];
                columns[i] = columns[swapIndex];
                columns[swapIndex] = value;
            }

            int extraPawnCount = 0;
            for (int i = 0; i < sourceEntries.Length; i++)
            {
                if (sourceEntries[i].PieceDefinition.PieceType == PieceType.Pawn
                    && !pairedPawns.Contains(sourceEntries[i]))
                {
                    extraPawnCount++;
                }
            }

            if (protectedEntries.Count + extraPawnCount > columns.Count)
            {
                Debug.LogError("Invalid enemy setup " + setup.name + ": protected pairs exceed available board columns.");
                return null;
            }

            int topRow = boardManager.BoardHeight - 1;
            int supportRow = boardManager.BoardHeight - 2;
            Dictionary<EnemySpawnEntry, GridPosition> generatedPositions = new Dictionary<EnemySpawnEntry, GridPosition>();
            int columnIndex = 0;

            for (int i = 0; i < protectedEntries.Count; i++)
            {
                int column = columns[columnIndex++];
                EnemySpawnEntry protectedEntry = protectedEntries[i];
                generatedPositions.Add(protectedEntry, new GridPosition(column, topRow));
                generatedPositions.Add(protectingPawns[protectedEntry], new GridPosition(column, supportRow));
            }

            for (int i = 0; i < sourceEntries.Length; i++)
            {
                EnemySpawnEntry entry = sourceEntries[i];
                if (!generatedPositions.ContainsKey(entry))
                {
                    generatedPositions.Add(entry, new GridPosition(columns[columnIndex++], supportRow));
                }
            }

            EnemySpawnEntry[] generatedEntries = new EnemySpawnEntry[sourceEntries.Length];
            for (int i = 0; i < sourceEntries.Length; i++)
            {
                generatedEntries[i] = sourceEntries[i].WithPosition(generatedPositions[sourceEntries[i]]);
            }

            return generatedEntries;
        }


        private bool CacheAndSpawn(EnemySetupDefinition setup, IReadOnlyList<EnemySpawnEntry> entries)
        {
            EnsureReferences();
            if (setup == null || boardManager == null || placementValidator == null
                || entries == null || entries.Count == 0)
            {
                Debug.LogError("Enemy setup requires setup data, spawn entries, BoardManager, and PlacementValidator.");
                return false;
            }

            if (!ValidateProtectedPairs(setup, entries))
            {
                return false;
            }

            ClearSpawnedEnemyInstances();
            for (int i = 0; i < entries.Count; i++)
            {
                if (!TrySpawnEnemy(entries[i]))
                {
                    ClearSpawnedEnemyInstances();
                    activeSpawnEntries.Clear();
                    activeSetup = null;
                    return false;
                }
            }

            activeSpawnEntries.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                activeSpawnEntries.Add(entries[i]);
            }

            activeSetup = setup;
            Debug.Log("Enemy setup spawned: " + setup.PatternName);
            return true;
        }


        public bool RespawnActiveSetup()
        {
            EnsureReferences();
            if (activeSetup == null || activeSpawnEntries.Count == 0)
            {
                Debug.LogError("Cannot respawn enemy setup because no generated encounter is cached.");
                return false;
            }

            if (!ValidateProtectedPairs(activeSetup, activeSpawnEntries))
            {
                return false;
            }

            ClearSpawnedEnemyInstances();
            for (int i = 0; i < activeSpawnEntries.Count; i++)
            {
                if (!TrySpawnEnemy(activeSpawnEntries[i]))
                {
                    ClearSpawnedEnemyInstances();
                    return false;
                }
            }

            Debug.Log("Enemy setup respawned without reroll: " + activeSetup.PatternName);
            return true;
        }
}
}
