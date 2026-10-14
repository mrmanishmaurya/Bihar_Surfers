using UnityEngine;

namespace BiharSurfers.Camera
{
    /// <summary>
    /// Third-person runner camera system.
    /// Smoothly follows player, dampens jump motion Y-oscillation, triggers camera shake on collision,
    /// and scales FOV dynamically with running speed.
    /// </summary>
    public class RunnerCamera : MonoBehaviour
    {
        public static RunnerCamera Instance { get; private set; }

        [Header("Target Tracking")]
        public Transform target;
        public Vector3 offset = new Vector3(0f, 3.5f, -6.5f);
        public float followSpeed = 12f;
        public float yDamping = 4f;

        [Header("Jump Response")]
        public bool dampenJumpMotion = true;
        private float targetGroundY = 0f;

        [Header("Dynamic FOV")]
        public UnityEngine.Camera cam;
        public float baseFOV = 60f;
        public float maxFOV = 75f;

        [Header("Camera Shake")]
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (cam == null) cam = GetComponent<UnityEngine.Camera>();
        }

        private void Start()
        {
            if (target == null && Player.PlayerController.Instance != null)
            {
                target = Player.PlayerController.Instance.transform;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                if (Player.PlayerController.Instance != null)
                {
                    target = Player.PlayerController.Instance.transform;
                }
                return;
            }

            // Calculate base target follow position
            Vector3 targetPos = target.position;

            // Dampen Y jump response so camera stays steady during jump airborne phase
            if (dampenJumpMotion)
            {
                if (Player.PlayerController.Instance != null && Player.PlayerController.Instance.movement != null)
                {
                    if (Player.PlayerController.Instance.movement.isGrounded)
                    {
                        targetGroundY = targetPos.y;
                    }
                    else
                    {
                        // Lerp Y target toward ground Y + partial jump height offset
                        targetPos.y = Mathf.Lerp(targetPos.y, targetGroundY + (targetPos.y - targetGroundY) * 0.3f, Time.deltaTime * yDamping);
                    }
                }
            }

            Vector3 desiredPosition = targetPos + offset;
            desiredPosition.x = Mathf.Lerp(transform.position.x, desiredPosition.x, Time.deltaTime * followSpeed);
            desiredPosition.y = Mathf.Lerp(transform.position.y, desiredPosition.y, Time.deltaTime * yDamping);
            desiredPosition.z = target.position.z + offset.z; // Direct Z follow for crisp forward motion

            // Camera Shake Offset
            if (shakeDuration > 0)
            {
                desiredPosition += Random.insideUnitSphere * shakeMagnitude;
                shakeDuration -= Time.deltaTime;
            }

            transform.position = desiredPosition;
            transform.LookAt(target.position + Vector3.up * 1.5f);

            // Dynamic FOV scaling based on player running speed
            if (cam != null && Player.PlayerController.Instance != null && Player.PlayerController.Instance.movement != null)
            {
                float speedRatio = Player.PlayerController.Instance.movement.forwardSpeed / Player.PlayerController.Instance.movement.maximumSpeed;
                cam.fieldOfView = Mathf.Lerp(baseFOV, maxFOV, speedRatio);
            }
        }

        public void TriggerShake(float duration = 0.35f, float magnitude = 0.4f)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }
    }
}
