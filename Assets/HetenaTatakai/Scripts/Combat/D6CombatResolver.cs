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
        public static D6Result Roll(FighterStats stats, AttackDefinition attack)
        {
            int natural = Random.Range(1, 7);

            // Stats influence the die but damage always remains inside the canonical 1-6 band.
            int powerBonus = stats != null && stats.rawPower >= 4 ? 1 : 0;
            int techniqueFloor = stats != null && stats.technique >= 4 ? 2 : 1;
            int attackBias = attack.heavy && natural <= 2 ? 1 : 0;
            int result = Mathf.Clamp(Mathf.Max(natural + powerBonus + attackBias, techniqueFloor), 1, 6);
            return new D6Result(natural, result);
        }
    }
}
