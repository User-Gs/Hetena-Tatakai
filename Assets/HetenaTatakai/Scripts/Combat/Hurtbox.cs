using UnityEngine;

namespace HetenaTatakai
{
    public sealed class Hurtbox : MonoBehaviour
    {
        [SerializeField] private FighterCombatController owner;

        public FighterCombatController Owner => owner;

        public void Configure(FighterCombatController fighter) => owner = fighter;

        public void ReceiveHit(CombatHit hit)
        {
            if (owner != null)
                owner.ReceiveHit(hit);
        }
    }
}
