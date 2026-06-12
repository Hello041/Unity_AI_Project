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

        private readonly List<PieceController> spawnedEnemies = new List<PieceController>();
        private EnemySetupDefinition activeSetup;

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
            EnsureReferences();

            if (setup == null || boardManager == null || placementValidator == null)
            {
                Debug.LogWarning("Enemy setup requires setup data, BoardManager, and PlacementValidator.");
                return false;
            }

            ClearSpawnedEnemies();
            EnemySpawnEntry[] entries = setup.SpawnEntries;
            if (entries == null || entries.Length == 0)
            {
                Debug.LogWarning("Enemy setup has no spawn entries: " + setup.name);
                return false;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                EnemySpawnEntry entry = entries[i];
                if (!TrySpawnEnemy(entry))
                {
                    ClearSpawnedEnemies();
                    activeSetup = null;
                    return false;
                }
            }

            activeSetup = setup;
            Debug.Log("Enemy setup spawned: " + setup.PatternName);
            return true;
        }

public void ClearSpawnedEnemies()
        {
            ClearSpawnedEnemyInstances();
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
}
}
