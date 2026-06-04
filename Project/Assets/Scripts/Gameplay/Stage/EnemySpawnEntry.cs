using System;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;
using UnityEngine;
using UnityEngine.Serialization;

namespace TacticalRoguelike.Gameplay.Stage
{
    [Serializable]
    public sealed class EnemySpawnEntry
    {
        [FormerlySerializedAs("PieceDefinition")]
        [SerializeField]
        private PieceDefinition pieceDefinition;

        [SerializeField]
        private int x;

        [SerializeField]
        private int y;

        public PieceDefinition PieceDefinition
        {
            get { return pieceDefinition; }
        }

        public GridPosition Position
        {
            get { return new GridPosition(x, y); }
        }
    }
}
