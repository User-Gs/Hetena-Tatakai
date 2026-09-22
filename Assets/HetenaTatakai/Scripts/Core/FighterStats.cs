using UnityEngine;

namespace HetenaTatakai
{
    [CreateAssetMenu(menuName = "Hetena Tatakai/Fighter Stats", fileName = "FighterStats")]
    public sealed class FighterStats : ScriptableObject
    {
        public FighterId fighterId;
        public string displayName;

        [Header("Core Stats (1-4)")]
        [Range(1, 4)] public int energy = 2;
        [Range(1, 4)] public int rawPower = 2;
        [Range(1, 4)] public int speed = 2;
        [Range(1, 4)] public int technique = 2;
        [Range(1, 4)] public int morale = 2;
        [Range(1, 4)] public int durability = 2;

        [Header("Prototype Tuning")]
        [Min(1f)] public float maxHealth = 100f;
        [Min(0.1f)] public float moveSpeed = 4.5f;
        [Min(0.1f)] public float sidestepSpeed = 4.0f;
        [Min(1f)] public float rotationSpeed = 720f;
    }
}
