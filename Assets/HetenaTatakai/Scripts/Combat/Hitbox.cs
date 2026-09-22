using System.Collections.Generic;
using UnityEngine;

namespace HetenaTatakai
{
    [RequireComponent(typeof(Collider))]
    public sealed class Hitbox : MonoBehaviour
    {
        [SerializeField] private FighterCombatController owner;
        private readonly HashSet<Hurtbox> hitThisActivation = new HashSet<Hurtbox>();
        private Collider triggerCollider;
        private CombatHit currentHit;
        private bool armed;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
            triggerCollider.enabled = false;
        }

        public void Configure(FighterCombatController fighter) => owner = fighter;

        public void Arm(CombatHit hit)
        {
            currentHit = hit;
            hitThisActivation.Clear();
            armed = true;
            triggerCollider.enabled = true;
        }

        public void Disarm()
        {
            armed = false;
            if (triggerCollider != null) triggerCollider.enabled = false;
            hitThisActivation.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!armed) return;

            Hurtbox hurtbox = other.GetComponent<Hurtbox>() ?? other.GetComponentInParent<Hurtbox>();
            if (hurtbox == null || hurtbox.Owner == null || hurtbox.Owner == owner || hitThisActivation.Contains(hurtbox)) return;

            hitThisActivation.Add(hurtbox);
            hurtbox.ReceiveHit(currentHit);
            owner?.NotifyAttackConnected();
        }
    }
}
