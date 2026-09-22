using System.Collections;
using UnityEngine;

namespace HetenaTatakai
{
    [RequireComponent(typeof(FighterHealth))]
    [RequireComponent(typeof(FighterStateMachine))]
    [RequireComponent(typeof(FighterMotor3D))]
    public sealed class FighterCombatController : MonoBehaviour
    {
        [SerializeField] private FighterStats stats;
        [SerializeField] private FighterCombatController target;
        [SerializeField] private Hitbox attackHitbox;

        [Header("Input")]
        [SerializeField] private KeyCode lightPunchKey = KeyCode.J;
        [SerializeField] private KeyCode heavyPunchKey = KeyCode.K;
        [SerializeField] private KeyCode kickKey = KeyCode.L;
        [SerializeField] private KeyCode guardKey = KeyCode.I;

        [Header("Attacks")]
        [SerializeField] private AttackDefinition lightPunch = new AttackDefinition("Light Punch", 2, 0.08f, 0.10f, 0.18f, 0.20f, 0.35f, 0.25f);
        [SerializeField] private AttackDefinition heavyPunch = new AttackDefinition("Heavy Punch", 4, 0.20f, 0.12f, 0.42f, 0.38f, 0.75f, 0.35f, true);
        [SerializeField] private AttackDefinition kick = new AttackDefinition("Kick", 3, 0.16f, 0.14f, 0.34f, 0.30f, 0.60f, 0.30f);

        [Header("Combo")]
        [SerializeField] private float comboResetWindow = 0.85f;
        [SerializeField] private int maxCombo = 3;

        private FighterHealth health;
        private FighterStateMachine stateMachine;
        private FighterMotor3D motor;
        private int attacksPerformed;
        private int comboCount;
        private float lastConnectTime = -100f;
        private Coroutine attackRoutine;
        private Coroutine stunRoutine;

        public bool IsGuarding => stateMachine.CurrentState == FighterState.Guarding;
        public bool IsStunned => stateMachine.CurrentState == FighterState.HitStun || stateMachine.CurrentState == FighterState.Knockdown;
        public int ComboCount => comboCount;
        public FighterHealth Health => health;

        private void Awake()
        {
            health = GetComponent<FighterHealth>();
            stateMachine = GetComponent<FighterStateMachine>();
            motor = GetComponent<FighterMotor3D>();
        }

        private void Start()
        {
            if (stats != null) health.Configure(stats.maxHealth);
        }

        public void Configure(FighterStats fighterStats, FighterCombatController opponent, Hitbox hitbox)
        {
            stats = fighterStats;
            target = opponent;
            attackHitbox = hitbox;
            if (attackHitbox != null) attackHitbox.Configure(this);
            health.Configure(stats != null ? stats.maxHealth : 100f);
        }

        public void SetKeys(KeyCode light, KeyCode heavy, KeyCode kickAttack, KeyCode guard)
        {
            lightPunchKey = light;
            heavyPunchKey = heavy;
            kickKey = kickAttack;
            guardKey = guard;
        }

        private void Update()
        {
            if (health.IsKO || stats == null) return;

            if (stateMachine.CanGuard && Input.GetKey(guardKey))
            {
                stateMachine.SetState(FighterState.Guarding);
                return;
            }

            if (stateMachine.CurrentState == FighterState.Guarding)
                stateMachine.SetState(FighterState.Neutral);

            if (!stateMachine.CanAttack) return;

            if (Input.GetKeyDown(lightPunchKey)) StartAttack(lightPunch);
            else if (Input.GetKeyDown(heavyPunchKey)) StartAttack(heavyPunch);
            else if (Input.GetKeyDown(kickKey)) StartAttack(kick);
        }

        private void StartAttack(AttackDefinition attack)
        {
            if (attackRoutine != null) StopCoroutine(attackRoutine);
            attackRoutine = StartCoroutine(AttackRoutine(attack));
        }

        private IEnumerator AttackRoutine(AttackDefinition attack)
        {
            stateMachine.SetState(FighterState.Attacking);
            attacksPerformed++;
            motor.SetInputEnabled(false);

            if (attack.startup > 0f) yield return new WaitForSeconds(attack.startup);

            D6Result roll = D6CombatResolver.Roll(stats, attack);
            Vector3 direction = target != null ? target.transform.position - transform.position : transform.forward;
            CombatHit hit = new CombatHit(gameObject, roll.FinalDamage, attack.hitStun, attack.knockback, roll.Critical, direction);

            attackHitbox?.Arm(hit);
            yield return new WaitForSeconds(attack.activeTime);
            attackHitbox?.Disarm();

            float recovery = attack.recovery * FatigueModel.RecoveryMultiplier(stats.energy, attacksPerformed);
            if (recovery > 0f) yield return new WaitForSeconds(recovery);

            if (!health.IsKO && !stateMachine.IsLocked)
                stateMachine.SetState(FighterState.Neutral);
            motor.SetInputEnabled(!health.IsKO);
            attackRoutine = null;
        }

        public void ReceiveHit(CombatHit hit)
        {
            if (health.IsKO || hit.Attacker == gameObject) return;

            float damage = hit.Damage;
            if (IsGuarding)
                damage = Mathf.Max(1f, Mathf.Ceil(damage * 0.30f));

            float applied = health.TakeDamage(damage);
            if (applied <= 0f) return;

            if (health.IsKO)
            {
                CancelAction();
                stateMachine.SetState(FighterState.KO);
                motor.SetInputEnabled(false);
                return;
            }

            if (IsGuarding)
            {
                motor.ApplyKnockback(hit.Direction, hit.Knockback * 0.35f);
                return;
            }

            float stunDuration = hit.Critical ? Mathf.Max(0.55f, hit.HitStun * 1.65f) : hit.HitStun;
            ApplyHitStun(stunDuration, hit.Direction, hit.Knockback, hit.Critical);
        }

        private void ApplyHitStun(float duration, Vector3 direction, float knockback, bool critical)
        {
            CancelAction();
            if (stunRoutine != null) StopCoroutine(stunRoutine);
            stunRoutine = StartCoroutine(HitStunRoutine(duration, direction, knockback, critical));
        }

        private IEnumerator HitStunRoutine(float duration, Vector3 direction, float knockback, bool critical)
        {
            motor.SetInputEnabled(false);
            stateMachine.SetState(critical ? FighterState.Knockdown : FighterState.HitStun);
            motor.ApplyKnockback(direction, knockback * (critical ? 1.25f : 1f));
            yield return new WaitForSeconds(Mathf.Max(0.05f, duration));

            if (!health.IsKO)
            {
                stateMachine.SetState(FighterState.Neutral);
                motor.SetInputEnabled(true);
            }
            stunRoutine = null;
        }

        public void NotifyAttackConnected()
        {
            if (Time.time - lastConnectTime > comboResetWindow) comboCount = 0;
            comboCount = Mathf.Clamp(comboCount + 1, 1, maxCombo);
            lastConnectTime = Time.time;
        }

        private void CancelAction()
        {
            attackHitbox?.Disarm();
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
        }

        public void ResetForRound()
        {
            CancelAction();
            if (stunRoutine != null)
            {
                StopCoroutine(stunRoutine);
                stunRoutine = null;
            }
            attacksPerformed = 0;
            comboCount = 0;
            health.Configure(stats != null ? stats.maxHealth : 100f);
            stateMachine.ForceReset();
            motor.SetInputEnabled(true);
        }
    }
}
