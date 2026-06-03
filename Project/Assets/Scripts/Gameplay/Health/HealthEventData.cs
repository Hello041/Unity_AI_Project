namespace TacticalRoguelike.Gameplay.Health
{
    public sealed class HealthEventData
    {
        public HealthEventData(int currentHp, int maxHp, int damageAmount)
        {
            CurrentHp = currentHp;
            MaxHp = maxHp;
            DamageAmount = damageAmount;
        }

        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public int DamageAmount { get; private set; }
    }
}
