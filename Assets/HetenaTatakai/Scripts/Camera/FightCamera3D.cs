using UnityEngine;

namespace HetenaTatakai
{
    public sealed class FightCamera3D : MonoBehaviour
    {
        [SerializeField] private Transform fighterA;
        [SerializeField] private Transform fighterB;
        [SerializeField] private float height = 3.2f;
        [SerializeField] private float baseDistance = 6.0f;
        [SerializeField] private float distanceScale = 0.45f;
        [SerializeField] private float smooth = 7.5f;
        [SerializeField] private float minFov = 42f;
        [SerializeField] private float maxFov = 56f;

        private Camera cachedCamera;
        private Vector3 previousSide = Vector3.back;

        private void Awake() => cachedCamera = GetComponent<Camera>();

        public void Configure(Transform a, Transform b)
        {
            fighterA = a;
            fighterB = b;
        }

        private void LateUpdate()
        {
            if (fighterA == null || fighterB == null) return;

            Vector3 midpoint = (fighterA.position + fighterB.position) * 0.5f;
            Vector3 line = fighterB.position - fighterA.position;
            line.y = 0f;
            if (line.sqrMagnitude < 0.001f) line = Vector3.right;

            Vector3 side = Vector3.Cross(Vector3.up, line.normalized);
            if (Vector3.Dot(side, previousSide) < 0f) side = -side;
            previousSide = side;

            float separation = Vector3.Distance(fighterA.position, fighterB.position);
            float distance = baseDistance + separation * distanceScale;
            Vector3 desired = midpoint + side * distance + Vector3.up * height;
            float t = 1f - Mathf.Exp(-smooth * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, desired, t);
            Quaternion targetRotation = Quaternion.LookRotation((midpoint + Vector3.up * 1.05f) - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            if (cachedCamera != null)
            {
                float targetFov = Mathf.Lerp(minFov, maxFov, Mathf.InverseLerp(2f, 8f, separation));
                cachedCamera.fieldOfView = Mathf.Lerp(cachedCamera.fieldOfView, targetFov, t);
            }
        }
    }
}
