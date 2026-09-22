using UnityEngine;

namespace HetenaTatakai
{
    public readonly struct D6Result
    {
        public readonly int NaturalRoll;
        public readonly int FinalDamage;
        public readonly bool Critical;

        public D6Result(int naturalRoll, int finalDamage)
        {
            NaturalRoll = naturalRoll;
            FinalDamage = Mathf.Clamp(finalDamage, 1, 6);
            Critical = naturalRoll == 6;
        }
    }

    public static class D6CombatResolver
    {
        public static D6Result Roll(FighterStats stats, AttackDefinition attack, bool cpuControlled, GameDifficulty difficulty)
        {
            int natural = RollNatural(cpuControlled, difficulty);

            int powerBonus = stats != null && stats.rawPower >= 4 ? 1 : 0;
            int techniqueFloor = stats != null && stats.technique >= 4 ? 2 : 1;
            int attackBias = attack.heavy && natural <= 2 ? 1 : 0;
            int result = Mathf.Clamp(Mathf.Max(natural + powerBonus + attackBias, techniqueFloor), 1, 6);
            return new D6Result(natural, result);
        }

        private static int RollNatural(bool cpuControlled, GameDifficulty difficulty)
        {
            if (!cpuControlled)
                return Random.Range(1, 7);

            if (difficulty == GameDifficulty.Easy)
                return Random.value < 0.10f ? 6 : Random.Range(1, 6);

            int roll = Random.Range(1, 7);
            if (roll <= 3 && Random.value < 0.15f)
                roll = Random.Range(4, 7);

            return roll;
        }
    }
}
