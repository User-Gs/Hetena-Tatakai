using System;
using UnityEngine;

namespace HetenaTatakai
{
    [Serializable]
    public struct AttackDefinition
    {
        public string name;
        [Range(1, 6)] public int baseDieDamage;
        public float startup;
        public float activeTime;
        public float recovery;
        public float hitStun;
        public float knockback;
        [Range(0f, 1f)] public float guardDamageMultiplier;
        public bool heavy;

        public AttackDefinition(string name, int baseDieDamage, float startup, float activeTime, float recovery,
            float hitStun, float knockback, float guardDamageMultiplier, bool heavy = false)
        {
            this.name = name;
            this.baseDieDamage = Mathf.Clamp(baseDieDamage, 1, 6);
            this.startup = Mathf.Max(0f, startup);
            this.activeTime = Mathf.Max(0.02f, activeTime);
            this.recovery = Mathf.Max(0.02f, recovery);
            this.hitStun = Mathf.Max(0f, hitStun);
            this.knockback = Mathf.Max(0f, knockback);
            this.guardDamageMultiplier = Mathf.Clamp01(guardDamageMultiplier);
            this.heavy = heavy;
        }

        public float TotalDuration => startup + activeTime + recovery;
    }
}
