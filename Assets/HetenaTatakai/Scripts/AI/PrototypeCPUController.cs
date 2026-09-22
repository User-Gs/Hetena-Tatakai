using UnityEngine;

namespace HetenaTatakai
{
    public sealed class PrototypeCPUController : MonoBehaviour
    {
        [SerializeField] private FighterMotor3D motor;
        [SerializeField] private FighterCombatController combat;
        [SerializeField] private Transform opponent;
        [SerializeField] private GameDifficulty difficulty = GameDifficulty.Easy;

        private float nextDecisionTime;
        private float guardUntil;

        public void Configure(FighterMotor3D fighterMotor, FighterCombatController fighterCombat, Transform target)
        {
            motor = fighterMotor;
            combat = fighterCombat;
            opponent = target;
            motor.SetAIControlled(true);
            combat.SetAIControlled(true);
        }

        public void SetDifficulty(GameDifficulty value)
        {
            difficulty = value;
            combat.SetDifficulty(value);
        }

        private void OnDisable()
        {
            if (motor != null) motor.SetAIMovement(Vector2.zero);
            if (combat != null) combat.SetGuardRequested(false);
        }

        private void Update()
        {
            if (motor == null || combat == null || opponent == null || combat.Health.IsKO) return;

            float distance = Vector3.Distance(transform.position, opponent.position);
            float forward = distance > 1.65f ? 1f : distance < 1.05f ? -0.7f : 0f;
            float side = Mathf.Sin(Time.time * (difficulty == GameDifficulty.Hard ? 1.9f : 1.15f)) * 0.45f;
            motor.SetAIMovement(new Vector2(side, forward));

            if (Time.time < guardUntil)
            {
                combat.SetGuardRequested(true);
                return;
            }

            combat.SetGuardRequested(false);
            if (Time.time < nextDecisionTime || distance > 2.15f) return;

            float interval = difficulty == GameDifficulty.Hard ? Random.Range(0.28f, 0.52f) : Random.Range(0.65f, 1.05f);
            nextDecisionTime = Time.time + interval;

            float guardChance = difficulty == GameDifficulty.Hard ? 0.22f : 0.08f;
            if (Random.value < guardChance)
            {
                guardUntil = Time.time + (difficulty == GameDifficulty.Hard ? 0.38f : 0.24f);
                combat.SetGuardRequested(true);
                return;
            }

            float r = Random.value;
            if (difficulty == GameDifficulty.Hard)
            {
                if (r < 0.42f) combat.TryLightPunch();
                else if (r < 0.72f) combat.TryKick();
                else combat.TryHeavyPunch();
            }
            else
            {
                if (r < 0.52f) combat.TryLightPunch();
                else if (r < 0.82f) combat.TryKick();
                else combat.TryHeavyPunch();
            }
        }
    }
}
