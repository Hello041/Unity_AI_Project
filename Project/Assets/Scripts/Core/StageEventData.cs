namespace TacticalRoguelike.Core
{
    public sealed class StageEventData
    {
        public StageEventData(GameState state, string stageId, string message)
        {
            State = state;
            StageId = stageId;
            Message = message;
        }

        public GameState State { get; private set; }
        public string StageId { get; private set; }
        public string Message { get; private set; }
    }
}
