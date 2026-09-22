using UnityEngine;

namespace HetenaTatakai
{
    public readonly struct CombatHit
    {
        public readonly GameObject Attacker;
        public readonly float Damage;
        public readonly float HitStun;
        public readonly float Knockback;
        public readonly bool Critical;
        public readonly Vector3 Direction;

        public CombatHit(GameObject attacker, float damage, float hitStun, float knockback, bool critical, Vector3 direction)
        {
            Attacker = attacker;
            Damage = damage;
            HitStun = hitStun;
            Knockback = knockback;
            Critical = critical;
            Direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
        }
    }
}
