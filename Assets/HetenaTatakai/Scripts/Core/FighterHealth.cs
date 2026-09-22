using System;
using UnityEngine;

namespace HetenaTatakai
{
    public sealed class FighterHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float Normalized => maxHealth <= 0f ? 0f : currentHealth / maxHealth;
        public bool IsKO { get; private set; }

        public event Action<float, float> HealthChanged;
        public event Action<FighterHealth> KnockedOut;

        public void Configure(float value)
        {
            maxHealth = Mathf.Max(1f, value);
            currentHealth = maxHealth;
            IsKO = false;
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public float TakeDamage(float damage)
        {
            if (IsKO || damage <= 0f) return 0f;

            float before = currentHealth;
            currentHealth = Mathf.Max(0f, currentHealth - damage);
            float applied = before - currentHealth;
            HealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0f)
            {
                IsKO = true;
                KnockedOut?.Invoke(this);
            }

            return applied;
        }
    }
}
