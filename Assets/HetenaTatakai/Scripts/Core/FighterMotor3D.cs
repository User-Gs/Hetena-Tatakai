using System.Collections;
using UnityEngine;

namespace HetenaTatakai
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(FighterStateMachine))]
    public sealed class FighterMotor3D : MonoBehaviour
    {
        [SerializeField] private FighterStats stats;
        [SerializeField] private Transform opponent;
        [SerializeField] private KeyCode forwardKey = KeyCode.W;
        [SerializeField] private KeyCode backKey = KeyCode.S;
        [SerializeField] private KeyCode leftKey = KeyCode.A;
        [SerializeField] private KeyCode rightKey = KeyCode.D;
        [SerializeField] private Vector3 arenaCenter = Vector3.zero;
        [SerializeField] private float arenaRadius = 8f;
        [SerializeField] private bool inputEnabled = true;
        [SerializeField] private bool aiControlled;

        private CharacterController controller;
        private FighterStateMachine stateMachine;
        private Vector2 aiMoveInput;

        public FighterStats Stats => stats;
        public Transform Opponent => opponent;
        public bool InputEnabled => inputEnabled;
        public bool AIControlled => aiControlled;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            stateMachine = GetComponent<FighterStateMachine>();
        }

        public void Configure(FighterStats fighterStats, Transform target)
        {
            stats = fighterStats;
            opponent = target;
        }

        public void SetKeys(KeyCode forward, KeyCode back, KeyCode left, KeyCode right)
        {
            forwardKey = forward;
            backKey = back;
            leftKey = left;
            rightKey = right;
        }

        public void SetArena(Vector3 center, float radius)
        {
            arenaCenter = center;
            arenaRadius = Mathf.Max(2f, radius);
        }

        public void SetInputEnabled(bool value)
        {
            inputEnabled = value;
            if (!value)
            {
                aiMoveInput = Vector2.zero;
                if (stateMachine.CurrentState == FighterState.Moving)
                    stateMachine.SetState(FighterState.Neutral);
            }
        }

        public void SetAIControlled(bool value)
        {
            aiControlled = value;
            aiMoveInput = Vector2.zero;
        }

        public void SetAIMovement(Vector2 movement)
        {
            aiMoveInput = Vector2.ClampMagnitude(movement, 1f);
        }

        private void Update()
        {
            if (opponent == null || stats == null) return;

            FaceOpponent();
            if (!inputEnabled || !stateMachine.CanMove) return;

            Vector3 toOpponent = opponent.position - transform.position;
            toOpponent.y = 0f;
            if (toOpponent.sqrMagnitude < 0.001f) toOpponent = transform.forward;

            Vector3 forward = toOpponent.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            float forwardInput;
            float sideInput;

            if (aiControlled)
            {
                sideInput = aiMoveInput.x;
                forwardInput = aiMoveInput.y;
            }
            else
            {
                forwardInput = (Input.GetKey(forwardKey) ? 1f : 0f) - (Input.GetKey(backKey) ? 1f : 0f);
                sideInput = (Input.GetKey(rightKey) ? 1f : 0f) - (Input.GetKey(leftKey) ? 1f : 0f);
            }

            Vector3 desired = forward * forwardInput * stats.moveSpeed + right * sideInput * stats.sidestepSpeed;
            controller.SimpleMove(desired);
            stateMachine.SetState(desired.sqrMagnitude > 0.001f ? FighterState.Moving : FighterState.Neutral);
            ClampToArena();
        }

        private void FaceOpponent()
        {
            if (stateMachine.IsLocked) return;

            Vector3 direction = opponent.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion target = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, stats.rotationSpeed * Time.deltaTime);
        }

        private void ClampToArena()
        {
            Vector3 fromCenter = transform.position - arenaCenter;
            fromCenter.y = 0f;
            if (fromCenter.magnitude <= arenaRadius) return;

            Vector3 clamped = arenaCenter + fromCenter.normalized * arenaRadius;
            clamped.y = transform.position.y;
            controller.enabled = false;
            transform.position = clamped;
            controller.enabled = true;
        }

        public void ApplyKnockback(Vector3 direction, float distance, float duration = 0.12f)
        {
            if (distance <= 0f || !gameObject.activeInHierarchy) return;
            StartCoroutine(KnockbackRoutine(direction, distance, duration));
        }

        private IEnumerator KnockbackRoutine(Vector3 direction, float distance, float duration)
        {
            direction.y = 0f;
            direction = direction.sqrMagnitude > 0.001f ? direction.normalized : -transform.forward;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float step = distance * (Time.deltaTime / Mathf.Max(0.02f, duration));
                controller.Move(direction * step);
                elapsed += Time.deltaTime;
                yield return null;
            }

            ClampToArena();
        }
    }
}
