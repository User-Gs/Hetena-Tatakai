using UnityEngine;

namespace HetenaTatakai
{
    public static class FatigueModel
    {
        public static int AttackThreshold(int energy) => Mathf.Clamp(energy, 1, 4) * 2;

        public static float RecoveryMultiplier(int energy, int attacksPerformed)
        {
            if (attacksPerformed <= AttackThreshold(energy)) return 1f;
            return Mathf.Lerp(1.35f, 1.1f, (Mathf.Clamp(energy, 1, 4) - 1) / 3f);
        }
    }
}
