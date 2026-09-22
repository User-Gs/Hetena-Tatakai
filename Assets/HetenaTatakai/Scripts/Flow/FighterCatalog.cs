using System.Collections.Generic;
using UnityEngine;

namespace HetenaTatakai
{
    public sealed class FighterProfile
    {
        public FighterId Id { get; }
        public string DisplayName { get; }
        public int SkinCount { get; }

        public FighterProfile(FighterId id, string displayName, int skinCount = 3)
        {
            Id = id;
            DisplayName = displayName;
            SkinCount = Mathf.Max(1, skinCount);
        }

        // Prototype-only neutral tuning. Canonical per-character stats can replace these
        // without changing the menu/combat flow.
        public FighterStats CreateRuntimeStats()
        {
            FighterStats stats = ScriptableObject.CreateInstance<FighterStats>();
            stats.fighterId = Id;
            stats.displayName = DisplayName;
            stats.energy = 3;
            stats.rawPower = 3;
            stats.speed = 3;
            stats.technique = 3;
            stats.morale = 3;
            stats.durability = 3;
            stats.maxHealth = 100f;
            stats.moveSpeed = 4.5f;
            stats.sidestepSpeed = 4.0f;
            stats.rotationSpeed = 720f;

            if (Id == FighterId.Ayaka)
            {
                stats.energy = 3;
                stats.rawPower = 2;
                stats.speed = 4;
                stats.technique = 4;
                stats.morale = 3;
                stats.durability = 3;
            }
            else if (Id == FighterId.Morana)
            {
                stats.energy = 4;
                stats.rawPower = 2;
                stats.speed = 3;
                stats.technique = 2;
            }

            return stats;
        }
    }

    public static class FighterCatalog
    {
        private static readonly FighterProfile[] profiles =
        {
            new FighterProfile(FighterId.Anna, "Anna"),
            new FighterProfile(FighterId.Leyla, "Leyla"),
            new FighterProfile(FighterId.Katarina, "Katarina"),
            new FighterProfile(FighterId.Jessica, "Jessica"),
            new FighterProfile(FighterId.Ayaka, "Ayaka"),
            new FighterProfile(FighterId.Poly, "Poly"),
            new FighterProfile(FighterId.Eleni, "Eleni"),
            new FighterProfile(FighterId.Helene, "Hélène"),
            new FighterProfile(FighterId.Mei, "Mei \"Kowloon\""),
            new FighterProfile(FighterId.Dao, "Dao"),
            new FighterProfile(FighterId.Nefarati, "Nefarati"),
            new FighterProfile(FighterId.Morana, "Morana")
        };

        public static IReadOnlyList<FighterProfile> All => profiles;

        public static FighterProfile Get(FighterId id)
        {
            for (int i = 0; i < profiles.Length; i++)
                if (profiles[i].Id == id) return profiles[i];

            return profiles[0];
        }
    }
}
