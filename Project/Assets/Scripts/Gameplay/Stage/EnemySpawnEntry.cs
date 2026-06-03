using System;
using TacticalRoguelike.Gameplay.Board;
using TacticalRoguelike.Gameplay.Pieces;

namespace TacticalRoguelike.Gameplay.Stage
{
    [Serializable]
    public sealed class EnemySpawnEntry
    {
        public PieceDefinition PieceDefinition;
        public GridPosition Position;
    }
}
